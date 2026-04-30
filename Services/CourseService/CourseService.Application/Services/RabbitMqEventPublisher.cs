using CourseService.Application.Interfaces;
using CourseService.Domain.Entities;
using LearnHub.Contracts.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Services
{
	public class RabbitMqEventPublisher : IEventPublisher
	{
		private readonly IBus _bus;
		public RabbitMqEventPublisher(IBus bus)
		{
			_bus = bus;
		}
		public async Task PublishCoursePublishedAsync(Course course, CancellationToken ct = default)
		{
			var @event = new CoursePublishedEvent
			{
				CourseId = course.Id,
				Title = course.Title,
				Price = course.Price,
				IsFree = course.IsFree,
				InstructorId = course.InstructorId,
				InstructorName = course.InstructorName,
				ThumbnailUrl = course.ThumbnailUrl ?? string.Empty,
				PublishedAt = DateTime.UtcNow
			};

			await _bus.Publish(@event, ct);
		}
	}
}
