using AntiPlagiarism.Analysis.Application.Models;

namespace AntiPlagiarism.Analysis.Application.Abstractions;

/// <summary>
/// Сервис анализа работ на плагиат.
/// Инкапсулирует бизнес-логику вокруг сдач и отчётов.
/// </summary>
public interface IAnalysisService
{
    /// <summary>
    /// Регистрирует сдачу работы и сразу выполняет анализ.
    /// </summary>
    Task<AnalysisReportModel> SubmitAndAnalyzeAsync(
        string studentId,
        string assignmentId,
        Guid fileId,
        string contentFingerprint,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает все отчёты по заданию.
    /// </summary>
    Task<IReadOnlyList<AnalysisReportModel>> GetReportsByAssignmentAsync(
        string assignmentId,
        CancellationToken cancellationToken = default);
}