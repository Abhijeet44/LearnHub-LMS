using CourseService.Application.DTOs.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Validators
{
	public class UpdateLessonDtoValidator : AbstractValidator<UpdateLessonDto>
	{
		public UpdateLessonDtoValidator()
		{
			RuleFor(x => x.Title)
				.NotEmpty()
				.MaximumLength(300);

			RuleFor(x => x.OrderIndex)
				.GreaterThan(0);
		}
	}
}
