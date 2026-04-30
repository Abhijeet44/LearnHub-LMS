using CourseService.Application.DTOs.Request;
using CourseService.Domain.Enum;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Validators
{
	public class UpdateCourseDtoValidator : AbstractValidator<UpdateCourseDto>
	{
		public UpdateCourseDtoValidator()
		{
			RuleFor(x => x.Title)
			.NotEmpty()
			.MinimumLength(5)
			.MaximumLength(300);

			RuleFor(x => x.Description)
				.NotEmpty()
				.MinimumLength(50);

			RuleFor(x => x.ShortSummary)
				.NotEmpty()
				.MaximumLength(500);

			RuleFor(x => x.Price)
				.GreaterThanOrEqualTo(0);

			RuleFor(x => x.Level)
				.Must(level => Enum.TryParse<CourseLevel>(level, ignoreCase: true, out _))
				.WithMessage("Level must be Beginner, Intermediate, or Advanced.");

			RuleFor(x => x.Language)
				.NotEmpty()
				.MaximumLength(100);
		}
	}
}
