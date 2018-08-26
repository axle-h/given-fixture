using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Core;
using Autofac.Extras.Moq;
using AutoFixture;
using FluentAssertions;
using Moq;

namespace FluentFixture
{
    public class TestFixture<TSubject> : ITestFixture
    {
        private readonly List<Parameter> _parameters = new List<Parameter>();
        private readonly List<Action<object>> _resultAssertions = new List<Action<object>>();
        private readonly List<Action<Exception>> _exceptionAssertions = new List<Action<Exception>>();
        private Func<TSubject, object> _act;
        private bool _shouldThrow;

        /// <summary>
        /// Gets the auto mock container.
        /// </summary>
        public AutoMock AutoMock { get; } = AutoMock.GetStrict();

        /// <summary>
        /// An auto fixture instance for convenience.
        /// Customizations should be done in the arrange step.
        /// </summary>
        public Fixture AutoFixture { get; } = new Fixture();

        /// <summary>
        /// Creates an instance of the specified model type.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns></returns>
        public TModel Create<TModel>() => AutoFixture.Create<TModel>();

        /// <summary>
        /// Creates many instances of the specified model type.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns></returns>
        public ICollection<TModel> CreateMany<TModel>() => AutoFixture.CreateMany<TModel>().ToList();

        /// <summary>
        /// Gets the mock with the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public Mock<TService> Mock<TService>(params Parameter[] parameters) where TService : class =>
            AutoMock.Mock<TService>(parameters);

        /// <summary>
        /// Passes the specified subject parameters to the auto mock container.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public void WithSubjectParameters(params Parameter[] parameters) => _parameters.AddRange(parameters);

        /// <summary>
        /// Specifies that the act step should use the specified function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="act">The act function.</param>
        /// <returns></returns>
        public ITestFixture Act<TResult>(Func<TSubject, TResult> act)
        {
            _act = r => act(r);
            return this;
        }

        /// <summary>
        /// Specifies that the act step should use the specified asynchronous function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="act">The act function.</param>
        /// <returns></returns>
        public ITestFixture ActAsync<TResult>(Func<TSubject, Task<TResult>> act)
        {
            _act = r => act(r).Result;
            return this;
        }

        /// <summary>
        /// Includes the specified actions in the assert step.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asserts">The assert actions.</param>
        /// <returns></returns>
        public ITestFixture Assert<TResult>(params Action<TResult>[] asserts)
        {
            return asserts.Any()
                       ? Assert(asserts.Select<Action<TResult>, Action<object>>(a => o => a(o.Should().BeAssignableTo<TResult>().Which)).ToArray())
                       : Assert(o => o.Should().BeAssignableTo<TResult>());
        }

        /// <summary>
        /// Includes the specified actions in the assert step.
        /// </summary>
        /// <param name="asserts">The assert actions.</param>
        /// <returns></returns>
        public ITestFixture Assert(params Action<object>[] asserts)
        {
            _resultAssertions.AddRange(asserts);
            return this;
        }

        /// <summary>
        /// Adds an action to the assert step that asserts that an exception of the specified type is thrown by the act step.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="asserts">The asserts.</param>
        /// <returns></returns>
        public ITestFixture ShouldThrow<TException>(params Action<TException>[] asserts)
        {
            var exceptionAssertions = asserts.Any()
                                          ? asserts.Select<Action<TException>, Action<Exception>>(a => e => a(e.Should().BeOfType<TException>().Which))
                                          : new Action<Exception>[] { o => o.Should().BeOfType<TException>() };

            return ShouldThrow(exceptionAssertions.ToArray());
        }

        /// <summary>
        /// Adds an action to the assert step that asserts that an exception is thrown by the act step.
        /// </summary>
        /// <param name="asserts">The asserts.</param>
        /// <returns></returns>
        public ITestFixture ShouldThrow(params Action<Exception>[] asserts)
        {
            _shouldThrow = true;
            _exceptionAssertions.AddRange(asserts);
            return this;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_act == null)
            {
                // Exception probably thrown in arrange step.
                return;
            }

            var subject = AutoMock.Create<TSubject>(_parameters.ToArray());
            try
            {
                var result = _act(subject);
                if (_shouldThrow)
                {
                    throw new DidNotThrowException(result);
                }

                using (var aggregator = new ExceptionAggregator())
                {
                    foreach (var action in _resultAssertions)
                    {
                        aggregator.Aggregate(() => action(result));
                    }
                }
            }
            catch (DidNotThrowException)
            {
                throw;
            }
            catch (AggregateException ae)
            {
                if (!_shouldThrow)
                {
                    throw;
                }

                using (var aggregator = new ExceptionAggregator())
                {
                    foreach (var action in _exceptionAssertions)
                    {
                        foreach (var exception in ae.InnerExceptions)
                        {
                            aggregator.Aggregate(() => action(exception));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!_shouldThrow)
                {
                    throw;
                }

                using (var aggregator = new ExceptionAggregator())
                {
                    foreach (var action in _exceptionAssertions)
                    {
                        aggregator.Aggregate(() => action(e));
                    }
                }
            }
        }

        private class ExceptionAggregator : IDisposable
        {
            private readonly ICollection<Exception> _exceptions = new List<Exception>();

            public void Aggregate(Action action)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    _exceptions.Add(e);
                }
            }

            public void Dispose()
            {
                if (!_exceptions.Any())
                {
                    return;
                }

                if (_exceptions.Count == 1)
                {
                    throw _exceptions.First();
                }

                throw new AggregateException(_exceptions);
            }
        }

        private class DidNotThrowException : Exception
        {
            public DidNotThrowException(object result) : base($"Expected to throw but did not. Subject returned {result ?? "<null>"}")
            {
            }
        }
    }
}
