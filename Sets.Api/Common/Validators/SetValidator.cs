using FluentValidation;
using Sets.Api.Models.Requests;

namespace Sets.Api.Common.Validators;

public class SetValidator : AbstractValidator<SetRequest>
{
    public SetValidator()
    {
        RuleFor(request => request.Name)
            .MinimumLength(3)
            .MaximumLength(15);
        RuleFor(request => request.Description)
            .MaximumLength(90);
        RuleForEach(request => request.KanjiList)
            .SetValidator(new KanjiValidator());
    }
}