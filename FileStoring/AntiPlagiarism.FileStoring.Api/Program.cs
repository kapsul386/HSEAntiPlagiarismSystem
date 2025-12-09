using System.IO;
using AntiPlagiarism.FileStoring.Application.Abstractions;
using AntiPlagiarism.FileStoring.Application.Models;
using AntiPlagiarism.FileStoring.Domain.ValueObjects;
using AntiPlagiarism.FileStoring.Infrastructure;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

// OpenAPI (описание API для Swagger/Postman)
builder.Services.AddOpenApi();

// Базовая директория для файлов:
// 1) сначала пытаемся взять из переменной окружения FILESTORING_ROOT
// 2) если её нет — используем локальный путь внутри приложения
var filesRoot = builder.Configuration["FILESTORING_ROOT"];

if (string.IsNullOrWhiteSpace(filesRoot))
{
    filesRoot = Path.Combine(
        builder.Environment.ContentRootPath,
        "data",
        "files");
}

// Регистрируем всё из FileStoring.Infrastructure
builder.Services.AddFileStoring(filesRoot);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // JSON-описание API будет доступно по /openapi/v1.json
    app.MapOpenApi();
}

app.UseHttpsRedirection();

/// <summary>
/// Загрузка файла.
/// </summary>
app.MapPost("/files", async (
        [FromForm] IFormFile file,
        IFileStoringService fileStoringService,
        CancellationToken cancellationToken) =>
    {
        if (file is null || file.Length == 0)
        {
            return Results.BadRequest("File is missing or empty.");
        }

        await using var stream = file.OpenReadStream();

        var model = await fileStoringService.StoreAsync(
            stream,
            file.FileName,
            file.ContentType,
            file.Length,
            cancellationToken);

        return Results.Created($"/files/{model.Id}", model);
    })
    .DisableAntiforgery()
    .WithName("UploadFile")
    .Accepts<IFormFile>("multipart/form-data")
    .Produces<StoredFileModel>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest);

/// <summary>
/// Получить метаданные файла по Id.
/// </summary>
app.MapGet("/files/{id:guid}", async (
        Guid id,
        IFileStoringService fileStoringService,
        CancellationToken cancellationToken) =>
    {
        var fileId = new FileId(id);

        var model = await fileStoringService.GetAsync(fileId, cancellationToken);

        return model is null
            ? Results.NotFound()
            : Results.Ok(model);
    })
    .WithName("GetFileMetadata")
    .Produces<StoredFileModel>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

app.Run();
