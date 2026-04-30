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
	public class SectionConfiguration : IEntityTypeConfiguration<Section>
	{
		public void Configure(EntityTypeBuilder<Section> builder)
		{
			builder.ToTable("Sections");
			builder.HasKey(s => s.Id);

			builder.Property(s => s.Title)
				.IsRequired()
				.HasMaxLength(300);

			builder.Property(s => s.OrderIndex)
				.IsRequired();

			builder.HasIndex(s => new { s.CourseId, s.OrderIndex });

			builder.HasMany(s => s.Lessons)
				.WithOne(l => l.Section)
				.HasForeignKey(l => l.SectionId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
