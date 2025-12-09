namespace AntiPlagiarism.Gateway.Models;

public sealed class WorkSubmissionResultDto
{
    public StoredFileDto File { get; init; } = null!;

    public AnalysisReportDto Report { get; init; } = null!;
}