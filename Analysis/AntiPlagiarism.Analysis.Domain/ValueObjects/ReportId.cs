namespace AntiPlagiarism.Analysis.Domain.ValueObjects;

public readonly struct ReportId
{
    public Guid Value { get; }

    public ReportId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Report id must be non-empty", nameof(value));
        }

        Value = value;
    }

    public static ReportId NewReportId()
    {
        return new ReportId(Guid.NewGuid());
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}