using FluentValidation;
using OrderSystem.DTOs;

namespace OrderSystem.Validators
{
    public class AddEditClientValidator : AbstractValidator<AddEditDTO>
    {
        public AddEditClientValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(50);
            RuleFor(x => x.FixedPart).NotEmpty().Length(3);
        }
    }


    public class LoginValidator : AbstractValidator<LoginDTO>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().MinimumLength(15).MaximumLength(50).EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class RegisterValidator : AbstractValidator<RegisterDTO>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email).NotEmpty().MinimumLength(15).MaximumLength(30).EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(20).Must(ValidPassword);
            RuleFor(x => x.FirstName).NotEmpty().MinimumLength(3).MaximumLength(12).Must(ValidName);
            RuleFor(x => x.LastName).NotEmpty().MinimumLength(3).MaximumLength(12).Must(ValidName);
            RuleFor(x => x.UserName).NotEmpty().MinimumLength(6).MaximumLength(15).Must(ValidUserName);

        }

        private bool ValidPassword(String password)
        {
            return password.Any(char.IsUpper) && password.Any(char.IsLower) && password.All(char.IsLetterOrDigit) && password.Any(char.IsDigit);
        }
        private bool ValidName(String name)
        {
            return name.All(char.IsLetter);
        }
        private bool ValidUserName(String userName)
        {
            return userName.All(char.IsLetterOrDigit);
        }

    }


    public class EditInfoValidator : AbstractValidator<EditInfo>
    {
        public EditInfoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MinimumLength(3).MaximumLength(12).Must(ValidName);
            RuleFor(x => x.LastName).NotEmpty().MinimumLength(3).MaximumLength(12).Must(ValidName);
            RuleFor(x => x.UserName).NotEmpty().MinimumLength(6).MaximumLength(15).Must(ValidUserName);

        }
        private bool ValidName(String name)
        {
            return name.All(char.IsLetter);
        }
        private bool ValidUserName(String userName)
        {
            return userName.All(char.IsLetterOrDigit);
        }

    }


    public class EditPasswordValidator : AbstractValidator<EditPassword>
    {
        public EditPasswordValidator()
        {
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(20).Must(ValidPassword);

        }

        private bool ValidPassword(String password)
        {
            return password.Any(char.IsUpper) && password.Any(char.IsLower) && password.All(char.IsLetterOrDigit) && password.Any(char.IsDigit);
        }
    }

    public class AddOrderValidator : AbstractValidator<AddEDITOrderDTO>
    {
        public AddOrderValidator()
        {
            RuleFor(x => x.OrderName).NotEmpty().MinimumLength(6).MaximumLength(30);
            RuleFor(x => x.NumberOfCopies).NotEmpty().GreaterThan(0);


        }
        
    }
}
