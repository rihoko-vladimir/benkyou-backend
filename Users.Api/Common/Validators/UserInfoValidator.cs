using FluentValidation;
using Users.Api.Models.Requests;

namespace Users.Api.Common.Validators;

public class UserInfoValidator : AbstractValidator<UpdateUserInfoRequest>
{
    public UserInfoValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(20);
        RuleFor(x => x.LastName)
            .MaximumLength(35);
        RuleFor(x => x.UserName)
            .MaximumLength(10);
        RuleFor(x => x.About).Custom((s, context) =>
        {
            if (s is { Length: > 350 }) context.AddFailure("About message must not be longer than 350 chars");
        });
        RuleFor(x => x.BirthDay).Custom((time, context) =>
        {
            if (time is { Year: < 1900 }) context.AddFailure("Incorrect birthday provided");
        });
    }
}