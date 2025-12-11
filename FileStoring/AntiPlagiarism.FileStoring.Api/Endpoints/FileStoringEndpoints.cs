using AntiPlagiarism.FileStoring.Application.Abstractions;
using AntiPlagiarism.FileStoring.Application.Models;
using AntiPlagiarism.FileStoring.Domain.ValueObjects;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace AntiPlagiarism.FileStoring.Api.Endpoints;

public static class FileStoringEndpoints
{
    /// <summary>
    /// Группировка всех эндпоинтов сервиса хранения файлов.
    /// </summary>
    public static void MapFileStoringEndpoints(this IEndpointRouteBuilder endpoints)
    {
        /// <summary>
        /// Загрузка файла.
        /// </summary>
        endpoints.MapPost("/files", async (
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
        endpoints.MapGet("/files/{id:guid}", async (
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
    }
}
