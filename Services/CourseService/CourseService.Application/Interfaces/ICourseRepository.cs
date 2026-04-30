using CourseService.Application.DTOs.Request;
using CourseService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Interfaces
{
	public interface ICourseRepository
	{
		Task<Course> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task<Course?> GetByIdNoTrackingAsync(Guid id, CancellationToken ct = default);
		Task<(IEnumerable<Course> Courses, int TotalCount)> GetPublishedAsync(CourseQueryDto query, CancellationToken ct = default);
		Task<IEnumerable<Course>> GetByInstructorIdAsync(Guid instructorId, CancellationToken ct = default);
		Task<IEnumerable<Course>> GetUnderReviewAsync(CancellationToken ct = default);
		Task AddAsync(Course course, CancellationToken ct = default);
		Task SaveChangesAsync(CancellationToken ct = default);
		Task<bool> ExistsAndBelongsToInstructorAsync(Guid courseId, Guid instructorId, CancellationToken ct = default);

	}
}
