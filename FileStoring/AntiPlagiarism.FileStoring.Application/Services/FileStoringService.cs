using System.IO;
using AntiPlagiarism.FileStoring.Application.Abstractions;
using AntiPlagiarism.FileStoring.Application.Models;
using AntiPlagiarism.FileStoring.Domain.Abstractions;
using AntiPlagiarism.FileStoring.Domain.Entities;
using AntiPlagiarism.FileStoring.Domain.ValueObjects;

namespace AntiPlagiarism.FileStoring.Application.Services;

/// <summary>
/// Координирует физическое сохранение файла и запись метаданных.
/// </summary>
public sealed class FileStoringService : IFileStoringService
{
    private readonly IFileStorage _fileStorage;
    private readonly IStoredFileRepository _storedFileRepository;

    public FileStoringService(
        IFileStorage fileStorage,
        IStoredFileRepository storedFileRepository)
    {
        _fileStorage = fileStorage;
        _storedFileRepository = storedFileRepository;
    }

    public async Task<StoredFileModel> StoreAsync(
        Stream content,
        string originalFileName,
        string contentType,
        long sizeBytes,
        CancellationToken cancellationToken = default)
    {
        // Сохраняем файл в физическое хранилище
        var storageKey = await _fileStorage.SaveAsync(content, cancellationToken);

        // Создаём доменную сущность (валидация здесь)
        var storedFile = StoredFile.CreateNew(
            storageKey,
            originalFileName,
            contentType,
            sizeBytes);

        // Сохраняем метаданные
        await _storedFileRepository.AddAsync(storedFile, cancellationToken);

        return MapToModel(storedFile);
    }

    public async Task<StoredFileModel?> GetAsync(
        FileId id,
        CancellationToken cancellationToken = default)
    {
        var storedFile = await _storedFileRepository.GetByIdAsync(id, cancellationToken);
        return storedFile is null ? null : MapToModel(storedFile);
    }

    private static StoredFileModel MapToModel(StoredFile storedFile)
    {
        return new StoredFileModel
        {
            Id = storedFile.Id.Value.ToString(),
            StorageKey = storedFile.StorageKey,
            OriginalFileName = storedFile.OriginalFileName,
            ContentType = storedFile.ContentType,
            SizeBytes = storedFile.SizeBytes,
            UploadedAtUtc = storedFile.UploadedAtUtc
        };
    }
}
