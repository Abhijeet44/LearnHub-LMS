using CourseService.Application.DTOs.Request;
using CourseService.Application.DTOs.Response;
using CourseService.Application.Interfaces;
using CourseService.Application.Mappings;
using CourseService.Domain.Entities;
using CourseService.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Services
{
	public class LessonAppService : ILessonAppService
	{
		private readonly ILessonRepository _lessonRepository;
		private readonly ICourseRepository _courseRepository;
		private readonly ISectionRepository _sectionRepository;
		private readonly IBlobStorageService _blobStorageService;

		private const string VideoContainer = "videos";

		public LessonAppService(ILessonRepository lessonRepository, ICourseRepository courseRepository, 
			ISectionRepository sectionRepository, IBlobStorageService blobStorageService)
		{
			_lessonRepository = lessonRepository;
			_courseRepository = courseRepository;
			_sectionRepository = sectionRepository;
			_blobStorageService = blobStorageService;
		}

		public async Task<LessonDto> CreateAsync(Guid sectionId, CreateLessonDto request, Guid instructorId, CancellationToken ct = default)
		{
			// 1. Fetch section to get CourseId for auth check
			var section = await _sectionRepository.GetByIdAsync(sectionId, ct) ??
				throw new SectionNotFoundException(sectionId);

			// 2. Walk up to Course for ownership verification
			var course = await _courseRepository.GetByIdAsync(section.CourseId, ct) ??
				throw new CourseNotFoundException(sectionId);

			if (course.InstructorId != instructorId)
				throw new UnauthorizedAccessException();

			// 3. Determine order index for the new lesson within this section
			int orderIndex;
			if(request.OrderIndex.HasValue)
			{
				orderIndex = request.OrderIndex.Value;
			}
			else
			{
				var maxOrder = await _lessonRepository.GetMaxOrderIndexAsync(sectionId, ct);
				orderIndex = maxOrder + 1;
			}

			// 4. Create and persist the new lesson
			//    Instructor uploads video separately via UploadVideoAsync

			var lesson = Lesson.Create(sectionId, request.Title, orderIndex, request.IsPreview);

			await _lessonRepository.AddAsync(lesson, ct);
			await _lessonRepository.SaveChangesAsync(ct);

			return CourseMapper.ToLessonDto(lesson);
		}

		public async Task DeleteAsync(Guid lessonId, Guid instructorId, CancellationToken ct = default)
		{
			var lesson = await _lessonRepository.GetByIdAsync(lessonId, ct)
			?? throw new LessonNotFoundException(lessonId);

			var section = await _sectionRepository.GetByIdAsync(lesson.SectionId, ct)
				?? throw new SectionNotFoundException(lesson.SectionId);

			var course = await _courseRepository.GetByIdAsync(section.CourseId, ct)
				?? throw new CourseNotFoundException(section.CourseId);

			if (course.InstructorId != instructorId)
				throw new UnauthorizedCourseAccessException();

			lesson.SoftDelete();

			course.RecalculateDuration();

			await _lessonRepository.SaveChangesAsync(ct);
			await _courseRepository.SaveChangesAsync(ct);
		}

		public async Task<IEnumerable<LessonDto>> GetBySectionIdAsync(Guid sectionId, CancellationToken ct = default)
		{
			var lessons = await _lessonRepository.GetBySectionIdAsync(sectionId, ct);
			return lessons.OrderBy(l => l.OrderIndex).Select(CourseMapper.ToLessonDto);
		}

		public async Task<LessonDto> UpdateAsync(Guid lessonId, UpdateLessonDto request, Guid instructorId, CancellationToken ct = default)
		{
			var lesson = await _lessonRepository.GetByIdAsync(lessonId, ct) ??
				throw new LessonNotFoundException(lessonId);

			var section = await _sectionRepository.GetByIdAsync(lesson.SectionId, ct) ??
				throw new SectionNotFoundException(lesson.SectionId);

			var course = await _courseRepository.GetByIdAsync(section.CourseId, ct) ??
				throw new CourseNotFoundException(section.CourseId);

			if (course.InstructorId != instructorId)
				throw new UnauthorizedAccessException();

			lesson.UpdateDetails(request.Title, request.OrderIndex, request.IsPreview);
			await _lessonRepository.SaveChangesAsync(ct);

			return CourseMapper.ToLessonDto(lesson);
		}

		public async Task<string> UploadVideoAsync(Guid lessonId, Stream videoStream, string fileName, string contentType, int durationSeconds, Guid instructorId, CancellationToken ct = default)
		{
			var lesson = await _lessonRepository.GetByIdAsync(lessonId, ct) ??
				throw new LessonNotFoundException(lessonId);

			var section = await _sectionRepository.GetByIdAsync(lesson.SectionId, ct) ??
				throw new SectionNotFoundException(lesson.SectionId);

			var course = await _courseRepository.GetByIdAsync(section.CourseId, ct) ??
				throw new CourseNotFoundException(section.CourseId);

			if (course.InstructorId != instructorId)
				throw new UnauthorizedCourseAccessException();

			if (!string.IsNullOrEmpty(lesson.VideoUrl))
			{
				await _blobStorageService.DeleteAsync(lesson.VideoUrl, VideoContainer, ct);
			}

			var extension = Path.GetExtension(fileName);
			var uniqueFileName = $"course-{section.CourseId}-lesson-{lessonId}-{Guid.NewGuid()}{extension}";

			var blobUrl = await _blobStorageService.UploadAsync(videoStream, uniqueFileName, contentType, VideoContainer, ct);

			// Update lesson with video URL and duration via domain method
			lesson.SetVideo(blobUrl, durationSeconds);
			await _lessonRepository.SaveChangesAsync(ct);

			//    Recalculate course total duration
			//    The new lesson's duration must be reflected on the Course entity
			//    Load fresh course with all sections and lessons for accurate sum
			var freshCourse = await _courseRepository.GetByIdAsync(section.CourseId, ct);
			freshCourse?.RecalculateDuration();
			await _courseRepository.SaveChangesAsync(ct);

			return blobUrl;
		}
	}
}
