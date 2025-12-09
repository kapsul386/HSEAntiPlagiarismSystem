using System.IO;
using AntiPlagiarism.FileStoring.Application.Models;
using AntiPlagiarism.FileStoring.Domain.ValueObjects;

namespace AntiPlagiarism.FileStoring.Application.Abstractions;

public interface IFileStoringService
{
    Task<StoredFileModel> StoreAsync(
        Stream content,
        string originalFileName,
        string contentType,
        long sizeBytes,
        CancellationToken cancellationToken = default);

    Task<StoredFileModel?> GetAsync(
        FileId id,
        CancellationToken cancellationToken = default);
}