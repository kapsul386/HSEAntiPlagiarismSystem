using AntiPlagiarism.Analysis.Application.Abstractions;
using AntiPlagiarism.Analysis.Application.Services;
using AntiPlagiarism.Analysis.Domain.Abstractions;
using AntiPlagiarism.Analysis.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AntiPlagiarism.Analysis.Infrastructure;

/// <summary>
/// Регистрация сервисов анализа в DI.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddAnalysis(this IServiceCollection services)
    {
        // In-memory репозитории (для разработки)
        services.AddSingleton<IWorkRepository, InMemoryWorkRepository>();
        services.AddSingleton<IAnalysisReportRepository, InMemoryAnalysisReportRepository>();

        // Бизнес-логика анализа
        services.AddScoped<IAnalysisService, AnalysisService>();

        return services;
    }
}