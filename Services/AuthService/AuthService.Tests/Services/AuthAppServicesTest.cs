using AuthService.Application.Intrfaces;
using AuthService.Application.Services;
using Moq;
using AuthService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthService.Domain.Entities;
using FluentAssertions;
using AuthService.Domain.Enums;
using System.Security.Authentication;

namespace AuthService.Tests.Services
{
	public class AuthAppServicesTest
	{
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
		private readonly Mock<IPasswordHasher> _passwordHasherMock;
		private readonly Mock<IEventPublisher> _eventPublisherMock;
		private readonly AuthAppServices _sut;

		public AuthAppServicesTest()
		{
			_userRepositoryMock = new Mock<IUserRepository>();
			_jwtTokenServiceMock = new Mock<IJwtTokenService>();
			_passwordHasherMock = new Mock<IPasswordHasher>();
			_eventPublisherMock = new Mock<IEventPublisher>();

			_sut = new AuthAppServices(
				_userRepositoryMock.Object,
				_jwtTokenServiceMock.Object,
				_passwordHasherMock.Object,
				_eventPublisherMock.Object
			);
		}

		[Fact]
		public async Task RegisterAsync_ShouldThrowException_WhenEmailNotTaken()
		{
			// Arrange
			var registerRequest = new RegisterRequestDto
			{
				Email = "test@example.com",
				Password = "Test@12345",
				FullName = "Test User",
				Role = "Student"
			};

			_userRepositoryMock.Setup(r => r.EmailExistsAsync(registerRequest.Email, default)).ReturnsAsync(false);

			_jwtTokenServiceMock.Setup(j => j.GenerateToken(It.IsAny<AppUser>())).Returns(("mock_token", DateTime.UtcNow.AddHours(1)));
			_passwordHasherMock.Setup(h => h.Hash(registerRequest.Password)).Returns("hashedPassword");

			_eventPublisherMock.Setup(j => j.PublishUserRegisteredAsync(It.IsAny<AppUser>())).Returns(Task.CompletedTask);

			// Act
			var result = await _sut.RegisterAsync(registerRequest, default);

			// Assert
			result.Token.Should().Be("mock_token");
			result.Email.Should().Be(registerRequest.Email);
			result.Role.Should().Be("Student");

			_userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AppUser>(), default), Times.Once);
			_userRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Once);

			_eventPublisherMock.Verify(r => r.PublishUserRegisteredAsync(It.IsAny<AppUser>()), Times.Once);

		}

		[Fact]
		public async Task RegisterAsync_ShouldThrowException_WhenEmailAlreadyExists()
		{
			_userRepositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), default)).ReturnsAsync(true);

			var request = new RegisterRequestDto
			{
				Email = "testemail@gmail.com",
				Password = "User@12345",
				FullName = "Test User",
				Role = "Student"
			};

			await _sut.Invoking(s => s.RegisterAsync(request, default)).Should().ThrowAsync<ApplicationException>()
				.WithMessage("*already exists.");

			_userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AppUser>(), default), Times.Never);
		}

		[Fact]
		public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
		{
			var appUser = AppUser.Create("test@gmail.com", "Test@12345", "Test User", UserRole.Student);

			_userRepositoryMock.Setup(r => r.GetByEmailAsync("test@gmail.com", default)).ReturnsAsync(appUser);

			_passwordHasherMock.Setup(p => p.Verify("Test@12345", It.IsAny<string>())).Returns(true);

			_jwtTokenServiceMock.Setup(j => j.GenerateToken(appUser)).Returns(("jwt_token", DateTime.UtcNow.AddHours(1)));

			var request = new LoginRequestDto
			{
				Email = "test@gmail.com",
				Password = "Test@12345"
			};
			var result = await _sut.LoginAsync(request);

			//result.Token.Should().NotBeNull();
			result.Token.Should().Be("jwt_token");
		}

		[Fact]
		public async Task LoginAsync_WithWrongPassword_ThrowInvalidCredentials()
		{
			var appUser = AppUser.Create("test@gmail.com", "Test@12345", "Test User", UserRole.Student);

			_userRepositoryMock.Setup(r => r.GetByEmailAsync("test@gmail.com", default)).ReturnsAsync(appUser);

			_passwordHasherMock.Setup(p => p.Verify("wrong_password", "stored_hash")).Returns(true);

			_jwtTokenServiceMock.Setup(j => j.GenerateToken(appUser)).Returns(("jwt_token", DateTime.UtcNow.AddHours(1)));

			var request = new LoginRequestDto
			{
				Email = "test@gmail.com",
				Password = "Test@12345"
			};

			var result = await _sut.Invoking(x => x.LoginAsync(request)).Should().ThrowAsync<InvalidCredentialException>();

		}

		[Fact]
		public async Task DeactivateUserAsync_WhenUserExists_DeactivatesUser()
		{
			// Arrange
			var user = AppUser.Create("user@test.com", "hash", "User", UserRole.Student);
			_userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id, default)).ReturnsAsync(user);

			// Act
			await _sut.DeactivateUserAsync(user.Id);

			// Assert — IsActive changed, SaveChanges was called
			user.IsActive.Should().BeFalse();
			_userRepositoryMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
		}
	}
}
