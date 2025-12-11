using AntiPlagiarism.Gateway.Models;

namespace AntiPlagiarism.Gateway.Clients;

/// <summary>
/// Контракт для взаимодействия Gateway с сервисом анализа.
/// </summary>
public interface IAnalysisClient
{
    /// <summary>
    /// Отправить работу на анализ и получить отчёт.
    /// </summary>
    Task<AnalysisReportDto> SubmitWorkAsync(
        string studentId,
        string assignmentId,
        Guid fileId,
        string contentFingerprint,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все отчёты по конкретному заданию.
    /// </summary>
    Task<IReadOnlyList<AnalysisReportDto>> GetReportsByAssignmentAsync(
        string assignmentId,
        CancellationToken cancellationToken = default);
}