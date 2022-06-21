using FluentValidation;
using Sets.Api.Models.Requests;

namespace Sets.Api.Common.Validators;

public class KunyomiValidator : AbstractValidator<KunyomiRequest>
{
    public KunyomiValidator()
    {
        RuleFor(request => request.Reading)
            .MaximumLength(10)
            .Matches(@"[ぁ-ん]");
    }
}