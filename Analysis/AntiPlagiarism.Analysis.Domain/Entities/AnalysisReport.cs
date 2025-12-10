using AntiPlagiarism.Analysis.Domain.Enums;
using AntiPlagiarism.Analysis.Domain.ValueObjects;

namespace AntiPlagiarism.Analysis.Domain.Entities;

/// <summary>
/// Доменная сущность отчёта анализа.
/// </summary>
public sealed class AnalysisReport
{
    public ReportId Id { get; }

    public WorkId WorkId { get; private set; }

    /// <summary>Задание, в рамках которого сдана работа.</summary>
    public string AssignmentId { get; private set; }

    public bool IsPlagiarism { get; private set; }

    public string? PlagiarismSourceStudentId { get; private set; }

    public ReportStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }

    private AnalysisReport(
        ReportId id,
        WorkId workId,
        string assignmentId,
        bool isPlagiarism,
        string? plagiarismSourceStudentId,
        ReportStatus status,
        DateTime createdAtUtc,
        DateTime? completedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(assignmentId))
        {
            throw new ArgumentException("Assignment id must be non-empty", nameof(assignmentId));
        }

        Id = id;
        WorkId = workId;
        AssignmentId = assignmentId;
        IsPlagiarism = isPlagiarism;
        PlagiarismSourceStudentId = plagiarismSourceStudentId;
        Status = status;
        CreatedAtUtc = createdAtUtc;
        CompletedAtUtc = completedAtUtc;
    }

    /// <summary>
    /// Финализированный отчёт (текущая версия анализа синхронная).
    /// </summary>
    public static AnalysisReport CreateCompleted(
        WorkId workId,
        string assignmentId,
        bool isPlagiarism,
        string? plagiarismSourceStudentId,
        DateTime completedAtUtc)
    {
        var id = ReportId.NewReportId();

        return new AnalysisReport(
            id,
            workId,
            assignmentId,
            isPlagiarism,
            plagiarismSourceStudentId,
            ReportStatus.Completed,
            completedAtUtc,
            completedAtUtc);
    }
}
