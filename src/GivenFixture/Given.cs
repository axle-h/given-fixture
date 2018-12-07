namespace GivenFixture
{
    /// <summary>
    /// Base of fluent test fixture conversation.
    /// </summary>
    public static class Given
    {
        /// <summary>
        /// Given a strict test fixture.
        /// </summary>
        public static ITestFixture Fixture => StrictFixture;

        /// <summary>
        /// Given a strict test fixture.
        /// </summary>
        public static ITestFixture StrictFixture => new TestFixture(true);

        /// <summary>
        /// Given a loose test fixture.
        /// </summary>
        public static ITestFixture LooseFixture => new TestFixture(false);
    }
}