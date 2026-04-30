using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Domain.Exceptions
{
	public class LessonNotFoundException : DomainException
	{
		public LessonNotFoundException(Guid lessonId)
		: base($"Lesson with ID '{lessonId}' was not found.") { }
	}
}
