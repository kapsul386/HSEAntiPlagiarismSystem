using System.Security.Cryptography;
using System.Text;
using AntiPlagiarism.Gateway.Clients;
using AntiPlagiarism.Gateway.Contracts;
using AntiPlagiarism.Gateway.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Base URLs для FileStoring и Analysis.
// В dev можно задать через appsettings.json или env-переменные.
// Здесь задаём дефолты, которые можно переопределить env-ами.
var fileStoringBaseUrl = builder.Configuration["FILESTORING_URL"] ?? "http://localhost:5014";
var analysisBaseUrl    = builder.Configuration["ANALYSIS_URL"]     ?? "http://localhost:5189";

builder.Services.AddHttpClient<IFileStoringClient, FileStoringClient>(client =>
{
    client.BaseAddress = new Uri(fileStoringBaseUrl);
});

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
/// Сдать работу: файл + данные студента.
/// Gateway сам:
/// 1) сохраняет файл через FileStoring
/// 2) считает fingerprint
/// 3) вызывает Analysis
/// 4) возвращает комбинированный результат
/// </summary>
app.MapPost("/works", async (
        [FromForm] SubmitWorkRequest request,
        IFileStoringClient fileStoringClient,
        IAnalysisClient analysisClient,
        CancellationToken cancellationToken) =>
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

        // 1. Сохраняем файл в FileStoring
        var storedFile = await fileStoringClient.UploadFileAsync(
            request.File,
            cancellationToken);

        // 2. Считаем fingerprint (SHA-256 по содержимому файла)
        string fingerprint;
        await using (var stream = request.File.OpenReadStream())
        {
            using var sha = SHA256.Create();
            var hash = await sha.ComputeHashAsync(stream, cancellationToken);
            fingerprint = Convert.ToHexString(hash); // AABBCC...
        }

        // 3. Отправляем в Analysis
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
    })
    .DisableAntiforgery() // для multipart формы
    .WithName("SubmitWork")
    .Produces<WorkSubmissionResultDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest);

/// <summary>
/// Получить отчёты по конкретному заданию.
/// </summary>
app.MapGet("/works/{assignmentId}/reports", async (
        string assignmentId,
        IAnalysisClient analysisClient,
        CancellationToken cancellationToken) =>
    {
        if (string.IsNullOrWhiteSpace(assignmentId))
        {
            return Results.BadRequest("AssignmentId is required.");
        }

        var reports = await analysisClient.GetReportsByAssignmentAsync(
            assignmentId,
            cancellationToken);

        return Results.Ok(reports);
    })
    .WithName("GetReportsByAssignmentViaGateway")
    .Produces<List<AnalysisReportDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest);

app.Run();
