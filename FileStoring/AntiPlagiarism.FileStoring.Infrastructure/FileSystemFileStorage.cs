using System.IO;
using AntiPlagiarism.FileStoring.Application.Abstractions;

namespace AntiPlagiarism.FileStoring.Infrastructure.FileStorage;

/// <summary>
/// Локальное файловое хранилище.
/// </summary>
public sealed class FileSystemFileStorage : IFileStorage
{
    private readonly string _baseDirectory;

    public FileSystemFileStorage(string baseDirectory)
    {
        if (string.IsNullOrWhiteSpace(baseDirectory))
            throw new ArgumentException("Base directory must be provided", nameof(baseDirectory));

        _baseDirectory = baseDirectory;
    }

    public async Task<string> SaveAsync(
        Stream content,
        CancellationToken cancellationToken = default)
    {
        if (content is null)
            throw new ArgumentNullException(nameof(content));

        Directory.CreateDirectory(_baseDirectory);

        var fileName = $"{Guid.NewGuid():N}.bin";
        var fullPath = Path.Combine(_baseDirectory, fileName);

        await using var fileStream = new FileStream(
            fullPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            useAsync: true);

        await content.CopyToAsync(fileStream, cancellationToken);

        return fileName; // storage key
    }

    public Task<Stream> OpenReadAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
            throw new ArgumentException("Storage key must be specified", nameof(storageKey));

        var fullPath = Path.Combine(_baseDirectory, storageKey);

        Stream stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            81920,
            useAsync: true);

        return Task.FromResult(stream);
    }
}