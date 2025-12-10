using System.Collections.Concurrent;
using AntiPlagiarism.Analysis.Domain.Abstractions;
using AntiPlagiarism.Analysis.Domain.Entities;
using AntiPlagiarism.Analysis.Domain.ValueObjects;

namespace AntiPlagiarism.Analysis.Infrastructure.Repositories;

/// <summary>
/// In-memory хранилище сдач работ.
/// Используется для разработки и тестов.
/// </summary>
public sealed class InMemoryWorkRepository : IWorkRepository
{
    private readonly ConcurrentDictionary<WorkId, WorkSubmission> _storage = new();

    public Task<WorkSubmission?> GetByIdAsync(
        WorkId id,
        CancellationToken cancellationToken = default)
    {
        _storage.TryGetValue(id, out var work);
        return Task.FromResult(work);
    }

    public Task<IReadOnlyList<WorkSubmission>> GetByAssignmentAsync(
        string assignmentId,
        CancellationToken cancellationToken = default)
    {
        var result = _storage.Values
            .Where(w => w.AssignmentId == assignmentId)
            .OrderBy(w => w.SubmittedAtUtc)
            .ToArray();

        return Task.FromResult<IReadOnlyList<WorkSubmission>>(result);
    }

    public Task AddAsync(
        WorkSubmission work,
        CancellationToken cancellationToken = default)
    {
        if (!_storage.TryAdd(work.Id, work))
        {
            throw new InvalidOperationException($"Work with id {work.Id.Value} already exists.");
        }

        return Task.CompletedTask;
    }
}