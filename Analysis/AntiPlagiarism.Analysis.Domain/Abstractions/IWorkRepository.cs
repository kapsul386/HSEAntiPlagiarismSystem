using AntiPlagiarism.Analysis.Domain.Entities;
using AntiPlagiarism.Analysis.Domain.ValueObjects;

namespace AntiPlagiarism.Analysis.Domain.Abstractions;

public interface IWorkRepository
{
    Task<WorkSubmission?> GetByIdAsync(
        WorkId id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkSubmission>> GetByAssignmentAsync(
        string assignmentId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        WorkSubmission work,
        CancellationToken cancellationToken = default);
}