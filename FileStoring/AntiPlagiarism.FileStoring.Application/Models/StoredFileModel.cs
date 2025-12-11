namespace AntiPlagiarism.FileStoring.Application.Models;

/// <summary>
/// Метаданные сохранённого файла.
/// </summary>
public sealed class StoredFileModel
{
    public string Id { get; init; } = null!;
    public string StorageKey { get; init; } = null!;
    public string OriginalFileName { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public long SizeBytes { get; init; }
    public DateTime UploadedAtUtc { get; init; }
}
