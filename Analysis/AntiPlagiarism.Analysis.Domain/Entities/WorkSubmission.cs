using AntiPlagiarism.Analysis.Domain.ValueObjects;

namespace AntiPlagiarism.Analysis.Domain.Entities;

/// <summary>
/// Доменная сущность отправленной работы.
/// </summary>
public sealed class WorkSubmission
{
    public WorkId Id { get; }

    public string StudentId { get; private set; }

    public string AssignmentId { get; private set; }

    public Guid FileId { get; private set; }

    /// <summary>SHA-256 отпечаток файла.</summary>
    public string ContentFingerprint { get; private set; }

    public DateTime SubmittedAtUtc { get; private set; }

    private WorkSubmission(
        WorkId id,
        string studentId,
        string assignmentId,
        Guid fileId,
        string contentFingerprint,
        DateTime submittedAtUtc)
    {
        Id = id;
        StudentId = studentId;
        AssignmentId = assignmentId;
        FileId = fileId;
        ContentFingerprint = contentFingerprint;
        SubmittedAtUtc = submittedAtUtc;
    }

    /// <summary>
    /// Создание новой сдачи с проверкой инвариантов.
    /// </summary>
    public static WorkSubmission CreateNew(
        string studentId,
        string assignmentId,
        Guid fileId,
        string contentFingerprint,
        DateTime submittedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(studentId))
            throw new ArgumentException("Student id is required", nameof(studentId));

        if (string.IsNullOrWhiteSpace(assignmentId))
            throw new ArgumentException("Assignment id is required", nameof(assignmentId));

        if (string.IsNullOrWhiteSpace(contentFingerprint))
            throw new ArgumentException("Content fingerprint is required", nameof(contentFingerprint));

        if (fileId == Guid.Empty)
            throw new ArgumentException("File id must be non-empty", nameof(fileId));

        var id = WorkId.NewWorkId();

        return new WorkSubmission(
            id,
            studentId,
            assignmentId,
            fileId,
            contentFingerprint,
            submittedAtUtc);
    }
}
