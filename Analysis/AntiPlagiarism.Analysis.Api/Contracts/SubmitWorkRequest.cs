namespace AntiPlagiarism.Analysis.Api.Contracts;

public sealed class SubmitWorkRequest
{
    public string StudentId { get; init; } = string.Empty;

    public string AssignmentId { get; init; } = string.Empty;

    public Guid FileId { get; init; }

    public string ContentFingerprint { get; init; } = string.Empty;
}