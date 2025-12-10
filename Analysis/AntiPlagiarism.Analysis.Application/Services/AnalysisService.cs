using AntiPlagiarism.Analysis.Application.Abstractions;
using AntiPlagiarism.Analysis.Application.Models;
using AntiPlagiarism.Analysis.Domain.Abstractions;
using AntiPlagiarism.Analysis.Domain.Entities;

namespace AntiPlagiarism.Analysis.Application.Services;

/// <summary>
/// Бизнес-логика анализа работ.
/// </summary>
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

        // Фиксируем сдачу
        var work = WorkSubmission.CreateNew(
            studentId,
            assignmentId,
            fileId,
            contentFingerprint,
            nowUtc);

        await _workRepository.AddAsync(work, cancellationToken);

        // Ищем совпавшие работы в рамках задания
        var previousWorks = await _workRepository.GetByAssignmentAsync(
            assignmentId,
            cancellationToken);

        var plagiarismSource = previousWorks
            .Where(w => w.ContentFingerprint == contentFingerprint)
            .OrderBy(w => w.SubmittedAtUtc)
            .FirstOrDefault(w => w.StudentId != studentId);

        var report = AnalysisReport.CreateCompleted(
            work.Id,
            work.AssignmentId,
            plagiarismSource is not null,
            plagiarismSource?.StudentId,
            nowUtc);

        await _reportRepository.AddAsync(report, cancellationToken);

        return new AnalysisReportModel
        {
            Id = report.Id.Value,
            WorkId = work.Id.Value,
            AssignmentId = work.AssignmentId,
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

        return reports
            .Select(r => new AnalysisReportModel
            {
                Id = r.Id.Value,
                WorkId = r.WorkId.Value,
                AssignmentId = r.AssignmentId,
                IsPlagiarism = r.IsPlagiarism,
                PlagiarismSourceStudentId = r.PlagiarismSourceStudentId,
                CreatedAtUtc = r.CreatedAtUtc,
                CompletedAtUtc = r.CompletedAtUtc
            })
            .ToArray();
    }
}
