using Auth.Api.Models.Requests;
using FluentValidation;

namespace Auth.Api.Validators;

public class ResetPasswordDataValidator : AbstractValidator<ResetPasswordConfirmationRequest>
{
    public ResetPasswordDataValidator()
    {
        RuleFor(x => x.Password)
            .MinimumLength(8)
            .Matches(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$")
            .WithMessage("Password must contain one upper char and one digit and be at least 8 characters long");
    }
}