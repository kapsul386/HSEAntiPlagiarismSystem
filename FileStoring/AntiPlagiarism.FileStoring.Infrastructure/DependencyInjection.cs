using AntiPlagiarism.FileStoring.Application.Abstractions;
using AntiPlagiarism.FileStoring.Application.Services;
using AntiPlagiarism.FileStoring.Domain.Abstractions;
using AntiPlagiarism.FileStoring.Infrastructure.FileStorage;
using AntiPlagiarism.FileStoring.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AntiPlagiarism.FileStoring.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Регистрирует все зависимости сервиса хранения файлов.
    /// </summary>
    public static IServiceCollection AddFileStoring(
        this IServiceCollection services,
        string baseDirectory)
    {
        if (string.IsNullOrWhiteSpace(baseDirectory))
            throw new ArgumentException("Base directory must be provided", nameof(baseDirectory));

        // Физическое хранилище файлов
        services.AddSingleton<IFileStorage>(_ => new FileSystemFileStorage(baseDirectory));

        // Хранилище метаданных (пока in-memory)
        services.AddSingleton<IStoredFileRepository, InMemoryStoredFileRepository>();

        // Application-сервис
        services.AddScoped<IFileStoringService, FileStoringService>();

        return services;
    }
}