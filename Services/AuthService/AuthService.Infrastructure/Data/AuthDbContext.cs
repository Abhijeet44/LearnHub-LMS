using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Data
{
	public class AuthDbContext : DbContext
	{
		public AuthDbContext(DbContextOptions<AuthDbContext> dbContextOptions) : base(dbContextOptions) { }

		public DbSet<AppUser> Users => Set<AppUser>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
			base.OnModelCreating(modelBuilder);
		}
	}
}
