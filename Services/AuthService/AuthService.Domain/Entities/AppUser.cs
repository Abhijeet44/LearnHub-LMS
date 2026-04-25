using AuthService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
	public class AppUser
	{
		public Guid Id { get; set; }
		public string Email { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
		public UserRole Role { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		private AppUser() { }

		public static AppUser Create(string email, string passwordHash, string fullName, UserRole role)
		{
			return new AppUser
			{
				Id = Guid.NewGuid(),
				Email = email,
				PasswordHash = passwordHash,
				FullName = fullName,
				Role = role,
				IsActive = true,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};
		}

		public void Deactivate()
		{
			IsActive = false;
			UpdatedAt = DateTime.UtcNow;
		}

		public void Activate()
		{
			IsActive = true;
			UpdatedAt = DateTime.UtcNow;
		}

		public void UpdatePassword(string newPasswordHash)
		{
			PasswordHash = newPasswordHash;
			UpdatedAt = DateTime.UtcNow;
		}

		public void UpdateProfile(string fullName)
		{
			FullName = fullName.Trim();
			UpdatedAt = DateTime.UtcNow;
		}
	}
}
