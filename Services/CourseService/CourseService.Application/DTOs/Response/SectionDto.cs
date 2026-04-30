using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.DTOs.Response
{
	public class SectionDto
	{
		public Guid Id { get; set; }
		public Guid CourseId { get; set; }
		public string Title { get; set; } = string.Empty;
		public int OrderIndex { get; set; }

		public IEnumerable<LessonDto> Lessons { get; set; } = Enumerable.Empty<LessonDto>();

		public int TotalDurationSeconds => Lessons.Sum(l => l.DurationSeconds);
	}
}
