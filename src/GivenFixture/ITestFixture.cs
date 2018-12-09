using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac.Core;
using Autofac.Extras.Moq;
using AutoFixture;
using Bogus;

namespace GivenFixture
{
    /// <summary>
    /// The test fixture.
    /// </summary>
    public interface ITestFixture
    {
        /// <summary>
        /// Gets the auto mock container.
        /// </summary>
        AutoMock AutoMock { get; }

        /// <summary>
        /// An auto fixture instance for convenience.
        /// </summary>
        IFixture AutoFixture { get; }

        /// <summary>
        /// A faker instance for convenience.
        /// </summary>
        Faker Faker { get; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Passes the specified subject parameters to the auto mock container.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        ITestFixture HavingSubjectParameters(params Parameter[] parameters);

        /// <summary>
        /// Specifies that the act step should use the specified function and subject type.
        /// </summary>
        /// <typeparam name="TSubject"></typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="act">The act function.</param>
        /// <returns></returns>
        ITestFixture When<TSubject, TResult>(Func<TSubject, TResult> act);
        
        /// <summary>
        /// Specifies that the act step should use the specified asynchronous function and subject type.
        /// </summary>
        /// <typeparam name="TSubject">The type of the subject.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="act">The act function.</param>
        /// <returns></returns>
        ITestFixture When<TSubject, TResult>(Func<TSubject, Task<TResult>> act);

        /// <summary>
        /// Specifies that the act step should use the specified action and subject type.
        /// </summary>
        /// <typeparam name="TSubject">The type of the subject.</typeparam>
        /// <param name="act">The act function.</param>
        /// <returns></returns>
        ITestFixture When<TSubject>(Action<TSubject> act);

        /// <summary>
        /// Specifies that the act step should use the specified static function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="act">The act function.</param>
        /// <returns></returns>
        ITestFixture WhenStatic<TResult>(Func<TResult> act);

        /// <summary>
        /// Specifies that the act step should use the specified static function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="act">The act function.</param>
        /// <returns></returns>
        ITestFixture WhenStatic<TResult>(Func<Task<TResult>> act);

        /// <summary>
        /// Includes the specified actions in the assert step.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asserts">The assert actions.</param>
        /// <returns></returns>
        ITestFixture ShouldReturn<TResult>(params Action<TResult>[] asserts);

        /// <summary>
        /// Includes the specified actions in the assert step.
        /// </summary>
        /// <param name="asserts">The assert actions.</param>
        /// <returns></returns>
        ITestFixture ShouldReturn(params Action<object>[] asserts);

        /// <summary>
        /// Adds an assertion to this fixture that an exception of the specified type is thrown by the act step.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="asserts">The asserts.</param>
        /// <returns></returns>
        ITestFixture ShouldThrow<TException>(params Action<TException>[] asserts);

        /// <summary>
        /// Adds an assertion to this fixture that an exception off nay type is thrown by the act step.
        /// </summary>
        /// <param name="asserts">The asserts.</param>
        /// <returns></returns>
        ITestFixture ShouldThrow(params Action<Exception>[] asserts);

        /// <summary>
        /// Runs the fixture synchronously.
        /// </summary>
        void Run();

        /// <summary>
        /// Runs the fixture asynchronously.
        /// </summary>
        /// <returns></returns>
        Task RunAsync();
    }
}
