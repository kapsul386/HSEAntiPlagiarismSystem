using AntiPlagiarism.Analysis.Api.Contracts;
using AntiPlagiarism.Analysis.Application.Abstractions;
using AntiPlagiarism.Analysis.Application.Models;
using AntiPlagiarism.Analysis.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
//Эндпоинты вынести в отдельный класс надо будет
builder.Services.AddOpenApi();

// Регистрируем слой Analysis (репозитории + сервис)
builder.Services.AddAnalysis();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

/// <summary>
/// Сдать работу и сразу получить отчёт по плагиату.
/// Это то, что будет вызывать Gateway после загрузки файла.
/// </summary>
app.MapPost("/analysis/works", async (
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
    .WithName("SubmitWorkAndAnalyze")
    .Produces<AnalysisReportModel>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest);

/// <summary>
/// Получить отчёты по всем работам для конкретного задания.
/// Это то, что будет использовать преподаватель.
/// </summary>
app.MapGet("/analysis/assignments/{assignmentId}/reports", async (
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
    .WithName("GetReportsByAssignment")
    .Produces<List<AnalysisReportModel>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest);

app.Run();
