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
        /// Given a strict test fixture that only verifies mocks that were configured as verifiable.
        /// </summary>
        public static ITestFixture StrictFixture => new TestFixture(true, false);

        /// <summary>
        /// Given a strict test fixture that verifies all mocks by default.
        /// </summary>
        public static ITestFixture StrictFullyVerifiedFixture => new TestFixture(true, true);

        /// <summary>
        /// Given a loose test fixture that only verifies mocks that were configured as verifiable.
        /// </summary>
        public static ITestFixture LooseFixture => new TestFixture(false, false);

        /// <summary>
        /// Given a loose test fixture that verifies all mocks by default.
        /// </summary>
        public static ITestFixture LooseFullyVerifiedFixture => new TestFixture(false, true);
    }
}