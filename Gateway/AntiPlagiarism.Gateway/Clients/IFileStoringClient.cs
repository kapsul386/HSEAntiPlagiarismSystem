using AntiPlagiarism.Gateway.Models;
using Microsoft.AspNetCore.Http;

namespace AntiPlagiarism.Gateway.Clients;

/// <summary>
/// Контракт для загрузки файлов в FileStoring.
/// </summary>
public interface IFileStoringClient
{
    Task<StoredFileDto> UploadFileAsync(
        IFormFile file,
        CancellationToken cancellationToken = default);
}