using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.DTOs.Response
{
	public class PagedResultDto<T>
	{
		public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
		public int TotalCount { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }

		public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
		public bool HasNextPage => Page < TotalPages;
		public bool HasPreviousPage => Page > 1;
	}
}
