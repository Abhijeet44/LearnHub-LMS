using CourseService.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Domain.Entities
{
	public class Course
	{
		public Guid Id { get; private set; }
		public string Title { get; private set; } = string.Empty;
		public string Description { get; private set; } = string.Empty;
		public string ShortSummary { get; private set; } = string.Empty;


		public Guid InstructorId { get; set; }
		public string InstructorName { get; private set; } = string.Empty;

		public string ThumbnailUrl { get; private set; } = string.Empty;
		public decimal Price { get; private set; }
		public bool IsFree { get; private set; }
		public CourseLevel Level { get; private set; }
		public CourseStatus Status { get; private set; }
		public string RejectionReason { get; private set; } = string.Empty;
		public string Language { get; private set; } = "English";
		public int TotalDurationSeconds { get; private set; }
		public DateTime CreatedAt { get; private set; }
		public DateTime UpdatedAt { get; private set; }

		public bool IsDeleted { get; private set; }

		public ICollection<Section> Sections { get; private set; } = new List<Section>();

		private Course() { }

		public static Course Create(string title,
		string description,
		string shortSummary,
		Guid instructorId,
		string instructorName,
		decimal price,
		CourseLevel level,
		string language = "English")
		{
			return new Course
			{
				Id = Guid.NewGuid(),
				Title = title.Trim(),
				Description = description.Trim(),
				ShortSummary = shortSummary.Trim(),
				InstructorId = instructorId,
				InstructorName = instructorName,
				Price = price,
				IsFree = price == 0,
				Level = level,
				Status = CourseStatus.Draft,
				Language = language,
				TotalDurationSeconds = 0,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow,
				IsDeleted = false
			};
		}

		public void UpdateDetails(string title,
		string description,
		string shortSummary,
		decimal price,
		CourseLevel level,
		string language)
		{
			if (Status == CourseStatus.UnderReview || Status == CourseStatus.Published)
				throw new InvalidOperationException(
					"Cannot edit a course that is under review or already published.");

			Title = title.Trim();
			Description = description.Trim();
			ShortSummary = shortSummary.Trim();
			Price = price;
			IsFree = price == 0;
			Level = level;
			Language = language;
			UpdatedAt = DateTime.UtcNow;
		}

		public void SetThumbnail(string blobUrl)
		{
			ThumbnailUrl = blobUrl;
			UpdatedAt = DateTime.UtcNow;
		}

		public void SubmitForReview()
		{
			if (Status != CourseStatus.Draft && Status != CourseStatus.Rejected)
				throw new InvalidOperationException("Only courses in draft or rejected status can be submitted for review.");

			if (!Sections.Any() || Sections.Any(s => s.Lessons.Any()))
				throw new InvalidOperationException("Course must have at least one section with lessons before submitting for review.");

			Status = CourseStatus.UnderReview;
			UpdatedAt = DateTime.UtcNow;
			RejectionReason = null;

		}

		public void Publish()
		{
			if (Status != CourseStatus.UnderReview)
				throw new InvalidOperationException("Only courses under review can be approved.");
			Status = CourseStatus.Published;
			UpdatedAt = DateTime.UtcNow;
			RejectionReason = null;
		}

		public void Reject(string reason)
		{
			if (Status != CourseStatus.UnderReview)
				throw new InvalidOperationException("Only courses under review can be rejected.");

			Status = CourseStatus.Rejected;
			UpdatedAt = DateTime.UtcNow;
			RejectionReason = reason.Trim();
		}

		public void RecalculateDuration()
		{
			TotalDurationSeconds = Sections.SelectMany(s => s.Lessons).Sum(l => l.DurationSeconds);
			UpdatedAt = DateTime.UtcNow;
		}

		public void SoftDelete()
		{
			IsDeleted = true;
			UpdatedAt = DateTime.UtcNow;
		}
	}
}
