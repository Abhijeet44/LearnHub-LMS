using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.DTOs.Response
{
	public class CourseListItemDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string ShortSummary { get; set; } = string.Empty;
		public string? ThumbnailUrl { get; set; }
		public string InstructorName { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public bool IsFree { get; set; }
		public string Level { get; set; } = string.Empty;
		public string Language { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;

		// Total duration formatted for display on the card: "4h 32m"
		public string TotalDuration => FormatTotalDuration(TotalDurationSeconds);
		public int TotalDurationSeconds { get; set; }

		public int SectionCount { get; set; }
		public int LessonCount { get; set; }

		public DateTime CreatedAt { get; set; }

		private static string FormatTotalDuration(int seconds)
		{
			var ts = TimeSpan.FromSeconds(seconds);
			if (ts.Hours > 0)
				return $"{ts.Hours}h {ts.Minutes}m";
			return $"{ts.Minutes}m";
		}
	}
}
