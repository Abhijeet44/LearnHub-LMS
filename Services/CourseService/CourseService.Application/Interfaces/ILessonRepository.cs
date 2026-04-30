using CourseService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Interfaces
{
	public interface ILessonRepository
	{
		Task<Lesson?> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task<IEnumerable<Lesson>> GetBySectionIdAsync(Guid sectionId, CancellationToken ct = default);
		Task<int> GetMaxOrderIndexAsync(Guid sectionId, CancellationToken ct = default);
		Task<int> GetTotalLessonCountByCourseIdAsync(Guid courseId, CancellationToken ct = default);
		Task AddAsync(Lesson lesson, CancellationToken ct = default);
		Task SaveChangesAsync(CancellationToken ct = default);
	}
}
