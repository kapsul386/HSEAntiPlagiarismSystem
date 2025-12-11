using System.Collections.Concurrent;
using AntiPlagiarism.FileStoring.Domain.Abstractions;
using AntiPlagiarism.FileStoring.Domain.Entities;
using AntiPlagiarism.FileStoring.Domain.ValueObjects;

namespace AntiPlagiarism.FileStoring.Infrastructure.Repositories;

/// <summary>
/// In-memory хранилище метаданных файлов.
/// Используется для разработки и прототипирования.
/// </summary>
public sealed class InMemoryStoredFileRepository : IStoredFileRepository
{
    private readonly ConcurrentDictionary<FileId, StoredFile> _storage = new();

    public Task<StoredFile?> GetByIdAsync(
        FileId id,
        CancellationToken cancellationToken = default)
    {
        _storage.TryGetValue(id, out var stored);
        return Task.FromResult(stored);
    }

    public Task AddAsync(
        StoredFile file,
        CancellationToken cancellationToken = default)
    {
        if (!_storage.TryAdd(file.Id, file))
            throw new InvalidOperationException($"File with id {file.Id.Value} already exists.");

        return Task.CompletedTask;
    }

    public Task UpdateAsync(
        StoredFile file,
        CancellationToken cancellationToken = default)
    {
        _storage[file.Id] = file;
        return Task.CompletedTask;
    }
}