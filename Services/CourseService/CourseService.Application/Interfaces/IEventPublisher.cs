using CourseService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Interfaces
{
	public interface IEventPublisher
	{
		Task PublishCoursePublishedAsync(Course course, CancellationToken ct = default);
	}
}