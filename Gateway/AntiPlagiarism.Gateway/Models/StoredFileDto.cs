namespace AntiPlagiarism.Gateway.Models;

public sealed class StoredFileDto
{
    public Guid Id { get; init; }

    public string StorageKey { get; init; } = string.Empty;

    public string OriginalFileName { get; init; } = string.Empty;

    public string ContentType { get; init; } = string.Empty;

    public long SizeBytes { get; init; }

    public DateTime UploadedAtUtc { get; init; }
}