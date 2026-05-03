using Azure.Storage.Blobs;
using CourseService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace CourseService.Application.Services
{
	public class AzureBlobStorageService : IBlobStorageService
	{
		private readonly BlobServiceClient _blobServiceClient;

		public AzureBlobStorageService(IConfiguration configuration)
		{
			var connectionString = configuration["AzureBlob:ConnectionString"]
				?? throw new InvalidOperationException("Azure Blob Storage connection string is not configured.");

			_blobServiceClient = new BlobServiceClient(connectionString);
		}

		public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string containerName, CancellationToken ct = default)
		{
			// Get or create the container
			// createIfNotExists = if "thumbnails" container doesn't exist yet, create it
			var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
			await containerClient.CreateIfNotExistsAsync(publicAccessType: PublicAccessType.Blob, cancellationToken: ct);

			var blobClient = containerClient.GetBlobClient(fileName);
			await blobClient.UploadAsync(fileStream, new BlobHttpHeaders
				{ 
					ContentType = contentType 
				}, cancellationToken: ct);

			return blobClient.Uri.ToString();

		}

		public async Task DeleteAsync(string blobUrl, string containerName, CancellationToken ct = default)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
			var Uri = new Uri(blobUrl);

			// Last segment of path after the container name is the blob name
			// uri.AbsolutePath = "/devstoreaccount1/thumbnails/courseId/guid.jpg"
			// We split and take everything after the container segment
			var blobName = string.Join("/", Uri.AbsolutePath.Split('/').Skip(3));  // skip: "", "account", "container"

			var blobClient = containerClient.GetBlobClient(blobName);

			await blobClient.DeleteIfExistsAsync(cancellationToken: ct);

		}

		public async Task<string> GetUrlAsync(string fileName, string containerName, CancellationToken ct = default)
		{
			var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
			var blobClient = containerClient.GetBlobClient(fileName);
			return await Task.FromResult(blobClient.Uri.ToString());
		}
	}
}
