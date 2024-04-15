using FluentValidation;
using $safeprojectname$.Services.Audit;
using $ext_safeprojectname$.Infrastructure;

namespace $safeprojectname$.Commands.Audits;

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
