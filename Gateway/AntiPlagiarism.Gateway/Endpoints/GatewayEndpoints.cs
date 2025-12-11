using System.Security.Cryptography;
using AntiPlagiarism.Gateway.Clients;
using AntiPlagiarism.Gateway.Contracts;
using AntiPlagiarism.Gateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace AntiPlagiarism.Gateway.Endpoints;

/// <summary>
/// Эндпоинты API Gateway.
/// </summary>
public static class GatewayEndpoints
{
    public static void MapGatewayEndpoints(this IEndpointRouteBuilder endpoints)
    {
        MapSubmitWork(endpoints);
        MapGetReports(endpoints);
    }

    private static void MapSubmitWork(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/works", async (
                [FromForm] SubmitWorkRequest request,
                IFileStoringClient fileStoringClient,
                IAnalysisClient analysisClient,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    if (request.File is null || request.File.Length == 0)
                        return Results.BadRequest("File is missing or empty.");

                    if (string.IsNullOrWhiteSpace(request.StudentId) ||
                        string.IsNullOrWhiteSpace(request.AssignmentId))
                        return Results.BadRequest("StudentId and AssignmentId are required.");

                    // 1. Сохраняем файл в FileStoring
                    var storedFile = await fileStoringClient.UploadFileAsync(
                        request.File,
                        cancellationToken);

                    // 2. Вычисляем SHA-256 отпечаток
                    string fingerprint;
                    await using (var stream = request.File.OpenReadStream())
                    {
                        using var sha = SHA256.Create();
                        var hash = await sha.ComputeHashAsync(stream, cancellationToken);
                        fingerprint = Convert.ToHexString(hash);
                    }

                    // 3. Передаём работу в Analysis
                    var report = await analysisClient.SubmitWorkAsync(
                        request.StudentId,
                        request.AssignmentId,
                        storedFile.Id,
                        fingerprint,
                        cancellationToken);

                    return Results.Ok(new WorkSubmissionResultDto
                    {
                        File = storedFile,
                        Report = report
                    });
                }
                catch (HttpRequestException ex)
                {
                    return Results.Problem(
                        title: "Downstream service unavailable",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status503ServiceUnavailable);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Unexpected error while submitting work",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .DisableAntiforgery()
            .WithName("SubmitWork")
            .Produces<WorkSubmissionResultDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status503ServiceUnavailable)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static void MapGetReports(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/works/{assignmentId}/reports", async (
                string assignmentId,
                IAnalysisClient analysisClient,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(assignmentId))
                        return Results.BadRequest("AssignmentId is required.");

                    var reports = await analysisClient.GetReportsByAssignmentAsync(
                        assignmentId,
                        cancellationToken);

                    return Results.Ok(reports);
                }
                catch (HttpRequestException ex)
                {
                    return Results.Problem(
                        title: "Downstream service unavailable",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status503ServiceUnavailable);
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "Unexpected error while fetching reports",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetReportsByAssignmentViaGateway")
            .Produces<List<AnalysisReportDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status503ServiceUnavailable)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}
