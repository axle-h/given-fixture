using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Core;
using Autofac.Extras.Moq;
using AutoFixture;
using Bogus;
using FluentAssertions;
using GivenFixture.Infrastructure;

namespace GivenFixture
{
    internal class TestFixture : ITestFixture
    {
        private readonly List<Parameter> _parameters = new List<Parameter>();
        private readonly List<Action<object>> _resultAssertions = new List<Action<object>>();
        private readonly List<Action<Exception>> _exceptionAssertions = new List<Action<Exception>>();
        private Func<object> _act;
        private Func<Task<object>> _actAsync;
        private bool _shouldThrow;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixture"/> class.
        /// </summary>
        /// <param name="strict">if set to <c>true</c> [strict].</param>
        internal TestFixture(bool strict)
        {
            AutoMock = strict ? AutoMock.GetLoose() : AutoMock.GetStrict();
        }

        /// <summary>
        /// Gets the auto mock container.
        /// </summary>
        public AutoMock AutoMock { get; }

        /// <summary>
        /// An auto fixture instance for convenience.
        /// </summary>
        public IFixture AutoFixture { get; } = new Fixture();

        /// <summary>
        /// A faker instance for convenience.
        /// </summary>
        public Faker Faker { get; } = new Faker();

        /// <summary>
        /// Gets the properties.
        /// </summary>
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Passes the specified subject parameters to the auto mock container.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public ITestFixture HavingSubjectParameters(params Parameter[] parameters)
        {
            _parameters.AddRange(parameters);
            return this;
        }

        /// <summary>
        /// Specifies that the act step should use the specified function and subject type.
        /// </summary>
        /// <typeparam name="TSubject"></typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="act">The act function.</param>
        /// <returns></returns>
        public ITestFixture When<TSubject, TResult>(Func<TSubject, TResult> act)
        {
            AssertNoActStep();
            _act = () => act(GetSubject<TSubject>());
            return this;
        }

        /// <summary>
        /// Specifies that the act step should use the specified static function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="act">The act function.</param>
        /// <returns></returns>
        public ITestFixture When<TResult>(Func<TResult> act)
        {
            AssertNoActStep();
            _act = () => act();
            return this;
        }

        /// <summary>
        /// Specifies that the act step should use the specified asynchronous function and subject type.
        /// </summary>
        /// <typeparam name="TSubject">The type of the subject.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="act">The act function.</param>
        /// <returns></returns>
        public ITestFixture When<TSubject, TResult>(Func<TSubject, Task<TResult>> act)
        {
            AssertNoActStep();
            _actAsync = async () => await act(GetSubject<TSubject>());
            return this;
        }

        /// <summary>
        /// Specifies that the act step should use the specified static function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="act">The act function.</param>
        /// <returns></returns>
        public ITestFixture When<TResult>(Func<Task<TResult>> act)
        {
            AssertNoActStep();
            _actAsync = async () => await act();
            return this;
        }

        /// <summary>
        /// Includes the specified actions in the assert step.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asserts">The assert actions.</param>
        /// <returns></returns>
        public ITestFixture ShouldReturn<TResult>(params Action<TResult>[] asserts) =>
            asserts.Any()
                ? ShouldReturn(asserts.Select<Action<TResult>, Action<object>>(a => o => a(o.Should().BeAssignableTo<TResult>().Which)).ToArray())
                : ShouldReturn(o => o.Should().BeAssignableTo<TResult>());

        /// <summary>
        /// Includes the specified actions in the assert step.
        /// </summary>
        /// <param name="asserts">The assert actions.</param>
        /// <returns></returns>
        public ITestFixture ShouldReturn(params Action<object>[] asserts)
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
        /// Runs the fixture synchronously.
        /// </summary>
        public void Run()
        {
            if (_act == null)
            {
                throw new InvalidOperationException($"Must run {nameof(When)} to configure the synchronous act step");
            }

            try
            {
                var result = _act();
                if (_shouldThrow)
                {
                    throw new DidNotThrowException(result);
                }

                RunResultAssertions(result);
            }
            catch (DidNotThrowException)
            {
                throw;
            }
            catch (Exception e)
            {
                if (!_shouldThrow)
                {
                    throw;
                }

                RunExceptionAssertions(e);
            }
        }

        /// <summary>
        /// Runs the fixture asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync()
        {
            if (_actAsync == null)
            {
                throw new InvalidOperationException($"Must run {nameof(When)} to configure the asynchronous act step");
            }

            try
            {
                var result = await _actAsync();
                if (_shouldThrow)
                {
                    throw new DidNotThrowException(result);
                }

                RunResultAssertions(result);
            }
            catch (DidNotThrowException)
            {
                throw;
            }
            catch (Exception e)
            {
                if (!_shouldThrow)
                {
                    throw;
                }

                RunExceptionAssertions(e);
            }
        }

        private TSubject GetSubject<TSubject>() => AutoMock.Create<TSubject>(_parameters.ToArray());

        private void AssertNoActStep()
        {
            if (_act != null || _actAsync != null)
            {
                throw new InvalidOperationException("Act step already configured");
            }
        }

        private void RunResultAssertions(object result)
        {
            using (var aggregator = new ExceptionAggregator())
            {
                foreach (var action in _resultAssertions)
                {
                    aggregator.Try(() => action(result));
                }
            }
        }

        private void RunExceptionAssertions(Exception e)
        {
            using (var aggregator = new ExceptionAggregator())
            {
                foreach (var action in _exceptionAssertions)
                {
                    aggregator.Try(() => action(e));
                }
            }
        }
    }
}
