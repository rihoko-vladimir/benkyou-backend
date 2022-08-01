using Auth.Api.Models.Requests;
using FluentValidation;

namespace Auth.Api.Validators;

public class RegistrationDataValidator : AbstractValidator<RegistrationRequest>
{
    public RegistrationDataValidator()
    {
        RuleFor(x => x.Email)
            .Matches(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
        RuleFor(x => x.FirstName)
            .MaximumLength(20)
            .NotEmpty();
        RuleFor(x => x.LastName)
            .MaximumLength(35)
            .NotEmpty();
        RuleFor(x => x.UserName)
            .MaximumLength(10)
            .MinimumLength(4);
        RuleFor(x => x.Password)
            .MinimumLength(8)
            .Matches(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$")
            .WithMessage("Password must contain one upper char and one digit and be at least 8 characters long");
        RuleFor(x => x.IsTermsAccepted)
            .Equal(true);
    }
}