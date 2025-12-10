using System.Collections.Concurrent;
using AntiPlagiarism.Analysis.Domain.Abstractions;
using AntiPlagiarism.Analysis.Domain.Entities;
using AntiPlagiarism.Analysis.Domain.ValueObjects;

namespace AntiPlagiarism.Analysis.Infrastructure.Repositories;

public sealed class InMemoryAnalysisReportRepository : IAnalysisReportRepository
{
    private readonly ConcurrentDictionary<ReportId, AnalysisReport> _storage = new();
    private readonly ConcurrentDictionary<string, List<ReportId>> _byAssignment = new();

    public Task<AnalysisReport?> GetByIdAsync(
        ReportId id,
        CancellationToken cancellationToken = default)
    {
        _storage.TryGetValue(id, out var report);
        return Task.FromResult(report);
    }

    public Task<IReadOnlyList<AnalysisReport>> GetByAssignmentAsync(
        string assignmentId,
        CancellationToken cancellationToken = default)
    {
        if (!_byAssignment.TryGetValue(assignmentId, out var ids))
        {
            return Task.FromResult<IReadOnlyList<AnalysisReport>>(Array.Empty<AnalysisReport>());
        }

        var result = ids
            .Select(id => _storage[id])
            .OrderBy(r => r.CreatedAtUtc)
            .ToArray();

        return Task.FromResult<IReadOnlyList<AnalysisReport>>(result);
    }

    public Task AddAsync(
        AnalysisReport report,
        CancellationToken cancellationToken = default)
    {
        if (!_storage.TryAdd(report.Id, report))
        {
            throw new InvalidOperationException($"Report with id {report.Id.Value} already exists.");
        }

        // Теперь индексируем по настоящему assignmentId
        var assignmentId = report.AssignmentId;

        var list = _byAssignment.GetOrAdd(assignmentId, _ => new List<ReportId>());
        list.Add(report.Id);

        return Task.CompletedTask;
    }
}