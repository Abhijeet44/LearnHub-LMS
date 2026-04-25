using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnHub.Contracts.Events
{
	public class UserRegisterdEvent
	{
		public Guid UserId { get; set; }
		public string Email { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
