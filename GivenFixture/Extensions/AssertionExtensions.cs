using System;
using System.Collections;
using FluentAssertions;

namespace GivenFixture.Extensions
{
    /// <summary>
    /// Assertion extensions for <see cref="ITestFixture"/>.
    /// </summary>
    public static class AssertionExtensions
    {
        /// <summary>
        /// Adds an assertion to the specified fixture that the result returned should be an object with the exact same reference as the specified result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static ITestFixture ShouldReturnSameAs<TResult>(this ITestFixture fixture, TResult result) =>
            fixture.ShouldReturn<TResult>(x => x.Should().BeSameAs(result));

        /// <summary>
        /// Adds an assertion to the specified fixture that the result returned should be an object that equals the specified result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static ITestFixture ShouldReturn<TResult>(this ITestFixture fixture, TResult result) =>
            fixture.ShouldReturn<TResult>(x => x.Should().Be(result));

        /// <summary>
        /// Adds an assertion to the specified fixture that the result returned should be an object equivalent to the specified result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static ITestFixture ShouldReturnEquivalent<TResult>(this ITestFixture fixture, TResult result) =>
            fixture.ShouldReturn(x => x.Should().BeEquivalentTo(result));

        /// <summary>
        /// Adds an assertion to the specified fixture that the result returned should be null.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static ITestFixture ShouldReturnNull(this ITestFixture fixture) =>
            fixture.ShouldReturn(x => x.Should().BeNull());

        /// <summary>
        /// Adds an assertion to the specified fixture that the result returned should be true.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static ITestFixture ShouldReturnTrue(this ITestFixture fixture) =>
            fixture.ShouldReturn<bool>(x => x.Should().BeTrue());

        /// <summary>
        /// Adds an assertion to the specified fixture that the result returned should be false.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static ITestFixture ShouldReturnFalse(this ITestFixture fixture) =>
            fixture.ShouldReturn<bool>(x => x.Should().BeFalse());
        
        /// <summary>
        /// Adds an assertion to the specified fixture that the result returned will be an empty collection.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static ITestFixture ShouldReturnEmptyCollection(this ITestFixture fixture) =>
            fixture.ShouldReturn<IEnumerable>(x => x.Should().BeEmpty());

        /// <summary>
        /// Adds an assertion to the specified fixture that the result returned will be a collection of the specified length.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static ITestFixture ShouldReturnCollectionWithCount(this ITestFixture fixture, int count) =>
            fixture.ShouldReturn<IEnumerable>(x => x.Should().HaveCount(count));

        /// <summary>
        /// Adds an assertion to the specified fixture that the result returned will be a collection with the same length as the specified collection.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static ITestFixture ShouldReturnCollectionWithSameCount(this ITestFixture fixture, IEnumerable collection) =>
            fixture.ShouldReturn<IEnumerable>(x => x.Should().HaveSameCount(collection));

        /// <summary>
        /// Adds an assertion to the specified fixture that an argument null exception will be thrown with the specified parameter name.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns></returns>
        public static ITestFixture ShouldThrowArgumentNullException(this ITestFixture fixture, string paramName) =>
            fixture.ShouldThrow<ArgumentNullException>(e => e.ParamName.Should().Be(paramName));

        /// <summary>
        /// Adds an assertion to the specified fixture that an argument exception will be thrown with the specified parameter name.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns></returns>
        public static ITestFixture ShouldThrowArgumentException(this ITestFixture fixture, string paramName) =>
            fixture.ShouldThrow<ArgumentException>(e => e.ParamName.Should().Be(paramName));

    }
}
