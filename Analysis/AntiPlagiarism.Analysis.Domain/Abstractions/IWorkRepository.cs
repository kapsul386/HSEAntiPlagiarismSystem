using AntiPlagiarism.Analysis.Domain.Entities;
using AntiPlagiarism.Analysis.Domain.ValueObjects;

namespace AntiPlagiarism.Analysis.Domain.Abstractions;

/// <summary>
/// Хранилище сданных работ.
/// </summary>
public interface IWorkRepository
{
    Task<WorkSubmission?> GetByIdAsync(
        WorkId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Все сдачи по заданию.
    /// </summary>
    Task<IReadOnlyList<WorkSubmission>> GetByAssignmentAsync(
        string assignmentId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        WorkSubmission work,
        CancellationToken cancellationToken = default);
}