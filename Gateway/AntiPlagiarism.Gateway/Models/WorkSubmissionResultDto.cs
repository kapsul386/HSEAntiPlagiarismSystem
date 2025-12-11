namespace AntiPlagiarism.Gateway.Models;

/// <summary>
/// Итоговый результат отправки работы: сохранённый файл + отчёт анализа.
/// </summary>
public sealed class WorkSubmissionResultDto
{
    public StoredFileDto File { get; init; } = null!;

    public AnalysisReportDto Report { get; init; } = null!;
}