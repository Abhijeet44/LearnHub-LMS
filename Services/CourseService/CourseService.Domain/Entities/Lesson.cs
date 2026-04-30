using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Domain.Entities
{
	public class Lesson
	{
		public Guid Id { get; private set; }
		public Guid SectionId { get; private set; }
		public string Title { get; private set; } = string.Empty;

		public string? VideoUrl { get; private set; } 
		public int DurationSeconds { get; private set; }

		public int OrderIndex { get; private set; }

		public bool IsPreview { get; private set; }

		public DateTime CreatedAt { get; private set; }
		public DateTime UpdatedAt { get; private set; }
		public bool IsDeleted { get; private set; }

		public Section Section { get; private set; } = null!;

		private Lesson() { }

		public static Lesson Create(Guid sectionId, string title, int orderIndex, bool isPreview = false)
		{
			return new Lesson
			{
				Id = Guid.NewGuid(),
				SectionId = sectionId,
				Title = title.Trim(),
				VideoUrl = null,
				DurationSeconds = 0,
				OrderIndex = orderIndex,
				IsPreview = isPreview,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow,
				IsDeleted = false
			};
		}

		public void UpdateDetails(string title, int orderIndex, bool isPreview)
		{
			Title = title.Trim();
			OrderIndex = orderIndex;
			IsPreview = isPreview;
			UpdatedAt = DateTime.UtcNow;
		}

		public void SetVideo(string blobUrl, int durationSeconds)
		{
			if (durationSeconds < 0)
			 throw new ArgumentException("Duration must be non-negative");

			VideoUrl = blobUrl;
			DurationSeconds = durationSeconds;
			UpdatedAt = DateTime.UtcNow;
		}

		public void SoftDelete()
		{
			IsDeleted = true;
			UpdatedAt = DateTime.UtcNow;
		}
	}
}
