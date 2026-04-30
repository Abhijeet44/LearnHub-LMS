using CourseService.Application.DTOs.Request;
using CourseService.Application.DTOs.Response;
using CourseService.Application.Interfaces;
using CourseService.Application.Mappings;
using CourseService.Domain.Entities;
using CourseService.Domain.Enum;
using CourseService.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Services
{
	public class CourseAppService : ICourseAppService
	{
		private readonly ICourseRepository _courseRepository;
		private readonly IBlobStorageService _blobStorageService;
		private readonly IEventPublisher _eventPublisher;

		private const string ThumbnailContainerName = "thumbnails";

		public CourseAppService(ICourseRepository courseRepository, IBlobStorageService blobStorageService, IEventPublisher eventPublisher)
		{
			_courseRepository = courseRepository;
			_blobStorageService = blobStorageService;
			_eventPublisher = eventPublisher;
		}

		public async Task<CourseDetailDto> CreateAsync(CreateCourseDto request, Guid instructorId, string instructorName, CancellationToken ct = default)
		{
			Enum.TryParse<CourseLevel>(request.Level, ignoreCase: true, out var level);

			var course = Course.Create(
				title: request.Title, 
				description: request.Description, 
				shortSummary: request.ShortSummary, 
				price: request.Price, 
				level: level,
				language: request.Language, 
				instructorId: instructorId, 
				instructorName: instructorName
			);

			await _courseRepository.AddAsync(course, ct);
			await _courseRepository.SaveChangesAsync(ct);

			return CourseMapper.ToDetailDto(course);
		}

		public async Task<CourseDetailDto> UpdateAsync(Guid courseId, UpdateCourseDto request, Guid instructorId, CancellationToken ct = default)
		{
			var course = await _courseRepository.GetByIdAsync(courseId, ct) ?? throw new CourseNotFoundException(courseId);

			if(course.InstructorId != instructorId)
			{
				throw new UnauthorizedAccessException("You are not authorized to update this course.");
			}

			Enum.TryParse<CourseLevel>(request.Level, ignoreCase: true, out var level);

			course.UpdateDetails(
				title: request.Title,
				description: request.Description,
				shortSummary: request.ShortSummary,
				price: request.Price,
				level: level,
				language: request.Language
			);

			await _courseRepository.SaveChangesAsync(ct);

			return CourseMapper.ToDetailDto(course);
		}

		public async Task<string> UploadThumbnailAsync(Guid courseId, Stream imageStream, string fileName, string contentType, Guid instructorId, CancellationToken ct = default)
		{
			var course = await _courseRepository.GetByIdAsync(courseId, ct) 
				?? throw new CourseNotFoundException(courseId);

			if (course.InstructorId != instructorId)
				throw new UnauthorizedAccessException("You are not authorized to update this course.");

			if (!string.IsNullOrEmpty(course.ThumbnailUrl))
			{
				await _blobStorageService.DeleteAsync(course.ThumbnailUrl, ThumbnailContainerName, ct);
			}

			var extension = Path.GetExtension(fileName);
			var newFileName = $"course-{courseId}-thumb-{Guid.NewGuid()}{extension}";

			var blobUrl = await _blobStorageService.UploadAsync(imageStream, newFileName, contentType, ThumbnailContainerName, ct);

			course.SetThumbnail(blobUrl);
			await _courseRepository.SaveChangesAsync(ct);

			return blobUrl;
		}

		public async Task DeleteAsync(Guid courseId, Guid instructorId, CancellationToken ct = default)
		{
			var course = _courseRepository.GetByIdAsync(courseId, ct).Result 
				?? throw new CourseNotFoundException(courseId);

			if (course.InstructorId != instructorId)
				throw new UnauthorizedAccessException("You are not authorized to delete this course.");

			if(course.Status == CourseStatus.Published || course.Status == CourseStatus.UnderReview)
			{
				throw new InvalidOperationException("Cannot delete a course that is published or under review. Please contact support.");
			}

			course.SoftDelete();
			await _courseRepository.SaveChangesAsync(ct);
		}

		public async Task<CourseDetailDto> GetByIdAsync(Guid courseId, bool isEnrolled = false, CancellationToken ct = default)
		{
			var course = await _courseRepository.GetByIdAsync(courseId, ct) 
				?? throw new CourseNotFoundException(courseId);

			var dto = CourseMapper.ToDetailDto(course);

			if (!isEnrolled)
			{
				foreach (var section in dto.Sections)
				{
					foreach (var lesson in section.Lessons)
					{
						if (!lesson.IsPreview)
						{
							lesson.VideoUrl = null;
						}
					}
				}
			}
			return dto;
		}

		public async Task<IEnumerable<CourseListItemDto>> GetCoursesUnderReviewAsync(CancellationToken ct = default)
		{
			var course = await _courseRepository.GetUnderReviewAsync(ct);
			return course.Select(CourseMapper.ToListItemDto);
		}

		public async Task<PagedResultDto<CourseListItemDto>> GetPublishedCoursesAsync(CourseQueryDto query, CancellationToken ct = default)
		{
			var (courses, totalCount) = await _courseRepository.GetPublishedAsync(query, ct);
			
			return new PagedResultDto<CourseListItemDto>
			{
				Items = courses.Select(CourseMapper.ToListItemDto),
				TotalCount = totalCount,
				Page = query.Page,
				PageSize = query.PageSize
			};
		}

		public async Task PublishAsync(Guid courseId, CancellationToken ct = default)
		{
			var course = await _courseRepository.GetByIdAsync(courseId, ct)
				?? throw new CourseNotFoundException(courseId);

			course.Publish();
			await _courseRepository.SaveChangesAsync(ct);
			await _eventPublisher.PublishCoursePublishedAsync(course, ct);

		}

		public async Task RejectAsync(Guid courseId, RejectCourseDto request, CancellationToken ct = default)
		{
			var course = await _courseRepository.GetByIdAsync(courseId, ct)
				?? throw new CourseNotFoundException(courseId);

			course.Reject(request.Reason);
			await _courseRepository.SaveChangesAsync(ct);
		}

		public async Task SubmitForReviewAsync(Guid courseId, Guid instructorId, CancellationToken ct = default)
		{
			var course = _courseRepository.GetByIdAsync(courseId, ct).Result 
				?? throw new CourseNotFoundException(courseId);

			if (course.InstructorId != instructorId)
				throw new UnauthorizedAccessException("You are not authorized to update this course.");

			course.SubmitForReview();
			await _courseRepository.SaveChangesAsync(ct);
		}
	}
}
