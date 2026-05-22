using FluentValidation;

namespace DungeonApi.Authentication
{
    public class RegisterValidator : AbstractValidator<AddOrUpdateAppUserModel>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("User name is required")
                .MinimumLength(3).WithMessage("User name must be at least 3 characters")
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain an uppercase letter")
                .Matches(@"[0-9]").WithMessage("Password must contain a digit")
                .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain a special character (e.g. ! or @)");
        }
    }
}
