using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac.Core;
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
        /// <param name="service">The service.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static ITestFixture HavingMock<TService>(this ITestFixture fixture,
                                                        Action<Mock<TService>> mockAction,
                                                        out TService service,
                                                        params Parameter[] parameters)
            where TService : class
        {
            var mock = fixture.AutoMock.Mock<TService>(parameters);
            mockAction(mock);
            service = mock.Object;
            return fixture;
        }

        /// <summary>
        /// Configures a mock of the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="mockAction">The mock action.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static ITestFixture HavingMock<TService>(this ITestFixture fixture,
                                                        Action<Mock<TService>> mockAction,
                                                        params Parameter[] parameters)
            where TService : class =>
            fixture.HavingMock(mockAction, out _, parameters);

        /// <summary>
        /// Gets a mocked object of the specified type
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="service">The service.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static ITestFixture HavingMock<TService>(this ITestFixture fixture,
                                                        out TService service,
                                                        params Parameter[] parameters)
            where TService : class =>
            fixture.HavingMock(_ => {}, out service, parameters);

        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the specified expression to return the specified result.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="result">The result.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static ITestFixture HavingMockedAsync<TService, TResult>(this ITestFixture fixture,
                                                                        Expression<Func<TService, Task<TResult>>> expression,
                                                                        TResult result,
                                                                        params Parameter[] parameters)
            where TService : class =>
            fixture.HavingMock<TService>(m => m.Setup(expression).ReturnsAsync(result).Verifiable(), parameters);

        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the specified expression to return a result of the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="result">The result.</param>
        /// <param name="composer">The composer.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static ITestFixture HavingMockedAsync<TService, TResult>(this ITestFixture fixture,
                                                                        Expression<Func<TService, Task<TResult>>> expression,
                                                                        out TResult result,
                                                                        Func<IPostprocessComposer<TResult>, IPostprocessComposer<TResult>> composer = null,
                                                                        params Parameter[] parameters)
            where TService : class =>
            fixture.HavingModel(out result, composer)
                   .HavingMockedAsync(expression, result, parameters);

        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the specified expression to return a collection of the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="result">The result.</param>
        /// <param name="composer">The composer.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static ITestFixture HavingMockedCollectionAsync<TService, TResult>(this ITestFixture fixture,
                                                                                  Expression<Func<TService, Task<ICollection<TResult>>>> expression,
                                                                                  out ICollection<TResult> result,
                                                                                  Func<IPostprocessComposer<TResult>, IPostprocessComposer<TResult>> composer = null,
                                                                                  params Parameter[] parameters)
            where TService : class =>
            fixture.HavingModels(out result, composer)
                   .HavingMockedAsync(expression, result, parameters);

        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the specified expression to return the specified result.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="result">The result.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static ITestFixture HavingMocked<TService, TResult>(this ITestFixture fixture,
                                                                   Expression<Func<TService, TResult>> expression,
                                                                   TResult result,
                                                                   params Parameter[] parameters)
            where TService : class =>
            fixture.HavingMock<TService>(m => m.Setup(expression).Returns(result).Verifiable(), parameters);

        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the specified expression to return a result of the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="result">The result.</param>
        /// <param name="composer">The composer.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static ITestFixture HavingMocked<TService, TResult>(this ITestFixture fixture,
                                                                   Expression<Func<TService, TResult>> expression,
                                                                   out TResult result,
                                                                   Func<IPostprocessComposer<TResult>, IPostprocessComposer<TResult>> composer = null,
                                                                   params Parameter[] parameters)
            where TService : class =>
            fixture.HavingModel(out result, composer)
                   .HavingMocked(expression, result, parameters);

        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the specified expression to return a result of the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="result">The result.</param>
        /// <param name="composer">The composer.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static ITestFixture HavingMockedCollection<TService, TResult>(this ITestFixture fixture,
                                                                             Expression<Func<TService, ICollection<TResult>>> expression,
                                                                             out ICollection<TResult> result,
                                                                             Func<IPostprocessComposer<TResult>, IPostprocessComposer<TResult>> composer = null,
                                                                             params Parameter[] parameters)
            where TService : class =>
            fixture.HavingModels(out result, composer)
                   .HavingMocked(expression, result, parameters);

        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the specified expression.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static ITestFixture HavingMocked<TService>(this ITestFixture fixture,
                                                          Expression<Action<TService>> expression,
                                                          params Parameter[] parameters)
            where TService : class =>
            fixture.HavingMock<TService>(m => m.Setup(expression).Verifiable(), parameters);

        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the specified expression to return a completed task.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static ITestFixture HavingMockedAsync<TService>(this ITestFixture fixture,
                                                               Expression<Func<TService, Task>> expression,
                                                               params Parameter[] parameters)
            where TService : class =>
            fixture.HavingMock<TService>(m => m.Setup(expression).Returns(Task.CompletedTask).Verifiable(), parameters);


        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the specified expression to throw the specified exception.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static ITestFixture HavingMockThrow<TService>(this ITestFixture fixture,
                                                             Expression<Action<TService>> expression,
                                                             Exception exception,
                                                             params Parameter[] parameters)
            where TService : class =>
            fixture.HavingMock<TService>(m => m.Setup(expression).Throws(exception).Verifiable(), parameters);

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
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static ITestFixture HavingMockThrow<TService, TException>(this ITestFixture fixture,
                                                                         Expression<Action<TService>> expression,
                                                                         out TException exception,
                                                                         Func<IPostprocessComposer<TException>, IPostprocessComposer<TException>> composer = null,
                                                                         params Parameter[] parameters)
            where TService : class
            where TException : Exception =>
            fixture.HavingModel(out exception, composer)
                   .HavingMockThrow(expression, exception, parameters);

        /// <summary>
        /// Configures a mock of the specified type to setup a call matching the
        /// specified expression to throw an exception of the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="composer">The composer.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static ITestFixture HavingMockThrow<TService, TException>(this ITestFixture fixture,
                                                                         Expression<Action<TService>> expression,
                                                                         Func<IPostprocessComposer<TException>, IPostprocessComposer<TException>> composer = null,
                                                                         params Parameter[] parameters)
            where TService : class
            where TException : Exception =>
            fixture.HavingMockThrow(expression, out _, composer, parameters);
    }
}