using CourseService.Application.Interfaces;
using CourseService.Domain.Entities;
using CourseService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Application.Services
{
	public class SectionRepository : ISectionRepository
	{
		private readonly CourseDbContext _context;

		public SectionRepository(CourseDbContext context)
		{
			_context = context;
		}
		public async Task AddAsync(Section section, CancellationToken ct = default)
		{
			await _context.Sections.AddAsync(section, ct);
		}

		public async Task<IEnumerable<Section>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default)
		{
			return await _context.Sections
				.AsNoTracking()
				.OrderBy(s => s.OrderIndex)
				.Include(s => s.Lessons.Where(l => !l.IsDeleted))
				.ToListAsync(ct);
		}

		public async Task<Section?> GetByIdAsync(Guid id, CancellationToken ct = default)
		{
			return await _context.Sections
				.Include(s => s.Lessons.Where(l => !l.IsDeleted))
				.FirstOrDefaultAsync(s => s.Id == id, ct);
		}

		public async Task<int> GetMaxOrderIndexAsync(Guid courseId, CancellationToken ct = default)
		{
			return await _context.Sections
				.Where(s => s.CourseId == courseId)
				.Select(s => (int?)s.OrderIndex)
				.MaxAsync(ct) ?? 0;
		}

		public Task SaveChangesAsync(CancellationToken ct = default)
		{
			return _context.SaveChangesAsync(ct);
		}
	}
}
