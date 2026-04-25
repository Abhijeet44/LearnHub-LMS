using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Intrfaces
{
	public interface IPasswordHasher
	{
		string Hash(string plainPassword);
		bool Verify(string plainPassword, string storedHash);
	}
}
