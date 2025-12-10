namespace AntiPlagiarism.Analysis.Application.Models;

/// <summary>
/// Результат анализа конкретной сдачи.
/// </summary>
public sealed class AnalysisReportModel
{
    public Guid Id { get; init; }

    /// <summary>Связанная сдача работы.</summary>
    public Guid WorkId { get; init; }

    public string AssignmentId { get; init; } = string.Empty;

    /// <summary>Признак обнаружения плагиата.</summary>
    public bool IsPlagiarism { get; init; }

    /// <summary>Если плагиат — студент, у которого совпала работа.</summary>
    public string? PlagiarismSourceStudentId { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    /// <summary>Момент завершения анализа, если требуется асинхронность.</summary>
    public DateTime? CompletedAtUtc { get; init; }
}