using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Exception
{
	public abstract class UserNotFoundException : DomainExceptions
	{
		public UserNotFoundException(string identifier) : base($"User `{identifier}` was not found") { }
	}
}
