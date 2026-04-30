using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.DTOs.Response
{
	public class CourseDetailDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string ShortSummary { get; set; } = string.Empty;
		public string? ThumbnailUrl { get; set; }
		public Guid InstructorId { get; set; }
		public string InstructorName { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public bool IsFree { get; set; }
		public string Level { get; set; } = string.Empty;
		public string Language { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;

		// Only populated when Admin rejects — null otherwise
		public string? RejectionReason { get; set; }

		public int TotalDurationSeconds { get; set; }
		public string TotalDuration => FormatTotalDuration(TotalDurationSeconds);

		// Sections ordered by OrderIndex, each with their lessons ordered by OrderIndex
		public IEnumerable<SectionDto> Sections { get; set; } = Enumerable.Empty<SectionDto>();

		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		private static string FormatTotalDuration(int seconds)
		{
			var ts = TimeSpan.FromSeconds(seconds);
			if (ts.Hours > 0)
				return $"{ts.Hours}h {ts.Minutes}m";
			return $"{ts.Minutes}m";
		}
	}
}
