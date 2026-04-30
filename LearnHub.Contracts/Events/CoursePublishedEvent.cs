using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnHub.Contracts.Events
{
	public record CoursePublishedEvent
	{
		public Guid CourseId { get; init; }
		public string Title { get; init; } = string.Empty;
		public decimal Price { get; init; }
		public bool IsFree { get; init; }
		public Guid InstructorId { get; init; }
		public string InstructorName { get; init; } = string.Empty;
		public string ThumbnailUrl { get; init; } = string.Empty;
		public DateTime PublishedAt { get; init; }
	}
}
