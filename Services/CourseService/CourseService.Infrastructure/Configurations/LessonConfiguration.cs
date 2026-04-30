using CourseService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Infrastructure.Configurations
{
	public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
	{
		public void Configure(EntityTypeBuilder<Lesson> builder)
		{
			builder.ToTable("Lessons");
			builder.HasKey(l => l.Id);

			builder.Property(l => l.Title)
				.IsRequired()
				.HasMaxLength(300);

			builder.Property(l => l.DurationSeconds)
				.HasDefaultValue(0);

			builder.HasIndex(l => new { l.SectionId, l.OrderIndex });

		}
	}
}
