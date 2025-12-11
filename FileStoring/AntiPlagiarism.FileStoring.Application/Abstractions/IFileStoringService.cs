using System.IO;
using AntiPlagiarism.FileStoring.Application.Models;
using AntiPlagiarism.FileStoring.Domain.ValueObjects;

namespace AntiPlagiarism.FileStoring.Application.Abstractions;

/// <summary>
/// Сервис для сохранения файлов и работы с их метаданными.
/// </summary>
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