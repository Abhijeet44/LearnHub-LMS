using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Data.Configurations
{
	public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
	{
		public void Configure(EntityTypeBuilder<AppUser> builder)
		{
			builder.ToTable("AppUsers");

			builder.HasKey(u => u.Id);

			builder.HasIndex(u => u.Email).IsUnique();

			builder.Property(u => u.Email).IsRequired().HasMaxLength(256);

			builder.Property(u => u.PasswordHash).IsRequired();

			builder.Property(u => u.FullName).HasMaxLength(200);

			builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(50);

			builder.Property(u => u.CreatedAt).IsRequired();

			builder.Property(u => u.UpdatedAt).IsRequired();
		}
	}
}
