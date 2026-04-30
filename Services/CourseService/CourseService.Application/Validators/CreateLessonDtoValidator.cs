using CourseService.Application.DTOs.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Validators
{
	public class CreateLessonDtoValidator : AbstractValidator<CreateLessonDto>
	{
		public CreateLessonDtoValidator()
		{
			RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Lesson title is required.")
				.MaximumLength(300).WithMessage("Lesson title cannot exceed 300 characters.");
		}
	}
}
