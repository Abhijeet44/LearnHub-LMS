using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.DTOs.Response
{
	public class LessonDto
	{
		public Guid Id { get; set; }
		public Guid SectionId { get; set; }
		public string Title { get; set; } = string.Empty;

		public string? VideoUrl { get; set; }

		public int DurationSeconds { get; set; }

		public string Duration => FormatDuration(DurationSeconds);

		public int OrderIndex { get; set; }
		public bool IsPreview { get; set; }

		private static string FormatDuration(int seconds)
		{
			var ts = TimeSpan.FromSeconds(seconds);
			return ts.Hours > 0
				? $"{ts.Hours}:{ts.Minutes:D2}:{ts.Seconds:D2}"
				: $"{ts.Minutes}:{ts.Seconds:D2}";
		}
	}
}
