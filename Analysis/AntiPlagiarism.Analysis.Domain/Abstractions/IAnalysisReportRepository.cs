using AntiPlagiarism.Analysis.Domain.Entities;
using AntiPlagiarism.Analysis.Domain.ValueObjects;

namespace AntiPlagiarism.Analysis.Domain.Abstractions;

/// <summary>
/// Хранилище отчётов анализа.
/// </summary>
public interface IAnalysisReportRepository
{
    Task<AnalysisReport?> GetByIdAsync(
        ReportId id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Все отчёты в рамках одного задания.
    /// </summary>
    Task<IReadOnlyList<AnalysisReport>> GetByAssignmentAsync(
        string assignmentId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        AnalysisReport report,
        CancellationToken cancellationToken = default);
}