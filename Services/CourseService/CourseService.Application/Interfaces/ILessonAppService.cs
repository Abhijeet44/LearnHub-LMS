using CourseService.Application.DTOs.Request;
using CourseService.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Interfaces
{
	public interface ILessonAppService
	{
		Task<LessonDto> CreateAsync(Guid sectionId, CreateLessonDto request, Guid instructorId, CancellationToken ct = default);
		Task<LessonDto> UpdateAsync(Guid lessonId, UpdateLessonDto request, Guid instructorId, CancellationToken ct = default);
		Task<string> UploadVideoAsync(Guid lessonId, Stream videoStream, string fileName, string contentType, int durationSeconds, Guid instructorId, CancellationToken ct = default);
		Task DeleteAsync(Guid lessonId, Guid instructorId, CancellationToken ct = default);
		Task<IEnumerable<LessonDto>> GetBySectionIdAsync(Guid sectionId, CancellationToken ct = default);
	}
}
