using CourseService.Application.DTOs.Request;
using CourseService.Application.DTOs.Response;
using CourseService.Application.Interfaces;
using CourseService.Application.Mappings;
using CourseService.Domain.Entities;
using CourseService.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Services
{
	public class SectionAppService : ISectionAppService
	{
		private readonly ICourseRepository _courseRepository;
		private readonly ISectionRepository _sectionRepository;
		public SectionAppService(ICourseRepository courseRepository, ISectionRepository sectionRepository)
		{
			_courseRepository = courseRepository;
			_sectionRepository = sectionRepository;
		}

		public async Task<SectionDto> CreateAsync(Guid courseId, CreateSectionDto request, Guid instructorId, CancellationToken ct = default)
		{
			var course = await _courseRepository.GetByIdAsync(courseId, ct) 
				?? throw new CourseNotFoundException(courseId);

			if (course.InstructorId != instructorId)
			{
				throw new UnauthorizedAccessException("You are not authorized to update this course.");
			}

			int orderIndex;
			if (request.OrderIndex.HasValue)
			{
				orderIndex = request.OrderIndex.Value;
			}
			else
			{
				var maxOrder = await _sectionRepository.GetMaxOrderIndexAsync(courseId, ct);
				orderIndex = maxOrder + 1;
			}

			// create course section
			var section = Section.Create(courseId, request.Title, orderIndex);

			await _sectionRepository.AddAsync(section, ct);
			await _sectionRepository.SaveChangesAsync(ct);

			return CourseMapper.ToSectionDto(section);
		}

		public async Task DeleteAsync(Guid sectionId, Guid instructorId, CancellationToken ct = default)
		{
			var section = await _sectionRepository.GetByIdAsync(sectionId, ct) ??
				throw new SectionNotFoundException(sectionId);	

			var course = await _courseRepository.GetByIdAsync(section.CourseId, ct) ??
				throw new CourseNotFoundException(section.CourseId);

			if (course.InstructorId != instructorId)
			{
				throw new UnauthorizedAccessException();
			}

			section.SoftDelete();
			course.RecalculateDuration();
			await _courseRepository.SaveChangesAsync(ct);
		}

		public async Task<IEnumerable<SectionDto>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default)
		{
			var sections = await _sectionRepository.GetByCourseIdAsync(courseId, ct);
			return sections.
				OrderBy(s => s.OrderIndex).
				Select(CourseMapper.ToSectionDto);

		}

		public async Task<SectionDto> UpdateAsync(Guid sectionId, UpdateSectionDto request, Guid instructorId, CancellationToken ct = default)
		{
			var section = await _sectionRepository.GetByIdAsync(sectionId, ct) ??
				throw new SectionNotFoundException(sectionId);

			var course = await _courseRepository.GetByIdAsync(section.CourseId, ct) ??
				throw new CourseNotFoundException(section.CourseId);

			if(course.InstructorId != instructorId)
			{
				throw new UnauthorizedAccessException("You are not authorized to update this course.");
			}

			section.UpdateTitle(request.Title, request.OrderIndex);
			section.UpdateOrder(request.OrderIndex);

			await _sectionRepository.SaveChangesAsync(ct);
			return CourseMapper.ToSectionDto(section);
		}
	}
}
