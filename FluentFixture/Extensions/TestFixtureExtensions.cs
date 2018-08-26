using FluentAssertions;

namespace FluentFixture.Extensions
{
    public static class TestFixtureExtensions
    {
        /// <summary>
        /// Sets up the Microsoft logging framework.
        /// Will log all messages to the console.
        /// </summary>
        /// <typeparam name="TSubject">The type of the subject.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static TestFixture<TSubject> HavingLogging<TSubject>(this TestFixture<TSubject> fixture)
        {
            fixture.AutoMock.WithLogging<TSubject>();
            return fixture;
        }

        /// <summary>
        /// Sets up options for the specified options type.
        /// </summary>
        /// <typeparam name="TOptions">The type of the options.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="options">The options.</param>
        /// <param name="verifiable">if set to <c>true</c> [verifiable].</param>
        /// <returns></returns>
        public static ITestFixture HavingOptions<TOptions>(this ITestFixture fixture, TOptions options, bool verifiable = true)
            where TOptions : class, new()
        {
            fixture.AutoMock.WithOptions(options, verifiable);
            return fixture;
        }

        /// <summary>
        /// Configures the memory cache mock to hit the cache for the specified key, returning the specified cached value.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <typeparam name="TCached">The type of the cached.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="keyRegex">The key regex.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static TFixture HavingMemoryCacheHit<TFixture, TCached>(this TFixture fixture, string keyRegex, TCached value)
            where TFixture : ITestFixture
            where TCached : class
        {
            fixture.AutoMock.WithMemoryCacheHit(keyRegex, value);
            return fixture;
        }

        /// <summary>
        /// Configures the memory cache mock to miss the cache for the specified key and to create a new entry.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="keyRegex">The key regex.</param>
        /// <returns></returns>
        public static TFixture HavingMemoryCacheMiss<TFixture>(this TFixture fixture, string keyRegex)
            where TFixture : ITestFixture
        {
            fixture.AutoMock.WithMemoryCacheMiss(keyRegex);
            return fixture;
        }

        /// <summary>
        /// Asserts that the fixture action should return the specified result.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static TFixture ShouldReturnResult<TFixture, TResult>(this TFixture fixture, TResult result)
            where TFixture : ITestFixture =>
            (TFixture) fixture.Assert<TResult>(x => x.Should().BeSameAs(result));

        /// <summary>
        /// Asserts that the fixture action should return null.
        /// </summary>
        /// <typeparam name="TFixture">The type of the fixture.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static TFixture ShouldReturnNull<TFixture>(this TFixture fixture)
            where TFixture : ITestFixture =>
            (TFixture) fixture.Assert(x => x.Should().BeNull());

    }
}
