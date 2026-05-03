using CourseService.Application.DTOs.Request;
using CourseService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourseService.API.Controllers
{
	[Route("api/courses")]
	[ApiController]
	public class CoursesController : ControllerBase
	{
		private readonly ICourseAppService _courseAppService;

		public CoursesController(ICourseAppService courseAppService)
		{
			_courseAppService = courseAppService;
		}

		private Guid GetUserId() =>
			Guid.Parse(User.FindFirstValue("UserId")
				?? throw new UnauthorizedAccessException("UserId not found in token."));

		private string GetUserName() => User.FindFirstValue("UserName")
			?? "Unknown instructor.";

		[HttpGet]
		public async Task<IActionResult> GetPublishedCourses([FromQuery] CourseQueryDto courseQueryDto, CancellationToken ct)
		{
			var courses = await _courseAppService.GetPublishedCoursesAsync(courseQueryDto, ct);
			return Ok(courses);
		}

		[HttpGet("id:guid")]
		public async Task<IActionResult> GetById(Guid id, [FromQuery] bool isEnrolled = false, CancellationToken ct = default)
		{
			var result = await _courseAppService.GetByIdAsync(id, isEnrolled, ct);
			return Ok(result);
		}

		[HttpPost]
		[Authorize(Roles = "Instructor")]
		public async Task<IActionResult> Create([FromBody] CreateCourseDto createCourseDto, CancellationToken ct)
		{
			var instructorId = GetUserId();
			var instructorName = GetUserName();
			var result = await _courseAppService.CreateAsync(createCourseDto, instructorId, instructorName, ct);
			return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
		}

		[HttpPut("id:guid")]
		[Authorize(Roles = "Instructor")]
		public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseDto updateCourseDto, CancellationToken ct)
		{
			var result = await _courseAppService.UpdateAsync(id, updateCourseDto, GetUserId(), ct);
			return Ok(result);
		}

		[HttpPost("id:guid/thumbnail")]
		[Authorize(Roles = "Instructor")]
		public async Task<IActionResult> UploadThumbnail(Guid id, IFormFile file, CancellationToken ct)
		{
			if(file == null || file.Length == 0)
				return BadRequest(new { error = "No file provided." });
			var allowedExtensions = new[] { "image/jpeg", "image/png", "image/webp" };

			if (!allowedExtensions.Contains(file.ContentType.ToLower()))
				return BadRequest(new { error = "Invalid file type. Only JPEG, PNG, and WEBP are allowed." });

			var maxFileSize = 5 * 1024 * 1024; // 5 MB
			if (file.Length > maxFileSize)
				return BadRequest(new { error = "File size exceeds the 5 MB limit." });

			using var stream = file.OpenReadStream();

			var result = await _courseAppService.UploadThumbnailAsync(id, stream, file.FileName, file.ContentType, GetUserId(), ct);
			
			return Ok(new { thumbnailUrl = result });
		}

		[HttpPost("{id:guid}/submit")]
		[Authorize(Roles = "Instructor")]
		public async Task<IActionResult> SubmitForReview(Guid id, CancellationToken ct)
		{
			await _courseAppService.SubmitForReviewAsync(id, GetUserId(), ct);
			return NoContent();
		}

		[HttpDelete("id:guid")]
		[Authorize (Roles = "Instructor")]
		public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
		{
			await _courseAppService.DeleteAsync(id, GetUserId(), ct);
			return NoContent();
		}

		//[HttpGet]
		//[Authorize (Roles = "Instructor")]
		//public async Task<IActionResult> GetMyCourses()
		//{
		//	var result = await _courseAppService.GetInstructorCoursesAsync(GetUserId(), ct);
		//	return Ok(result);
		//}



		//  ADMIN ENDPOINTS

		[HttpGet("admin/under-review")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetCoursesUnderReview(CancellationToken ct)
		{
			var result = await _courseAppService.GetCoursesUnderReviewAsync(ct);
			return Ok(result);
		}

		[HttpPut("{id:guid}/publish")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
		{
			await _courseAppService.PublishAsync(id, ct);
			return NoContent();
		}

		[HttpPut("{id:guid}/reject")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Reject(Guid id, [FromBody] RejectCourseDto request, CancellationToken ct)
		{
			await _courseAppService.RejectAsync(id, request, ct);
			return NoContent();
		}
	}
}
