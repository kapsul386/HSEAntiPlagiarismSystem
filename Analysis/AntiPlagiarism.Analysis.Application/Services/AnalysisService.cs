using AntiPlagiarism.Analysis.Application.Abstractions;
using AntiPlagiarism.Analysis.Application.Models;
using AntiPlagiarism.Analysis.Domain.Abstractions;
using AntiPlagiarism.Analysis.Domain.Entities;
using AntiPlagiarism.Analysis.Domain.ValueObjects;

namespace AntiPlagiarism.Analysis.Application.Services;

public sealed class AnalysisService : IAnalysisService
{
    private readonly IWorkRepository _workRepository;
    private readonly IAnalysisReportRepository _reportRepository;

    public AnalysisService(
        IWorkRepository workRepository,
        IAnalysisReportRepository reportRepository)
    {
        _workRepository = workRepository;
        _reportRepository = reportRepository;
    }

    public async Task<AnalysisReportModel> SubmitAndAnalyzeAsync(
        string studentId,
        string assignmentId,
        Guid fileId,
        string contentFingerprint,
        CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;

        var work = WorkSubmission.CreateNew(
            studentId,
            assignmentId,
            fileId,
            contentFingerprint,
            nowUtc);

        await _workRepository.AddAsync(work, cancellationToken);

        var previousWorks = await _workRepository.GetByAssignmentAsync(
            assignmentId,
            cancellationToken);

        var plagiarismSource = previousWorks
            .Where(w => w.Id.Value != work.Id.Value)
            .Where(w => w.ContentFingerprint == contentFingerprint)
            .OrderBy(w => w.SubmittedAtUtc)
            .FirstOrDefault(w => w.StudentId != studentId);

        var isPlagiarism = plagiarismSource != null;

        var report = AnalysisReport.CreateCompleted(
            work.Id,
            isPlagiarism,
            plagiarismSource?.StudentId,
            nowUtc);

        await _reportRepository.AddAsync(report, cancellationToken);

        return new AnalysisReportModel
        {
            Id = report.Id.Value,
            WorkId = work.Id.Value,
            IsPlagiarism = report.IsPlagiarism,
            PlagiarismSourceStudentId = report.PlagiarismSourceStudentId,
            CreatedAtUtc = report.CreatedAtUtc,
            CompletedAtUtc = report.CompletedAtUtc
        };
    }

    public async Task<IReadOnlyList<AnalysisReportModel>> GetReportsByAssignmentAsync(
        string assignmentId,
        CancellationToken cancellationToken = default)
    {
        var reports = await _reportRepository.GetByAssignmentAsync(
            assignmentId,
            cancellationToken);

        var result = reports
            .Select(r => new AnalysisReportModel
            {
                Id = r.Id.Value,
                WorkId = r.WorkId.Value,
                IsPlagiarism = r.IsPlagiarism,
                PlagiarismSourceStudentId = r.PlagiarismSourceStudentId,
                CreatedAtUtc = r.CreatedAtUtc,
                CompletedAtUtc = r.CompletedAtUtc
            })
            .ToArray();

        return result;
    }
}
