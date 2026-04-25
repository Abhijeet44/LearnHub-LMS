using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Exception
{
	public abstract class DomainExceptions : System.Exception
	{
		protected DomainExceptions(string message) : base(message) { }
	}
}
