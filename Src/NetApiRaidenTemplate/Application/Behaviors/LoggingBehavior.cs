using MediatR;
using Microsoft.Extensions.Logging;
using $safeprojectname$.Infrastructure.Extensions;

namespace $safeprojectname$.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => this.logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("----- Handling command {CommandName} ({Command})", request.GetGenericTypeName(), request);
        var response = await next();

        // IQueryable usado en ODATA no se logea la respuesta por que se realiza la consulta entera en BBDD
        var responseLog = (response != null && response.GetType().GetInterfaces().Any(item => item == typeof(IQueryable)) ?
            (object)((IQueryable)response).Expression.ToString() :
            response);
        logger.LogInformation("----- Command {CommandName} handled - response: {Response}", request.GetGenericTypeName(), responseLog);

        return response;
    }
}
