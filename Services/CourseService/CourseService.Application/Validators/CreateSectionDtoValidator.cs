using CourseService.Application.DTOs.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Validators
{
	public class CreateSectionDtoValidator : AbstractValidator<CreateSectionDto>
	{
		public CreateSectionDtoValidator()
		{
			RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Section title is required.")
				.MaximumLength(300);

			RuleFor(x => x.OrderIndex)
				.GreaterThan(0).WithMessage("OrderIndex must be greater than 0.")
				.When(x => x.OrderIndex.HasValue);
		}
	}
}
