using System;

namespace Upscript.Employee.API.Infrastructure.Exceptions
{
    /// <summary>
    /// Exception type for app exceptions
    /// </summary>
    public class EmployeeDomainException : Exception
    {
        public EmployeeDomainException()
        { }

        public EmployeeDomainException(string message)
            : base(message)
        { }

        public EmployeeDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
