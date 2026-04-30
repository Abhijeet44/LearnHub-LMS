using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Domain.Exceptions
{
	public class CourseNotFoundException : DomainException
	{
		public CourseNotFoundException(Guid courseId) : base($"Course with ID '{courseId}' was not found.")
		{

		}
	}
}
