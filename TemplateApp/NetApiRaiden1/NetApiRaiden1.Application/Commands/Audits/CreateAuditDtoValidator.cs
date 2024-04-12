using FluentValidation;
using NetApiRaiden1.Application.Services.Audit;
using NetApiRaiden1.Infrastructure;

namespace NetApiRaiden1.Application.Commands.Audits;

public class CreateAuditDtoValidator : AbstractValidator<AuditDto>
{
    public CreateAuditDtoValidator()
    {
        RuleFor(obj => obj.UserId).NotEmpty();
        RuleFor(obj => obj.Operation).NotEmpty().MaximumLength(ColumnSizes.LongText);
        RuleFor(obj => obj.ClientIp).NotEmpty();
        RuleFor(obj => obj.AuditType).NotEmpty();
    }
}
