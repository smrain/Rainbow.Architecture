using System;

namespace Rainbow.Architecture.Domain.Exceptions
{
    /// <summary>
    /// Exception type for domain exceptions
    /// </summary>
    public class AppDomainException : Exception
    {
        public AppDomainException()
        { }

        public AppDomainException(string message)
            : base(message)
        { }

        public AppDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
