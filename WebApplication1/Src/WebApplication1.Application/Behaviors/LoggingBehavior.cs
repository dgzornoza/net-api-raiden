using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using WebApplication1.Application.Common.Extensions;

namespace WebApplication1.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => this.logger = logger;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            this.logger.LogInformation("----- Handling command {CommandName} ({@Command})", request.GetGenericTypeName(), request);
            var response = await next();
            this.logger.LogInformation("----- Command {CommandName} handled - response: {@Response}", request.GetGenericTypeName(), response);

            return response;
        }
    }
}
