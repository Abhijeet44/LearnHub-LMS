using AuthService.Application.DTOs;
using AuthService.Application.Intrfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.API.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly ILogger<AuthController> _logger;
		private readonly IAuthAppService _authAppService;

		public AuthController(ILogger<AuthController> logger, IAuthAppService authAppService)
		{
			this._logger = logger;
			this._authAppService = authAppService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken ct)
		{
			var result = await _authAppService.RegisterAsync(request, ct);
			return Ok(result);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken ct)
		{
			var result = await _authAppService.LoginAsync(request, ct);
			return Ok(result);
		}

		[HttpGet("me")]
		[Authorize]
		public IActionResult GetMe(CancellationToken ct)
		{
			var userId = GetCurrentUserId();
			var result = _authAppService.GetCurrentUserAsync(userId, ct).Result;
			return Ok(result);
		}

		[HttpPut("change-password")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request, CancellationToken ct)
		{
			var userId = GetCurrentUserId();
		    await _authAppService.ChangePasswordAsync(userId, request, ct);
			return NoContent();
		}

		[HttpGet("users")]
		public async Task<IActionResult> GetAllUsers(CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
		{
			var result = await _authAppService.GetAllUsersAsync(page, pageSize, ct);
			return Ok(result);
		}

		[HttpPut("users/{id:guid}/deactivate")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeactivateUser(Guid id, CancellationToken ct)
		{
			if (id == GetCurrentUserId())
			{
				return BadRequest(new { error = "You cannot deactivate your own account." });
			}
			await _authAppService.DeactivateUserAsync(id, ct);
			return NoContent();
		}

		[HttpPut("users/{id:guid}/activate")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> ActivateUser(Guid id, CancellationToken ct)
		{
			await _authAppService.ActivateUserAsync(id, ct);
			return NoContent();
		}

		private Guid GetCurrentUserId()
		{
			var userIdClaim = User.FindFirstValue("UserId")
				?? throw new UnauthorizedAccessException("User ID claim not found.");

			return Guid.Parse(userIdClaim);
		}
	}
}
