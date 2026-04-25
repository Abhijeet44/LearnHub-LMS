using AuthService.Application.Intrfaces;
using AuthService.Domain.Entities;
using MassTransit;
using LearnHub.Contracts.Events;

namespace AuthService.Infrastructure.Services
{
	public class RabbitMqEventPublisher : IEventPublisher
	{
		private readonly IBus _bus;

		public RabbitMqEventPublisher(IBus bus)
		{
			_bus = bus;
		}

		public async Task PublishUserRegisteredAsync(AppUser user)
		{
			var @event = new UserRegisterdEvent
			{
				UserId = user.Id,
				Email = user.Email,
				FullName = user.FullName,
				Role = user.Role.ToString(),
				CreatedAt = user.CreatedAt
			};

			await _bus.Publish(@event);
		}
	}
}
