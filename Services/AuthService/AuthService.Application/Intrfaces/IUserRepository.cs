using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Intrfaces
{
	public interface IUserRepository
	{
		Task<AppUser?> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default);
		Task<IEnumerable<AppUser>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
		Task<int> GetTotalCountAsync(CancellationToken ct = default);
		Task AddAsync(AppUser user, CancellationToken ct = default);
		Task SaveChangesAsync(CancellationToken ct = default);
		Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);

	}
}
