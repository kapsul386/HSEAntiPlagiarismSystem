namespace AntiPlagiarism.Analysis.Application.Models;

public sealed class AnalysisReportModel
{
    public Guid Id { get; init; }

    public Guid WorkId { get; init; }

    public bool IsPlagiarism { get; init; }

    public string? PlagiarismSourceStudentId { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? CompletedAtUtc { get; init; }
}