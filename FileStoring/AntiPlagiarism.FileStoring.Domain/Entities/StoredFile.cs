using AntiPlagiarism.FileStoring.Domain.ValueObjects;

namespace AntiPlagiarism.FileStoring.Domain.Entities;

public class StoredFile
{
    public FileId Id { get; private set; }

    /// <summary>
    /// Ключ в хранилище: относительный путь на диске или key в S3/MinIO.
    /// Например: "works/2025/11/29/abc123.pdf"
    /// </summary>
    public string StorageKey { get; private set; } = null!;

    /// <summary>
    /// Оригинальное имя файла, которое загрузил пользователь.
    /// </summary>
    public string OriginalFileName { get; private set; } = null!;

    /// <summary>
    /// MIME-тип (например, "application/pdf", "application/vnd.openxmlformats-officedocument.wordprocessingml.document").
    /// </summary>
    public string ContentType { get; private set; } = null!;
    
    /// <summary>
    /// Размер файла в байтах.
    /// </summary>
    public long SizeBytes { get; private set; }
    
    /// <summary>
    /// Время загрузки в UTC.
    /// </summary>
    public DateTime UploadedAtUtc { get; private set; }

    private StoredFile() { } // для ORM / сериализации

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
