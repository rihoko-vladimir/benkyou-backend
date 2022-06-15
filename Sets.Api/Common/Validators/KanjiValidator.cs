using FluentValidation;
using Sets.Api.Models.Requests;

namespace Sets.Api.Common.Validators;

public class KanjiValidator : AbstractValidator<KanjiRequest>
{
    public KanjiValidator()
    {
        RuleFor(request => request.KanjiChar)
            .Length(1)
            .Matches(@"[一-龯]");
        RuleForEach(request => request.KunyomiReadings)
            .SetValidator(new KunyomiValidator());
        RuleForEach(request => request.OnyomiReadings)
            .SetValidator(new OnyomiValidator());
    }
}