using CourseService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata; // Add this using directive for HasColumnType

namespace CourseService.Infrastructure.Configurations
{
	public class CourseConfigurations : IEntityTypeConfiguration<Course>
	{
		public void Configure(EntityTypeBuilder<Course> builder)
		{
			builder.HasKey(c => c.Id);

			builder.Property(c => c.Title)
			.IsRequired()
			.HasMaxLength(300);

			builder.Property(c => c.Description)
			.IsRequired();

			builder.Property(c => c.ShortSummary)
		   .IsRequired()
		   .HasMaxLength(500);

			builder.Property(c => c.InstructorName)
			.IsRequired()
			.HasMaxLength(200);

			builder.Property(c => c.Price)
		    .HasColumnType("decimal(10,2)");

			builder.Property(c => c.Status)
			.HasConversion<string>()
			.HasMaxLength(50);

			builder.Property(c => c.Level)
		   .HasConversion<string>()
		   .HasMaxLength(50);

			builder.Property(c => c.Language)
		    .HasMaxLength(100)
		    .HasDefaultValue("English");

			builder.Property(c => c.TotalDurationSeconds)
		   .HasDefaultValue(0);

			builder.HasIndex(c => c.Status);

			builder.HasIndex(c => c.InstructorId);

			builder.HasMany(c => c.Sections)
		   .WithOne(s => s.Course)
		   .HasForeignKey(s => s.CourseId)
		   .OnDelete(DeleteBehavior.Cascade);

		}
	}
}
