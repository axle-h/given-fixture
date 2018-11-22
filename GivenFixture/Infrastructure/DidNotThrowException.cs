using System;

namespace GivenFixture.Infrastructure
{
    internal class DidNotThrowException : Exception
    {
        public DidNotThrowException(object result) : base($"Expected to throw but did not. Subject returned {result ?? "<null>"}")
        {
        }
    }
}