using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Intrfaces
{
	public interface IJwtTokenService
	{
		(string Token, DateTime ExpiresAt) GenerateToken(AppUser appUser);
	}
}
