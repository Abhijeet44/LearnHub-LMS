using AuthService.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Validators
{
	public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
	{
		public RegisterRequestValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("A valid email address is required.")
				.MaximumLength(255);

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required.")
				.MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
				.Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
				.Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
				.Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

			RuleFor(x => x.FullName)
				.NotEmpty().WithMessage("Full name is required.")
				.MinimumLength(2)
				.MaximumLength(200);

			RuleFor(x => x.Role)
				.Must(role => role == "Student" || role == "Instrustor").
				WithMessage("Role must be either 'Student' or 'Instaructor'.");
		}
	}
}
