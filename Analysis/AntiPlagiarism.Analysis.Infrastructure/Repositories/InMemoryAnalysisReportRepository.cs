using System.Collections.Concurrent;
using AntiPlagiarism.Analysis.Domain.Abstractions;
using AntiPlagiarism.Analysis.Domain.Entities;
using AntiPlagiarism.Analysis.Domain.ValueObjects;

namespace AntiPlagiarism.Analysis.Infrastructure.Repositories;

/// <summary>
/// In-memory реализация репозитория отчётов.
/// Подходит для разработки и тестов.
/// </summary>
public sealed class InMemoryAnalysisReportRepository : IAnalysisReportRepository
{
    private readonly ConcurrentDictionary<ReportId, AnalysisReport> _storage = new();

    // Индекс для быстрого поиска по assignmentId
    // List используется для учебного примера (не потокобезопасно)
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

        var assignmentId = report.AssignmentId;

        var list = _byAssignment.GetOrAdd(assignmentId, _ => new List<ReportId>());
        list.Add(report.Id);

        return Task.CompletedTask;
    }
}
