using AuthService.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Validators
{
	public class LoginRequestValidators : AbstractValidator<LoginRequestDto>
	{
		public LoginRequestValidators()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress();

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required.");


		}
	}
}
