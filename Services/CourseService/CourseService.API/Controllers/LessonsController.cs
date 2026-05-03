using CourseService.Application.DTOs.Request;
using CourseService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourseService.API.Controllers
{
	[Route("api/sections/{sectionId:guid}/lessons")]
	[ApiController]
	public class LessonsController : ControllerBase
	{
		private readonly ILessonAppService _lessonAppService;

		public LessonsController(ILessonAppService lessonAppService)
		{
			_lessonAppService = lessonAppService;
		}

		private Guid GetUserId() => Guid.Parse(User.FindFirstValue("UserId") 
			?? throw new UnauthorizedAccessException("UserId not found in token."));

		[HttpGet]
		public async Task<IActionResult> Get(Guid sectionId, CancellationToken ct)
		{
			var result = await _lessonAppService.GetBySectionIdAsync(sectionId, ct);
			return Ok(result);
		}

		[HttpPost]
		[Authorize(Roles = "Instructor")]
		public async Task<IActionResult> Create(Guid sectionId, [FromBody] CreateLessonDto request, CancellationToken ct)
		{
			var result = await _lessonAppService.CreateAsync(sectionId, request, GetUserId(), ct);
			return Ok(result);
		}


		[HttpPut("{lessonId:guid}")]
		[Authorize(Roles = "Instructor")]
		public async Task<IActionResult> Update(Guid sectionId, Guid lessonId, [FromBody] UpdateLessonDto request, CancellationToken ct)
		{
			var result = await _lessonAppService.UpdateAsync(lessonId, request, GetUserId(), ct);
			return Ok(result);
		}

		[HttpPost("{lessonId:guid}/video")]
		[Authorize(Roles = "Instructor")]
		[RequestSizeLimit(500_000_000)] // 500 MB max
		public async Task<IActionResult> UploadVideo(Guid sectionId, Guid lessonId, IFormFile file, [FromForm] int durationInSec,  CancellationToken ct)
		{
			if (file == null || file.Length == 0)
				return BadRequest(new { error = "no file uploaded" });

			var allowedTypes = new[] { "video/mp4", "video/webm", "video/quicktime" };
			if (!allowedTypes.Contains(file.ContentType))
				return BadRequest(new { error = "Only MP4, WebM, and MOV videos are allowed." });

			using var stream = file.OpenReadStream();
			var url = await _lessonAppService.UploadVideoAsync(lessonId, stream, file.Name, file.ContentType, durationInSec, GetUserId(), ct);
			return Ok(new { videoUrl = url });
		}

		[HttpDelete("{lessonId:guid}")]
		[Authorize(Roles = "Instructor")]
		public async Task<IActionResult> Delete(Guid lessonId, Guid sectionId, CancellationToken ct)
		{
			await _lessonAppService.DeleteAsync(lessonId, GetUserId(), ct);
			return NoContent();
		}

	}
}
