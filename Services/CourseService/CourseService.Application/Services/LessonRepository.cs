using CourseService.Application.Interfaces;
using CourseService.Domain.Entities;
using CourseService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Services
{
	public class LessonRepository : ILessonRepository
	{
		private readonly CourseDbContext _context;

		public LessonRepository(CourseDbContext context)
		{
			_context = context;
		}

		public async Task AddAsync(Lesson lesson, CancellationToken ct = default)
		{
			await _context.Lessons.AddAsync(lesson, ct);
		}

		public async Task<Lesson?> GetByIdAsync(Guid id, CancellationToken ct = default)
		{
			return await _context.Lessons.FirstOrDefaultAsync(l => l.Id == id, ct);
		}

		public async Task<IEnumerable<Lesson>> GetBySectionIdAsync(Guid sectionId, CancellationToken ct = default)
		{
			var lessons = await _context.Lessons
				.AsNoTracking()
				.Where(l => l.SectionId == sectionId)
				.OrderBy(l => l.OrderIndex)
				.ToListAsync(ct);
			return lessons;

		}

		public async Task<int> GetMaxOrderIndexAsync(Guid sectionId, CancellationToken ct = default)
		{
			return await _context.Lessons
				.Where(l => l.SectionId == sectionId)
				.Select(l => (int?)l.OrderIndex)
				.MaxAsync(ct) ?? 0;
		}

		public async Task<int> GetTotalLessonCountByCourseIdAsync(Guid courseId, CancellationToken ct = default)
		{
			return await _context.Lessons
				.Where(l => l.Section.CourseId == courseId)
				.CountAsync(ct);
		}

		public async Task SaveChangesAsync(CancellationToken ct = default)
		{
			await _context.SaveChangesAsync(ct);
		}
	}
}
