using System.IO;

namespace AntiPlagiarism.FileStoring.Application.Abstractions;

/// <summary>
/// Абстракция над физическим хранилищем файлов (диск, S3, MinIO и т.п.).
/// </summary>
public interface IFileStorage
{
    /// <summary>
    /// Сохранить содержимое файла и вернуть storage key
    /// (относительный путь или ключ в объектном хранилище).
    /// </summary>
    Task<string> SaveAsync(
        Stream content,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Открыть файл для чтения по storage key.
    /// </summary>
    Task<Stream> OpenReadAsync(
        string storageKey,
        CancellationToken cancellationToken = default);
}
