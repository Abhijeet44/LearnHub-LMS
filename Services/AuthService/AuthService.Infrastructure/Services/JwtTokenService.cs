using AuthService.Application.Intrfaces;
using AuthService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Services
{
	public class JwtTokenService : IJwtTokenService
	{
		private readonly IConfiguration _configuration;
		public JwtTokenService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public (string Token, DateTime ExpiresAt) GenerateToken(AppUser appUser)
		{
			var secret = _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt secret is not configured.");
			var issuer = _configuration["Jwt:issuer"];
			var audience = _configuration["Jwt:audience"];
			var expiryMinutes = int.Parse(_configuration["Jwt:expiryMinutes"] ?? "60");

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, appUser.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.Role, appUser.Role.ToString()),
				new Claim("fullName", appUser.FullName),
				new Claim("UserId", appUser.Id.ToString())
			};

			var token = new JwtSecurityToken(
				issuer: issuer,
				audience: audience,
				claims: claims,
				expires: expiresAt,
				signingCredentials: credentials
			);

			var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

			return (tokenString, expiresAt);
		}
	}
}
