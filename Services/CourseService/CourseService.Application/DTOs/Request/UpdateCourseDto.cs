using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.DTOs.Request
{
	public class UpdateCourseDto
	{
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string ShortSummary { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public string Level { get; set; } = "Beginner";
		public string Language { get; set; } = "English";
	}
}
