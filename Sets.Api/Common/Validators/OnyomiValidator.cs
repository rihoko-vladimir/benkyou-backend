using FluentValidation;
using Sets.Api.Models.Requests;

namespace Sets.Api.Common.Validators;

public class OnyomiValidator : AbstractValidator<OnyomiRequest>
{
    public OnyomiValidator()
    {
        RuleFor(request => request.Reading)
            .MaximumLength(10)
            .Matches(@"[ァ-ン]+");
    }
}