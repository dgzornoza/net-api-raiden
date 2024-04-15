using FluentValidation;

namespace $safeprojectname$.Commands.Audits;

public class CreateAuditCommandValidator : AbstractValidator<CreateAuditCommand>
{
    public CreateAuditCommandValidator()
    {
        RuleForEach(item => item.Audits).SetValidator(new CreateAuditDtoValidator());
    }
}
