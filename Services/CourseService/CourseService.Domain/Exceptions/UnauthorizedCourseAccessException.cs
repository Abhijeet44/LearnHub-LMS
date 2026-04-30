using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Domain.Exceptions
{
	public class UnauthorizedCourseAccessException : DomainException
	{
		public UnauthorizedCourseAccessException()
		: base("You do not have permission to modify this course.") { }
	}
}
