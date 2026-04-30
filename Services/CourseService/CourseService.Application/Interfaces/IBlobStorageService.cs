using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseService.Application.Interfaces
{
	public interface IBlobStorageService
	{
		Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string containerName, CancellationToken ct = default);
		Task DeleteAsync(string blobUrl, string containerName, CancellationToken ct = default);
		
		
		// Generates a URL for a blob
		// For public containers: returns direct URL
		// For private containers: returns a SAS (Shared Access Signature) URL with expiry
		Task<string> GetUrlAsync(string fileName, string containerName, CancellationToken ct = default);
	}
}
