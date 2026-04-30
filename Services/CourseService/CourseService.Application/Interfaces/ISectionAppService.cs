using CourseService.Application.DTOs.Request;
using CourseService.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Interfaces
{
	public interface ISectionAppService
	{
		Task<SectionDto> CreateAsync(Guid courseId, CreateSectionDto request, Guid instructorId, CancellationToken ct = default);
		Task<SectionDto> UpdateAsync(Guid sectionId, UpdateSectionDto request, Guid instructorId, CancellationToken ct = default);
		Task DeleteAsync(Guid sectionId, Guid instructorId, CancellationToken ct = default);
		Task<IEnumerable<SectionDto>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default);
	}
}
