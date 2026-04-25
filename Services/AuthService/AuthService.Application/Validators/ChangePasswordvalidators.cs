using AuthService.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Validators
{
	public class ChangePasswordValidators : AbstractValidator<ChangePasswordDto>
	{
		public ChangePasswordValidators()
		{
			RuleFor(x => x.CurrentPassword)
				.NotEmpty().WithMessage("Current password is required.");

			RuleFor(x => x.NewPassword)
				.NotEmpty()
				.MinimumLength(8)
				.Matches(@"[A-Z]")
				.Matches(@"[0-9]")
				.Matches(@"[^a-zA-Z0-9]");

			RuleFor(x => x.NewPassword)
				.NotEqual(x => x.CurrentPassword)
				.WithMessage("New password must be different from the current password.");
		}
	}
}
