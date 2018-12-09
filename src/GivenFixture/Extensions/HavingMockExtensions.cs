using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture.Dsl;
using Moq;

namespace GivenFixture.Extensions
{
    /// <summary>
    /// Moq extensions for <see cref="ITestFixture"/>.
    /// </summary>
    public static class HavingMockExtensions
    {
        /// <summary>
        /// Configures a mock of the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="mockAction">The mock action.</param>
        /// <returns></returns>
        public static ITestFixture HavingMock<TService>(this ITestFixture fixture,
                                                        Action<Mock<TService>> mockAction)
            where TService : class
        {
            var mock = fixture.AutoMock.Mock<TService>();
            mockAction(mock);
            return fixture;
        }

        /// <summary>
        /// Gets a mocked object of the specified type
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        public static ITestFixture HavingMock<TService>(this ITestFixture fixture, out TService service)
            where TService : class
        {
            service = fixture.AutoMock.Mock<TService>().Object;
            return fixture;
        }

        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the specified expression to return the specified result.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="result">The result.</param>
        /// <param name="because">The because.</param>
        /// <returns></returns>
        public static ITestFixture HavingMocked<TService, TResult>(this ITestFixture fixture,
                                                                   Expression<Func<TService, Task<TResult>>> expression,
                                                                   TResult result,
                                                                   string because = null)
            where TService : class =>
            fixture.HavingMock<TService>(m => m.Setup(expression).ReturnsAsync(result).Verifiable(because));

        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the specified expression to return a result of the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="result">The result.</param>
        /// <param name="composer">The composer.</param>
        /// <param name="because">The because.</param>
        /// <returns></returns>
        public static ITestFixture HavingMocked<TService, TResult>(this ITestFixture fixture,
                                                                   Expression<Func<TService, Task<TResult>>> expression,
                                                                   out TResult result,
                                                                   Func<IPostprocessComposer<TResult>, IPostprocessComposer<TResult>> composer = null,
                                                                   string because = null)
            where TService : class =>
            fixture.HavingModel(out result, composer)
                   .HavingMocked(expression, result, because);


        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the specified expression to return the specified result.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="result">The result.</param>
        /// <param name="because">The because.</param>
        /// <returns></returns>
        public static ITestFixture HavingMocked<TService, TResult>(this ITestFixture fixture,
                                                                   Expression<Func<TService, TResult>> expression,
                                                                   TResult result,
                                                                   string because = null)
            where TService : class =>
            fixture.HavingMock<TService>(m => m.Setup(expression).Returns(result).Verifiable(because));

        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the specified expression to return a result of the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="result">The result.</param>
        /// <param name="composer">The composer.</param>
        /// <param name="because">The because.</param>
        /// <returns></returns>
        public static ITestFixture HavingMocked<TService, TResult>(this ITestFixture fixture,
                                                                   Expression<Func<TService, TResult>> expression,
                                                                   out TResult result,
                                                                   Func<IPostprocessComposer<TResult>, IPostprocessComposer<TResult>> composer = null,
                                                                   string because = null)
            where TService : class =>
            fixture.HavingModel(out result, composer)
                   .HavingMocked(expression, result, because);

        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the specified expression.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="because">The because.</param>
        /// <returns></returns>
        public static ITestFixture HavingMocked<TService>(this ITestFixture fixture,
                                                          Expression<Action<TService>> expression,
                                                          string because = null)
            where TService : class =>
            fixture.HavingMock<TService>(m => m.Setup(expression).Verifiable(because));

        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the specified expression to return a completed task.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="because">The because.</param>
        /// <returns></returns>
        public static ITestFixture HavingMocked<TService>(this ITestFixture fixture,
                                                          Expression<Func<TService, Task>> expression,
                                                          string because = null)
            where TService : class =>
            fixture.HavingMock<TService>(m => m.Setup(expression).Returns(Task.CompletedTask).Verifiable(because));


        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the specified expression to throw the specified exception.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="because">The because.</param>
        /// <returns></returns>
        public static ITestFixture HavingMockThrow<TService>(this ITestFixture fixture,
                                                             Expression<Action<TService>> expression,
                                                             Exception exception,
                                                             string because = null)
            where TService : class =>
            fixture.HavingMock<TService>(m => m.Setup(expression).Throws(exception).Verifiable(because));

        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the
        /// specified expression to throw an exception of the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="composer">The composer.</param>
        /// <param name="because">The because.</param>
        /// <returns></returns>
        public static ITestFixture HavingMockThrow<TService, TException>(this ITestFixture fixture,
                                                                         Expression<Action<TService>> expression,
                                                                         out TException exception,
                                                                         Func<IPostprocessComposer<TException>, IPostprocessComposer<TException>> composer = null,
                                                                         string because = null)
            where TService : class
            where TException : Exception =>
            fixture.HavingModel(out exception, composer)
                   .HavingMockThrow(expression, exception, because);

        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the
        /// specified expression to throw an exception of the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="composer">The composer.</param>
        /// <param name="because">The because.</param>
        /// <returns></returns>
        public static ITestFixture HavingMockThrow<TService, TException>(this ITestFixture fixture,
                                                                         Expression<Action<TService>> expression,
                                                                         Func<IPostprocessComposer<TException>, IPostprocessComposer<TException>> composer = null,
                                                                         string because = null)
            where TService : class
            where TException : Exception =>
            fixture.HavingMockThrow(expression, out _, composer, because);
    }
}