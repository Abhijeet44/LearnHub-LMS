using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Domain.Exceptions
{
	public class SectionNotFoundException : DomainException
	{
		public SectionNotFoundException(Guid sectionId)
		: base($"Section with ID '{sectionId}' was not found.") { }
	}
}
