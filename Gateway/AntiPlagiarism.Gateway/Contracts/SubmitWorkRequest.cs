using Microsoft.AspNetCore.Http;

namespace AntiPlagiarism.Gateway.Contracts;

public sealed class SubmitWorkRequest
{
    public string StudentId { get; set; } = string.Empty;

    public string AssignmentId { get; set; } = string.Empty;

    public IFormFile File { get; set; } = null!;
}