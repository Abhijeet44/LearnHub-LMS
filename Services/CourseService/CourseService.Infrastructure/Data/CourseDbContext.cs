using CourseService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Infrastructure.Data
{
	public class CourseDbContext : DbContext
	{
		public CourseDbContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<Course> Courses { get; set; }
		public DbSet<Section> Sections { get; set; }
		public DbSet<Lesson> Lessons { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourseDbContext).Assembly);

			modelBuilder.Entity<Course>().HasQueryFilter(c => !c.IsDeleted);
			modelBuilder.Entity<Section>().HasQueryFilter(s => !s.IsDeleted);
			modelBuilder.Entity<Lesson>().HasQueryFilter(l => !l.IsDeleted);

			base.OnModelCreating(modelBuilder);
		}

	}
}
