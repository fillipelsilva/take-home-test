using Fundo.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fundo.Application.Exceptions
{
    public sealed class LoanManagementException : Exception
    {
        public LoanManagementException(string requestName, Error? error = null, Exception? innerException = null)
            : base("Application exception", innerException)
        {
            RequestName = requestName;
            Error = error;
        }

        public string RequestName { get; }

        public Error? Error { get; }
    }
}
