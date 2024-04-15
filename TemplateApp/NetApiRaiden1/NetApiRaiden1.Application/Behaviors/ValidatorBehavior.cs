using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using NetApiRaiden1.Application.Infrastructure.Extensions;

namespace NetApiRaiden1.Application.Behaviors;

public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> logger;
    private readonly IEnumerable<IValidator<TRequest>> validators;

    public ValidatorBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidatorBehavior<TRequest, TResponse>> logger)
    {
        this.validators = validators;
        this.logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var failures = validators
            .Select(item => item.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(error => error != null)
            .ToList();

        if (failures.Count != 0)
        {
            var typeName = request.GetGenericTypeName();
            logger.LogWarning("Validation errors - {CommandType} - Command: {Command} - Errors: {ValidationErrors}", typeName, request, failures);

            throw new ValidationException("Validation exception", failures);
        }

        return await next();
    }
}
