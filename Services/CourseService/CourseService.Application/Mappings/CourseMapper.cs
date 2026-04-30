using CourseService.Application.DTOs.Response;
using CourseService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Mappings
{
	public static class CourseMapper
	{
		public static CourseDetailDto ToDetailDto(Course course)
		{
			return new CourseDetailDto
			{
				Id = course.Id,
				Title = course.Title,
				Description = course.Description,
				ShortSummary = course.ShortSummary,
				ThumbnailUrl = course.ThumbnailUrl,
				InstructorId = course.InstructorId,
				InstructorName = course.InstructorName,
				Price = course.Price,
				IsFree = course.IsFree,
				Level = course.Level.ToString(),
				Language = course.Language,
				Status = course.Status.ToString(),
				RejectionReason = course.RejectionReason,
				TotalDurationSeconds = course.TotalDurationSeconds,
				Sections = course.Sections.OrderBy(s => s.OrderIndex).Select(ToSectionDto)
			};
		}

		public static CourseListItemDto ToListItemDto(Course course)
		{
			return new CourseListItemDto
			{
				Id = course.Id,
				Title = course.Title,
				ShortSummary = course.ShortSummary,
				ThumbnailUrl = course.ThumbnailUrl,
				InstructorName = course.InstructorName,
				Price = course.Price,
				IsFree = course.IsFree,
				Level = course.Level.ToString(),
				Language = course.Language,
				Status = course.Status.ToString(),
				TotalDurationSeconds = course.TotalDurationSeconds,
				CreatedAt = course.CreatedAt,

				// Count sections and lessons for display on the course card
				// e.g. "5 sections • 42 lessons"
				SectionCount = course.Sections.Count(s => !s.IsDeleted),
				LessonCount = course.Sections
				.Where(s => !s.IsDeleted)
				.SelectMany(s => s.Lessons)
				.Count(l => !l.IsDeleted)
			};
		}

		public static SectionDto ToSectionDto(Section section)
		{
			return new SectionDto
			{
				Id = section.Id,
				CourseId = section.CourseId,
				Title = section.Title,
				OrderIndex = section.OrderIndex,
				Lessons = section.Lessons.Where(i => !i.IsDeleted).OrderBy(i => i.OrderIndex).Select(ToLessonDto)
			};
		}

		public static LessonDto ToLessonDto(Lesson lesson)
		{
			return new LessonDto
			{
				Id = lesson.Id,
				SectionId = lesson.SectionId,
				Title = lesson.Title,
				VideoUrl = lesson.VideoUrl,
				DurationSeconds = lesson.DurationSeconds,
				OrderIndex = lesson.OrderIndex,
				IsPreview = lesson.IsPreview
			};
		}

	}
}
