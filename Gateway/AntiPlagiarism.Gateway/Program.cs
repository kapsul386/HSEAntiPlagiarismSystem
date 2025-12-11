using AntiPlagiarism.Gateway.Clients;
using AntiPlagiarism.Gateway.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Адреса сервисов
var fileStoringBaseUrl = builder.Configuration["FILESTORING_URL"] ?? "http://filestoring:8080";
var analysisBaseUrl = builder.Configuration["ANALYSIS_URL"] ?? "http://analysis:8080";

builder.Services.AddHttpClient<IFileStoringClient, FileStoringClient>(c =>
{
    c.BaseAddress = new Uri(fileStoringBaseUrl);
});
builder.Services.AddHttpClient<IAnalysisClient, AnalysisClient>(c =>
{
    c.BaseAddress = new Uri(analysisBaseUrl);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGatewayEndpoints();

app.Run();