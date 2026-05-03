using CourseService.Application.DTOs.Request;
using CourseService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CourseService.API.Controllers
{
	[Route("api/courses/{courseId:guid}/sections")]
	[ApiController]
	public class SectionsController : ControllerBase
	{
		private readonly ISectionAppService _sectionAppService;

		public SectionsController(ISectionAppService sectionAppService)
		{
			_sectionAppService = sectionAppService;	
		}

		private Guid GetUserId() =>
			Guid.Parse(User.FindFirstValue("UserId")
				?? throw new UnauthorizedAccessException("UserId not found in token."));

		[HttpGet]
		public async Task<IActionResult> GetByCourseId(Guid courseId, CancellationToken ct)
		{
			var result = await _sectionAppService.GetByCourseIdAsync(courseId, ct);
			return Ok(result);
		}

		[HttpPost]
		[Authorize(Roles = "Instructor")]
		public async Task<IActionResult> Create(Guid courseId, [FromBody] CreateSectionDto createSectionDto, CancellationToken ct)
		{
			var result = await _sectionAppService.CreateAsync(courseId, createSectionDto, GetUserId(), ct);
			return Ok(result);
		}

		[HttpPut("{sectionId:guid}")]
		[Authorize(Roles = "Instructor")]
		public async Task<IActionResult> Update(Guid sectionId, [FromBody] UpdateSectionDto updateSectionDto, CancellationToken ct)
		{
			var result = await _sectionAppService.UpdateAsync(sectionId, updateSectionDto, GetUserId(), ct);
			return Ok(result);
		}

		[HttpDelete("{sectionId:guid}")]
		[Authorize(Roles = "Instructor")]
		public async Task<IActionResult> Delete(Guid sectionId, CancellationToken ct)
		{
			await _sectionAppService.DeleteAsync(sectionId, GetUserId(), ct);
			return NoContent();
		}
	}
}
