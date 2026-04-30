using CourseService.Application.DTOs.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Validators
{
	public class UpdateSectionDtoValidator : AbstractValidator<UpdateSectionDto>
	{
		public UpdateSectionDtoValidator()
		{
			RuleFor(x => x.Title)
				.NotEmpty()
				.MaximumLength(300);

			RuleFor(x => x.OrderIndex)
				.GreaterThan(0).WithMessage("OrderIndex must be greater than 0.");
		}
	}
}
