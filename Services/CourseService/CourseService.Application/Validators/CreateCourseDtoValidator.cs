using CourseService.Application.DTOs.Request;
using CourseService.Domain.Enum;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Validators
{
	public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
	{
		public CreateCourseDtoValidator()
		{
			RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Title is required.")
				.MaximumLength(300).WithMessage("Title cannot exceed 300 characters.")
				.MinimumLength(5).WithMessage("Title must be at least 5 characters.");

			RuleFor(x => x.Description)
				.NotEmpty().WithMessage("Description is required.")
				.MinimumLength(50).WithMessage("Description must be at least 50 characters.");

			RuleFor(x => x.ShortSummary)
				.NotEmpty().WithMessage("Short summary is required.")
				.MaximumLength(500).WithMessage("Short summary cannot exceed 500 characters.");

			RuleFor(x => x.Price)
				.GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

			RuleFor(x => x.Level)
				.Must(level => Enum.TryParse<CourseLevel>(level, ignoreCase: true, out _))
				.WithMessage("Level must be Beginner, Intermediate, or Advanced.");

			RuleFor(x => x.Language)
				.NotEmpty().WithMessage("Language is required.")
				.MaximumLength(100);

		}
	}
}
