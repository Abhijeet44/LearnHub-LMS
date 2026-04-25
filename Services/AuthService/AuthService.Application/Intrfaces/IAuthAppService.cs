using AuthService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Intrfaces
{
	public interface IAuthAppService
	{
		Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest, CancellationToken ct = default);
		Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest, CancellationToken ct = default);
		Task<UserResponseDto> GetCurrentUserAsync(Guid UserId, CancellationToken ct = default);
		Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto, CancellationToken ct = default);
		Task<IEnumerable<UserResponseDto>> GetAllUsersAsync(int page, int pageSize, CancellationToken ct = default);
		Task DeactivateUserAsync(Guid userId, CancellationToken ct = default);
		Task ActivateUserAsync(Guid userId, CancellationToken ct = default);
	}
}
