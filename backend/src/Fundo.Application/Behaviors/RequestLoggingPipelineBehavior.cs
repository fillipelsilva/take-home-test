using Fundo.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Threading;
using System.Threading.Tasks;

namespace Fundo.Application.Behaviors
{
    internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
    {
        private readonly ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> _logger;

        public RequestLoggingPipelineBehavior(
            ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            string requestName = typeof(TRequest).Name;

            using (LogContext.PushProperty("RequestName", requestName))
            {
                _logger.LogInformation("Processing request {RequestName}", requestName);

                TResponse result = await next();

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Completed request {RequestName} successfully", requestName);
                }
                else
                {
                    using (LogContext.PushProperty("Error", result.Error, true))
                    {
                        _logger.LogWarning("Completed request {RequestName} with error", requestName);
                    }
                }

                return result;
            }
        }
    }
}
