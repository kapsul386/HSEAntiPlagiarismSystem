namespace AntiPlagiarism.Analysis.Api.Contracts;

/// <summary>
/// Модель запроса на анализ работы.
/// </summary>
public sealed class SubmitWorkRequest
{
    /// <summary>Студент, отправивший работу.</summary>
    public string StudentId { get; init; } = string.Empty;

    /// <summary>Задание, к которому относится работа.</summary>
    public string AssignmentId { get; init; } = string.Empty;

    /// <summary>Файл из FileStoring.</summary>
    public Guid FileId { get; init; }

    /// <summary>SHA-256 хеш содержимого.</summary>
    public string ContentFingerprint { get; init; } = string.Empty;
}