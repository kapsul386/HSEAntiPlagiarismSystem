using System.IO;
using AntiPlagiarism.FileStoring.Api.Endpoints;
using AntiPlagiarism.FileStoring.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

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

// Эндпоинты сервиса хранения файлов
app.MapFileStoringEndpoints();

app.Run();