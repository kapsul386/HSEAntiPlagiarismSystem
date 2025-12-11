using AntiPlagiarism.FileStoring.Domain.ValueObjects;

namespace AntiPlagiarism.FileStoring.Domain.Entities;

/// <summary>
/// Доменная сущность сохранённого файла.
/// Содержит только проверенные метаданные.
/// </summary>
public class StoredFile
{
    public FileId Id { get; private set; }

    /// <summary>Ключ в физическом хранилище (путь или object key).</summary>
    public string StorageKey { get; private set; } = null!;

    /// <summary>Имя файла, переданное пользователем.</summary>
    public string OriginalFileName { get; private set; } = null!;

    public string ContentType { get; private set; } = null!;

    public long SizeBytes { get; private set; }

    public DateTime UploadedAtUtc { get; private set; }

    // Для ORM/сериализации
    private StoredFile() { }

    private StoredFile(
        FileId id,
        string storageKey,
        string originalFileName,
        string contentType,
        long sizeBytes,
        DateTime uploadedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
            throw new ArgumentException("Storage key cannot be empty.", nameof(storageKey));

        if (string.IsNullOrWhiteSpace(originalFileName))
            throw new ArgumentException("Original file name cannot be empty.", nameof(originalFileName));

        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type cannot be empty.", nameof(contentType));

        if (sizeBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(sizeBytes), "File size must be positive.");

        Id = id;
        StorageKey = storageKey;
        OriginalFileName = originalFileName;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        UploadedAtUtc = uploadedAtUtc;
    }

    /// <summary>
    /// Создать новый объект сохранённого файла с валидацией.
    /// </summary>
    public static StoredFile CreateNew(
        string storageKey,
        string originalFileName,
        string contentType,
        long sizeBytes,
        DateTime? uploadedAtUtc = null)
    {
        var id = FileId.New();
        var uploaded = uploadedAtUtc ?? DateTime.UtcNow;

        return new StoredFile(
            id,
            storageKey,
            originalFileName,
            contentType,
            sizeBytes,
            uploaded);
    }
}
