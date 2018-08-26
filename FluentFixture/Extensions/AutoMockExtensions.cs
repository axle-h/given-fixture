using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Autofac.Extras.Moq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FluentFixture.Extensions
{
    public static class AutoMockExtensions
    {
        /// <summary>
        /// Sets up the Microsoft logging framework in the specified auto mock container.
        /// Will log all messages to the console.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="mock">The mock.</param>
        /// <returns></returns>
        public static AutoMock WithLogging<TService>(this AutoMock mock)
        {
            var logger = new UnitTestLogger<TService>();
            mock.Provide<ILogger>(logger);
            mock.Provide<ILogger<TService>>(logger);
            var factory = mock.Mock<ILoggerFactory>();
            factory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger);
            factory.Setup(x => x.Dispose());

            return mock;
        }

        /// <summary>
        /// Sets up <see cref="IOptions{TOptions}" /> for the specified options type in the specified auto mock container.
        /// </summary>
        /// <typeparam name="TOptions">The type of the options.</typeparam>
        /// <param name="mock">The mock.</param>
        /// <param name="options">The options.</param>
        /// <param name="verifiable">if set to <c>true</c> [verifiable].</param>
        public static void WithOptions<TOptions>(this AutoMock mock, TOptions options, bool verifiable = true)
            where TOptions : class, new()
        {
            var setup = mock.Mock<IOptions<TOptions>>().Setup(x => x.Value).Returns(options);
            if (verifiable)
            {
                setup.Verifiable();
            }
        }

        /// <summary>
        /// Configures the <see cref="IMemoryCache"/> mock to hit the cache for the specified key, returning the specified cached value.
        /// </summary>
        /// <typeparam name="TCached">The type of the cached.</typeparam>
        /// <param name="mock">The mock.</param>
        /// <param name="keyRegex">The key regex.</param>
        /// <param name="value">The value.</param>
        public static void WithMemoryCacheHit<TCached>(this AutoMock mock, string keyRegex, TCached value)
            where TCached : class
        {
            mock.Mock<IMemoryCache>().Setup(x => x.Dispose());

            // ReSharper disable once RedundantAssignment
            object o = value;
            mock.Mock<IMemoryCache>().Setup(x => x.TryGetValue(It.Is<string>(s => Regex.IsMatch(s, keyRegex)), out o)).Returns(true).Verifiable();
        }

        /// <summary>
        /// Configures the <see cref="IMemoryCache"/> mock to miss the cache for the specified key.
        /// </summary>
        /// <typeparam name="TCached">The type of the cached.</typeparam>
        /// <param name="mock">The mock.</param>
        /// <param name="keyRegex">The key regex.</param>
        /// <param name="cached">The cached.</param>
        public static void WithMemoryCacheMiss<TCached>(this AutoMock mock, string keyRegex, TCached cached)
            where TCached : class
        {
            mock.Mock<IMemoryCache>().Setup(x => x.Dispose());

            object o;
            mock.Mock<IMemoryCache>().Setup(x => x.TryGetValue(It.Is<string>(s => Regex.IsMatch(s, keyRegex)), out o)).Returns(false).Verifiable();

            // New mock, not pulled from the auto mock as we don't want a singleton cache entry.
            var entry = Mock.Of<ICacheEntry>();
            mock.Mock<IMemoryCache>().Setup(x => x.CreateEntry(It.Is<string>(s => Regex.IsMatch(s, keyRegex)))).Returns(entry).Verifiable();

            if (cached == null)
            {
                Mock.Get(entry).SetupSet<ICacheEntry>(c => c.Value = It.IsAny<TCached>()).Verifiable();
            }
            else
            {
                Mock.Get(entry).SetupSet<ICacheEntry>(c => c.Value = cached).Verifiable();
            }

            Mock.Get(entry).Setup(x => x.Dispose()).Verifiable();
        }

        /// <summary>
        /// Configures the <see cref="IMemoryCache"/> mock to miss the cache for the specified key.
        /// </summary>
        /// <param name="mock">The mock.</param>
        /// <param name="keyRegex">The key regex.</param>
        public static void WithMemoryCacheMiss(this AutoMock mock, string keyRegex)
        {
            mock.Mock<IMemoryCache>().Setup(x => x.Dispose());

            object o;
            mock.Mock<IMemoryCache>().Setup(x => x.TryGetValue(It.Is<string>(s => Regex.IsMatch(s, keyRegex)), out o)).Returns(false).Verifiable();

            // New mock, not pulled from the auto mock as we don't want a singleton cache entry.
            var entry = Mock.Of<ICacheEntry>();
            mock.Mock<IMemoryCache>().Setup(x => x.CreateEntry(It.Is<string>(s => Regex.IsMatch(s, keyRegex)))).Returns(entry).Verifiable();

            Mock.Get(entry).SetupSet<ICacheEntry>(c => c.Value = It.IsAny<object>()).Verifiable();
            Mock.Get(entry).Setup(x => x.Dispose()).Verifiable();
        }

        private class UnitTestLogger<TService> : ILogger<TService>, IDisposable
        {
            private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
                => Console.WriteLine($"[{_stopwatch.Elapsed}] [{logLevel}] {formatter(state, exception)}");

            public bool IsEnabled(LogLevel logLevel) => true;

            public IDisposable BeginScope<TState>(TState state) => new Scope();

            private class Scope : IDisposable
            {
                public void Dispose()
                {
                }
            }

            public void Dispose() => _stopwatch.Stop();
        }
    }
}
