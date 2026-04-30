using CourseService.Application.DTOs.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Validators
{
	public class RejectCourseDtoValidator : AbstractValidator<RejectCourseDto>
	{
		public RejectCourseDtoValidator()
		{
			RuleFor(x => x.Reason)
				.NotEmpty().WithMessage("Rejection reason is required.")
				.MinimumLength(10).WithMessage("Please provide a meaningful rejection reason.")
				.MaximumLength(1000).WithMessage("Rejection reason must be less than 1000 characters.");
		}
	}
}
