using System.Net.Http.Json;
using AntiPlagiarism.Gateway.Models;

namespace AntiPlagiarism.Gateway.Clients;

/// <summary>
/// HTTP-клиент для обращения к сервису анализа.
/// </summary>
public sealed class AnalysisClient : IAnalysisClient
{
    private readonly HttpClient _httpClient;

    public AnalysisClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Внутренняя модель запроса к Analysis API
    private sealed class SubmitWorkPayload
    {
        public string StudentId { get; init; } = string.Empty;
        public string AssignmentId { get; init; } = string.Empty;
        public Guid FileId { get; init; }
        public string ContentFingerprint { get; init; } = string.Empty;
    }

    public async Task<AnalysisReportDto> SubmitWorkAsync(
        string studentId,
        string assignmentId,
        Guid fileId,
        string contentFingerprint,
        CancellationToken cancellationToken = default)
    {
        var payload = new SubmitWorkPayload
        {
            StudentId = studentId,
            AssignmentId = assignmentId,
            FileId = fileId,
            ContentFingerprint = contentFingerprint
        };

        using var response = await _httpClient.PostAsJsonAsync(
            "/analysis/works",
            payload,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AnalysisReportDto>(
            cancellationToken: cancellationToken);

        if (result is null)
        {
            throw new InvalidOperationException("Analysis service returned empty response.");
        }

        return result;
    }

    public async Task<IReadOnlyList<AnalysisReportDto>> GetReportsByAssignmentAsync(
        string assignmentId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"/analysis/assignments/{assignmentId}/reports",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<List<AnalysisReportDto>>(
            cancellationToken: cancellationToken);

        return result ?? new List<AnalysisReportDto>();
    }
}
