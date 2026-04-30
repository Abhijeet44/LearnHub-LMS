using CourseService.Application.DTOs.Request;
using CourseService.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Interfaces
{
	public interface ICourseAppService
	{
		Task<CourseDetailDto> CreateAsync(CreateCourseDto request, Guid instructorId, string instructorName, CancellationToken ct = default);
		Task<CourseDetailDto> UpdateAsync(Guid courseId, UpdateCourseDto request, Guid instructorId, CancellationToken ct = default);
		Task<string> UploadThumbnailAsync(Guid courseId, Stream imageStream, string fileName, string contentType, Guid instructorId, CancellationToken ct = default);
		Task SubmitForReviewAsync(Guid courseId, Guid instructorId, CancellationToken ct = default);
		Task DeleteAsync(Guid courseId, Guid instructorId, CancellationToken ct = default);

		// Admin actions
		Task PublishAsync(Guid courseId, CancellationToken ct = default);
		Task RejectAsync(Guid courseId, RejectCourseDto request, CancellationToken ct = default);
		Task<IEnumerable<CourseListItemDto>> GetCoursesUnderReviewAsync(CancellationToken ct = default);

		// public actions
		Task<PagedResultDto<CourseListItemDto>> GetPublishedCoursesAsync(CourseQueryDto query, CancellationToken ct = default);
		Task<CourseDetailDto> GetByIdAsync(Guid courseId, bool isEnrolled = false, CancellationToken ct = default);

	}
}
