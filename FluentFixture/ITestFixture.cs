using System;
using System.Collections.Generic;
using Autofac.Core;
using Autofac.Extras.Moq;
using AutoFixture;
using Moq;

namespace FluentFixture
{
    public interface ITestFixture : IDisposable
    {
        /// <summary>
        /// Gets the auto mock container.
        /// </summary>
        AutoMock AutoMock { get; }

        /// <summary>
        /// An auto fixture instance for convenience.
        /// Customizations should be done in the arrange step.
        /// </summary>
        Fixture AutoFixture { get; }

        /// <summary>
        /// Creates an instance of the specified model type.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns></returns>
        TModel Create<TModel>();

        /// <summary>
        /// Creates many instances of the specified model type.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns></returns>
        ICollection<TModel> CreateMany<TModel>();

        /// <summary>
        /// Gets the mock with the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        Mock<TService> Mock<TService>(params Parameter[] parameters) where TService : class;

        /// <summary>
        /// Passes the specified subject parameters to the auto mock container.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        void WithSubjectParameters(params Parameter[] parameters);
        
        /// <summary>
        /// Includes the specified actions in the assert step.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asserts">The assert actions.</param>
        /// <returns></returns>
        ITestFixture Assert<TResult>(params Action<TResult>[] asserts);

        /// <summary>
        /// Includes the specified actions in the assert step.
        /// </summary>
        /// <param name="asserts">The assert actions.</param>
        /// <returns></returns>
        ITestFixture Assert(params Action<object>[] asserts);

        /// <summary>
        /// Adds an action to the assert step that asserts that an exception of the specified type is thrown by the act step.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="asserts">The asserts.</param>
        /// <returns></returns>
        ITestFixture ShouldThrow<TException>(params Action<TException>[] asserts);

        /// <summary>
        /// Adds an action to the assert step that asserts that an exception is thrown by the act step.
        /// </summary>
        /// <param name="asserts">The asserts.</param>
        /// <returns></returns>
        ITestFixture ShouldThrow(params Action<Exception>[] asserts);
    }
}
