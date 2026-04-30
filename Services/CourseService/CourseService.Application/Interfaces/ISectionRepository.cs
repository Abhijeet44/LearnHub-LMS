using CourseService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Interfaces
{
	public interface ISectionRepository
	{
		Task<Section?> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task<IEnumerable<Section>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default);
		Task<int> GetMaxOrderIndexAsync(Guid courseId, CancellationToken ct = default);
		Task AddAsync(Section section, CancellationToken ct = default);
		Task SaveChangesAsync(CancellationToken ct = default);
	}
}
