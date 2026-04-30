using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.DTOs.Request
{
	public class CourseQueryDto
	{
		public string? Search { get; set; }

		public string? Level { get; set; }

		public string? Language { get; set; }

		public bool? IsFree { get; set; }

		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 12; 
	}
}
