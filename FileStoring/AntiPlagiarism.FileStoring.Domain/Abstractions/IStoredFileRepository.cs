using AntiPlagiarism.FileStoring.Domain.Entities;
using AntiPlagiarism.FileStoring.Domain.ValueObjects;

namespace AntiPlagiarism.FileStoring.Domain.Abstractions;

public interface IStoredFileRepository
{
    Task<StoredFile?> GetByIdAsync(
        FileId id,
        CancellationToken cancellationToken = default);
    
    Task AddAsync(
        StoredFile file,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Может потребоваться в будущем — например, при логическом удалении или переносе файла.
    /// </summary>
    Task UpdateAsync(
        StoredFile file,
        CancellationToken cancellationToken = default);
}