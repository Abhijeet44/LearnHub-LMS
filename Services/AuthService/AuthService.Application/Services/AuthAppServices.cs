using AuthService.Application.DTOs;
using AuthService.Application.Intrfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using System.Net.NetworkInformation;
using System.Security.Authentication;

namespace AuthService.Application.Services
{
	public class AuthAppServices : IAuthAppService
	{
		private readonly IUserRepository _userRepository;
		private readonly IJwtTokenService _jwtTokenService;
		private readonly IPasswordHasher _passwordHasher;
		private readonly IEventPublisher _eventPublisher;

		public AuthAppServices(IUserRepository userRepository, IJwtTokenService jwtTokenService, IPasswordHasher passwordHasher, IEventPublisher eventPublisher)
		{
			_userRepository = userRepository;
			_jwtTokenService = jwtTokenService;
			_passwordHasher = passwordHasher;
			_eventPublisher = eventPublisher;
		}

		public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest, CancellationToken ct = default)
		{
			var emailExists = await _userRepository.EmailExistsAsync(registerRequest.Email);
			if (emailExists)
				throw new ApplicationException($"{registerRequest.Email} already exists.");

			if(!Enum.TryParse<UserRole>(registerRequest.Role, true, out var userRole))
				userRole = UserRole.Student;

			if(userRole == UserRole.Admin)
				throw new UnauthorizedAccessException("Admin Role can not be self assigned.");

			var hash = _passwordHasher.Hash(registerRequest.Password);
			var user = AppUser.Create(registerRequest.Email, hash, registerRequest.FullName, userRole);

			await _userRepository.AddAsync(user);
			await _userRepository.SaveChangesAsync(ct);

			await _eventPublisher.PublishUserRegisteredAsync(user);

			var (token, expiresAt) = _jwtTokenService.GenerateToken(user);

			return new AuthResponseDto
			{
				Token = token,
				Email = user.Email,
				FullName = user.FullName,
				Role = user.Role.ToString(),
				ExpiresAt = expiresAt
			};
		}

		public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest, CancellationToken ct = default)
		{
			var user = await _userRepository.GetByEmailAsync(loginRequest.Email);
			if(user == null)
				throw new InvalidCredentialException("Invalid credentials.");

			if(!user.IsActive)
				throw new UnauthorizedAccessException("User account is deactivated.");

			var validPassword = _passwordHasher.Verify(loginRequest.Password, user.PasswordHash);

			if (!validPassword)
				throw new InvalidCredentialException("Invalid credentials.");

			var (token, expiresAt) = _jwtTokenService.GenerateToken(user);

			return new AuthResponseDto
			{
				Token = token,
				Email = user.Email,
				FullName = user.FullName,
				Role = user.Role.ToString(),
				ExpiresAt = expiresAt
			};
		}

		public async Task<UserResponseDto> GetCurrentUserAsync(Guid userId, CancellationToken ct = default)
		{
			var user = await _userRepository.GetByIdAsync(userId)
				?? throw new ApplicationException("User not found.");

			return MapToUserResponseDto(user);
		}
		public async Task ActivateUserAsync(Guid userId, CancellationToken ct = default)
		{
			var user = await _userRepository.GetByIdAsync(userId)
				?? throw new ApplicationException(userId.ToString());

			user.Activate();
			await _userRepository.SaveChangesAsync(ct);
		}

		public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto, CancellationToken ct = default)
		{
			var user = _userRepository.GetByIdAsync(userId).Result
				?? throw new ApplicationException(userId.ToString());

			var currentPasswordValid = _passwordHasher.Verify(changePasswordDto.CurrentPassword, user.PasswordHash);

			if(!currentPasswordValid)
				throw new InvalidCredentialException("Current password is incorrect.");

			var newHash = _passwordHasher.Hash(changePasswordDto.NewPassword);
			user.UpdatePassword(newHash);

			await _userRepository.SaveChangesAsync(ct);
		}

		public async Task DeactivateUserAsync(Guid userId, CancellationToken ct = default)
		{
			var user = await _userRepository.GetByIdAsync(userId)
				?? throw new ApplicationException(userId.ToString());

			user.Deactivate();
			await _userRepository.SaveChangesAsync(ct);
		}

		public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync(int page, int pageSize, CancellationToken ct = default)
		{
			var users = await _userRepository.GetAllAsync(page, pageSize);
			return users.Select(MapToUserResponseDto);
		}

		private static UserResponseDto MapToUserResponseDto(AppUser user)
		{
			return new UserResponseDto
			{
				Id = user.Id,
				Email = user.Email,
				FullName = user.FullName,
				Role = user.Role.ToString(),
				IsActive = user.IsActive
			};
		}
	}
}
