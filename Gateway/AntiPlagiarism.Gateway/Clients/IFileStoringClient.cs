using AntiPlagiarism.Gateway.Models;
using Microsoft.AspNetCore.Http;

namespace AntiPlagiarism.Gateway.Clients;

public interface IFileStoringClient
{
    Task<StoredFileDto> UploadFileAsync(
        IFormFile file,
        CancellationToken cancellationToken = default);
}