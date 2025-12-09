using AntiPlagiarism.Analysis.Domain.Entities;
using AntiPlagiarism.Analysis.Domain.ValueObjects;

namespace AntiPlagiarism.Analysis.Domain.Abstractions;

public interface IAnalysisReportRepository
{
    Task<AnalysisReport?> GetByIdAsync(
        ReportId id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AnalysisReport>> GetByAssignmentAsync(
        string assignmentId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        AnalysisReport report,
        CancellationToken cancellationToken = default);
}