using AuthService.Application.Intrfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Services
{
	public class BcryptPasswordHasher : IPasswordHasher
	{
		private const int workFactor = 12;

		public string Hash(string plainPassword)
		{
			return BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor);
		}
		public bool Verify(string plainPassword, string storedHash)
		{
			return BCrypt.Net.BCrypt.Verify(plainPassword, storedHash);
		}
	}
}
