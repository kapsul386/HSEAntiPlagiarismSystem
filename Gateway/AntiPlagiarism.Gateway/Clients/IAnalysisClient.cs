using AntiPlagiarism.Gateway.Models;

namespace AntiPlagiarism.Gateway.Clients;

public interface IAnalysisClient
{
    Task<AnalysisReportDto> SubmitWorkAsync(
        string studentId,
        string assignmentId,
        Guid fileId,
        string contentFingerprint,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AnalysisReportDto>> GetReportsByAssignmentAsync(
        string assignmentId,
        CancellationToken cancellationToken = default);
}