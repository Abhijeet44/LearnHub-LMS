using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.DTOs.Request
{
	public class CreateSectionDto
	{
		public string Title { get; set; } = string.Empty;
		public int? OrderIndex { get; set; }
	}
}
