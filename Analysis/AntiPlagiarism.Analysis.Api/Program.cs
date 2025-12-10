using AntiPlagiarism.Analysis.Api.Endpoints;
using AntiPlagiarism.Analysis.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddAnalysis();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Подключаем группу эндпоинтов анализа
app.MapAnalysisEndpoints();

app.Run();