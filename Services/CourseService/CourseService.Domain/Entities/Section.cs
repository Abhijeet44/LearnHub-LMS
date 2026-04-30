using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Domain.Entities
{
	public class Section
	{
		public Guid Id { get; private set; }
		public Guid CourseId { get; private set; }
		public string Title { get; private set; } = string.Empty;
		public int OrderIndex { get; private set; }
		public DateTime CreatedAt { get; private set; }
		public DateTime UpdatedAt { get; private set; }
		public bool IsDeleted { get; private set; }
		public Course Course { get; private set; } = null!;
		public ICollection<Lesson> Lessons { get; private set; } = new List<Lesson>();

		private Section() { }

		public static Section Create(Guid courseId, string title, int orderIndex)
		{
			return new Section
			{
				Id = Guid.NewGuid(),
				CourseId = courseId,
				Title = title.Trim(),
				OrderIndex = orderIndex,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow,
				IsDeleted = false
			};
		}

		public void UpdateTitle(string title, int orderIndex)
		{
			Title = title.Trim();
			UpdatedAt = DateTime.UtcNow;
		}

		public void UpdateOrder(int orderIndex)
		{
			OrderIndex = orderIndex;
			UpdatedAt = DateTime.UtcNow;
		}

		public void SoftDelete()
		{
			IsDeleted = true;
			UpdatedAt = DateTime.UtcNow;

			// Cascade soft delete to all lessons in this section
			foreach (var lesson in Lessons)
				lesson.SoftDelete();
		}
	}
}
