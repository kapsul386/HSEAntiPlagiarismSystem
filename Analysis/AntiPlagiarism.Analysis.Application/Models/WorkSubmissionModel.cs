namespace AntiPlagiarism.Analysis.Application.Models;

public sealed class WorkSubmissionModel
{
    public Guid Id { get; init; }

    public string StudentId { get; init; } = string.Empty;

    public string AssignmentId { get; init; } = string.Empty;

    public Guid FileId { get; init; }

    public string ContentFingerprint { get; init; } = string.Empty;

    public DateTime SubmittedAtUtc { get; init; }
}