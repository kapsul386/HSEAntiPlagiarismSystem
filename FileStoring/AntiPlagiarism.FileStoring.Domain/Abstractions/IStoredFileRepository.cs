using AntiPlagiarism.FileStoring.Domain.Entities;
using AntiPlagiarism.FileStoring.Domain.ValueObjects;

namespace AntiPlagiarism.FileStoring.Domain.Abstractions;

public interface IStoredFileRepository
{
    Task<StoredFile?> GetByIdAsync(FileId id, CancellationToken cancellationToken = default);
    
    Task AddAsync(StoredFile file, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Обновление может пригодиться, если захотим помечать файл удалённым или переносить.
    /// </summary>
    Task UpdateAsync(StoredFile file, CancellationToken cancellationToken = default);
}