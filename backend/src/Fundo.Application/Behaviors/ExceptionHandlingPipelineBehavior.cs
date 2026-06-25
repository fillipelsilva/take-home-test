using Fundo.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fundo.Application.Exceptions;

namespace Fundo.Application.Behaviors
{
    internal sealed class ExceptionHandlingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    {
        private readonly ILogger<ExceptionHandlingPipelineBehavior<TRequest, TResponse>> _logger;

        public ExceptionHandlingPipelineBehavior(
            ILogger<ExceptionHandlingPipelineBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (LoanManagementException)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unhandled exception for {RequestName}", typeof(TRequest).Name);

                throw new LoanManagementException(typeof(TRequest).Name, innerException: exception);
            }
        }
    }
}
