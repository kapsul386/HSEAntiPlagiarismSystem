using System.Security.Cryptography;
using AntiPlagiarism.Gateway.Clients;
using AntiPlagiarism.Gateway.Contracts;
using AntiPlagiarism.Gateway.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// URLs для сервисов — дефолты, переопределяются env переменными
var fileStoringBaseUrl = builder.Configuration["FILESTORING_URL"] ?? "http://filestoring:8080";
var analysisBaseUrl = builder.Configuration["ANALYSIS_URL"] ?? "http://analysis:8080";

// Регистрируем HttpClient для FileStoring
builder.Services.AddHttpClient<IFileStoringClient, FileStoringClient>(client =>
{
    client.BaseAddress = new Uri(fileStoringBaseUrl);
});

// Регистрируем HttpClient для Analysis
builder.Services.AddHttpClient<IAnalysisClient, AnalysisClient>(client =>
{
    client.BaseAddress = new Uri(analysisBaseUrl);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

/// <summary>
/// Upload file + register work + run plagiarism analysis.
/// </summary>
app.MapPost("/works", async (
        [FromForm] SubmitWorkRequest request,
        IFileStoringClient fileStoringClient,
        IAnalysisClient analysisClient,
        CancellationToken cancellationToken) =>
    {
        try
        {
            if (request.File is null || request.File.Length == 0)
            {
                return Results.BadRequest("File is missing or empty.");
            }

            if (string.IsNullOrWhiteSpace(request.StudentId) ||
                string.IsNullOrWhiteSpace(request.AssignmentId))
            {
                return Results.BadRequest("StudentId and AssignmentId are required.");
            }

            // 1. Save file to FileStoring
            var storedFile = await fileStoringClient.UploadFileAsync(
                request.File,
                cancellationToken);

            // 2. Fingerprint calculation (SHA-256 hash)
            string fingerprint;
            await using (var stream = request.File.OpenReadStream())
            {
                using var sha = SHA256.Create();
                var hash = await sha.ComputeHashAsync(stream, cancellationToken);
                fingerprint = Convert.ToHexString(hash);
            }

            // 3. Submit for analysis
            var report = await analysisClient.SubmitWorkAsync(
                request.StudentId,
                request.AssignmentId,
                storedFile.Id,
                fingerprint,
                cancellationToken);

            var result = new WorkSubmissionResultDto
            {
                File = storedFile,
                Report = report
            };

            return Results.Ok(result);
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


/// <summary>
/// Get analysis results for assignment.
/// </summary>
app.MapGet("/works/{assignmentId}/reports", async (
        string assignmentId,
        IAnalysisClient analysisClient,
        CancellationToken cancellationToken) =>
    {
        try
        {
            if (string.IsNullOrWhiteSpace(assignmentId))
            {
                return Results.BadRequest("AssignmentId is required.");
            }

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

app.Run();
