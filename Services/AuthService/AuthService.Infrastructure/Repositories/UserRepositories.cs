using AuthService.Application.Intrfaces;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Repositories
{
	public class UserRepositories : IUserRepository
	{
		private readonly AuthDbContext _dbContext;
		public UserRepositories(AuthDbContext dbContext) 
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(AppUser user, CancellationToken ct = default)
		{
			 await _dbContext.Users.AddAsync(user, ct);
		}

		public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
		{
			return await _dbContext.Users.AnyAsync(u => u.Email == email.Trim().ToLower(), ct);
		}

		public async Task<IEnumerable<AppUser>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
		{
			return await _dbContext.Users.OrderByDescending(u => u.CreatedAt)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync(ct);
		}

		public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default)
		{
			return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email.Trim().ToLower(), ct);
		}

		public async Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default)
		{
			return await _dbContext.Users.FindAsync(id, ct);
		}

		public async Task<int> GetTotalCountAsync(CancellationToken ct = default)
		{
			return await _dbContext.Users.CountAsync(ct);
		}

		public async Task SaveChangesAsync(CancellationToken ct = default)
		{
			await _dbContext.SaveChangesAsync(ct);
		}
	}
}
