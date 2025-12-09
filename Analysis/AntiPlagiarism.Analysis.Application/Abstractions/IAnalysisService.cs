using AntiPlagiarism.Analysis.Application.Models;

namespace AntiPlagiarism.Analysis.Application.Abstractions;

public interface IAnalysisService
{
    Task<AnalysisReportModel> SubmitAndAnalyzeAsync(
        string studentId,
        string assignmentId,
        Guid fileId,
        string contentFingerprint,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AnalysisReportModel>> GetReportsByAssignmentAsync(
        string assignmentId,
        CancellationToken cancellationToken = default);
}