using FluentValidation;

namespace NetApiRaiden1.Application.Commands.Audits;

public class CreateAuditCommandValidator : AbstractValidator<CreateAuditCommand>
{
    public CreateAuditCommandValidator()
    {
        RuleForEach(item => item.Audits).SetValidator(new CreateAuditDtoValidator());
    }
}
