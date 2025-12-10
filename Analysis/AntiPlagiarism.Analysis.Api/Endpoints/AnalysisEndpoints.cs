using AntiPlagiarism.Analysis.Api.Contracts;
using AntiPlagiarism.Analysis.Application.Abstractions;
using AntiPlagiarism.Analysis.Application.Models;

namespace AntiPlagiarism.Analysis.Api.Endpoints;

public static class AnalysisEndpoints
{
    /// <summary>
    /// Группировка всех эндпоинтов анализа.
    /// </summary>
    public static void MapAnalysisEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/analysis/works", async (
                SubmitWorkRequest request,
                IAnalysisService analysisService,
                CancellationToken cancellationToken) =>
            {
                if (string.IsNullOrWhiteSpace(request.StudentId) ||
                    string.IsNullOrWhiteSpace(request.AssignmentId) ||
                    request.FileId == Guid.Empty ||
                    string.IsNullOrWhiteSpace(request.ContentFingerprint))
                {
                    return Results.BadRequest("Invalid request payload.");
                }

                var report = await analysisService.SubmitAndAnalyzeAsync(
                    request.StudentId,
                    request.AssignmentId,
                    request.FileId,
                    request.ContentFingerprint,
                    cancellationToken);

                return Results.Ok(report);
            })
            .WithName("SubmitWorkAndAnalyze");

        endpoints.MapGet("/analysis/assignments/{assignmentId}/reports", async (
                string assignmentId,
                IAnalysisService analysisService,
                CancellationToken cancellationToken) =>
            {
                if (string.IsNullOrWhiteSpace(assignmentId))
                {
                    return Results.BadRequest("Assignment id is required.");
                }

                var reports = await analysisService.GetReportsByAssignmentAsync(
                    assignmentId,
                    cancellationToken);

                return Results.Ok(reports);
            })
            .WithName("GetReportsByAssignment");
    }
}
