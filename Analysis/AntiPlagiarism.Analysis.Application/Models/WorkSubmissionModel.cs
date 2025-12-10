namespace AntiPlagiarism.Analysis.Application.Models;

/// <summary>
/// Фиксация отправленной студентом работы.
/// </summary>
public sealed class WorkSubmissionModel
{
    public Guid Id { get; init; }

    public string StudentId { get; init; } = string.Empty;

    public string AssignmentId { get; init; } = string.Empty;

    /// <summary>Файл, сохранённый в FileStoring.</summary>
    public Guid FileId { get; init; }

    /// <summary>SHA-256 отпечаток файла.</summary>
    public string ContentFingerprint { get; init; } = string.Empty;

    public DateTime SubmittedAtUtc { get; init; }
}