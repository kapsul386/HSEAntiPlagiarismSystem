using System.IO;
using AntiPlagiarism.FileStoring.Application.Abstractions;
using AntiPlagiarism.FileStoring.Application.Models;
using AntiPlagiarism.FileStoring.Domain.Abstractions;
using AntiPlagiarism.FileStoring.Domain.Entities;
using AntiPlagiarism.FileStoring.Domain.ValueObjects;

namespace AntiPlagiarism.FileStoring.Application.Services;

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
        // 1. Сохраняем байты файла в физическое хранилище
        var storageKey = await _fileStorage.SaveAsync(content, cancellationToken);

        // 2. Создаём доменную сущность с проверками
        var storedFile = StoredFile.CreateNew(
            storageKey,
            originalFileName,
            contentType,
            sizeBytes);

        // 3. Сохраняем метаданные в хранилище
        await _storedFileRepository.AddAsync(storedFile, cancellationToken);

        // 4. Возвращаем модель наружу
        return MapToModel(storedFile);
    }

    public async Task<StoredFileModel?> GetAsync(
        FileId id,
        CancellationToken cancellationToken = default)
    {
        var storedFile = await _storedFileRepository.GetByIdAsync(id, cancellationToken);
        if (storedFile is null)
        {
            return null;
        }

        return MapToModel(storedFile);
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
