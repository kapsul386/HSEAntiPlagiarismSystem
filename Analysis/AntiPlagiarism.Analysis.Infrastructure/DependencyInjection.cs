using AntiPlagiarism.Analysis.Application.Abstractions;
using AntiPlagiarism.Analysis.Application.Services;
using AntiPlagiarism.Analysis.Domain.Abstractions;
using AntiPlagiarism.Analysis.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AntiPlagiarism.Analysis.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAnalysis(this IServiceCollection services)
    {
        services.AddSingleton<IWorkRepository, InMemoryWorkRepository>();
        services.AddSingleton<IAnalysisReportRepository, InMemoryAnalysisReportRepository>();

        services.AddScoped<IAnalysisService, AnalysisService>();

        return services;
    }
}