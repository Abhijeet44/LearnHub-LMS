using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Exception
{
	public abstract class InvalidCredentialException : DomainExceptions
	{
		public InvalidCredentialException(string identifier) : base("Invalid email or password.") { }
	}
}
