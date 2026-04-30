using CourseService.Application.DTOs.Request;
using CourseService.Application.Interfaces;
using CourseService.Domain.Entities;
using CourseService.Domain.Enum;
using CourseService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static MassTransit.Logging.OperationName;


namespace CourseService.Application.Services
{
	public class CourseRepository : ICourseRepository
	{
		private readonly CourseDbContext _dbContext;

		public CourseRepository(CourseDbContext courseDbContext)
		{
			_dbContext = courseDbContext;
		}

		public async Task AddAsync(Course course, CancellationToken ct = default)
		{
			await _dbContext.Courses.AddAsync(course, ct);
		}

		public Task<bool> ExistsAndBelongsToInstructorAsync(Guid courseId, Guid instructorId, CancellationToken ct = default)
		{
			return _dbContext.Courses.AnyAsync(c => c.Id == courseId && c.InstructorId == instructorId, ct);
		}

		public async Task<Course> GetByIdAsync(Guid id, CancellationToken ct = default)
		{
			return await _dbContext.Courses
				.Include(c => c.Sections.Where(s => !s.IsDeleted))
					.ThenInclude(s => s.Lessons.Where(l => !l.IsDeleted))
					.FirstOrDefaultAsync(c => c.Id == id, ct);
		}

		public async Task<Course?> GetByIdNoTrackingAsync(Guid id, CancellationToken ct = default)
		{
			return await _dbContext.Courses
			.AsNoTracking()
			.Include(c => c.Sections.Where(s => !s.IsDeleted))
				.ThenInclude(s => s.Lessons.Where(l => !l.IsDeleted))
			.FirstOrDefaultAsync(c => c.Id == id, ct);
		}

		public async Task<IEnumerable<Course>> GetByInstructorIdAsync(Guid instructorId, CancellationToken ct = default)
		{
			return await _dbContext.Courses
				.AsNoTracking()
				.Where(c => c.InstructorId == instructorId)
				.OrderByDescending(c => c.UpdatedAt)
				.Include(c => c.Sections.Where(s => !s.IsDeleted))
					.ThenInclude(s => s.Lessons.Where(l => !l.IsDeleted))
				.ToListAsync(ct);
		}

		public async Task<(IEnumerable<Course> Courses, int TotalCount)> GetPublishedAsync(CourseQueryDto query, CancellationToken ct = default)
		{
			var baseQuery = _dbContext.Courses
			.AsNoTracking()
			.Where(c => c.Status == CourseStatus.Published);

			if (!string.IsNullOrWhiteSpace(query.Search) && Enum.TryParse<CourseLevel>(query.Level, ignoreCase: true, out var level))
			{
				baseQuery = baseQuery.Where(c => c.Level == level);
			}

			if (!string.IsNullOrWhiteSpace(query.Language))
			{
				baseQuery = baseQuery.Where(c =>
					c.Language.ToLower() == query.Language.Trim().ToLower());
			}

			if (query.IsFree.HasValue)
			{
				baseQuery = baseQuery.Where(c => c.IsFree == query.IsFree.Value);
			}

			var totalCount = await baseQuery.CountAsync(ct);

			var courses = await baseQuery
				.OrderByDescending(c => c.CreatedAt)
				.Skip((query.Page - 1) * query.PageSize)
				.Take(query.PageSize)
				.Include(c => c.Sections.Where(s => !s.IsDeleted))
				.ThenInclude(s => s.Lessons.Where(l => !l.IsDeleted))
				.ToListAsync(ct);

			return (courses, totalCount);
		}

		public async Task<IEnumerable<Course>> GetUnderReviewAsync(CancellationToken ct = default)
		{
			return await _dbContext.Courses
				.AsNoTracking()
				.Where(c => c.Status == CourseStatus.UnderReview)
				.OrderByDescending(c => c.UpdatedAt)
				.Include(c => c.Sections.Where(s => !s.IsDeleted))
					.ThenInclude(s => s.Lessons.Where(l => !l.IsDeleted))
				.ToListAsync(ct);
		}

		public async Task SaveChangesAsync(CancellationToken ct = default)
		{
			await _dbContext.SaveChangesAsync(ct);
		}
	}
}
