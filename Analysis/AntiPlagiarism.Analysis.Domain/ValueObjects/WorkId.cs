namespace AntiPlagiarism.Analysis.Domain.ValueObjects;

public readonly struct WorkId
{
    public Guid Value { get; }

    public WorkId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Work id must be non-empty", nameof(value));
        }

        Value = value;
    }

    public static WorkId NewWorkId()
    {
        return new WorkId(Guid.NewGuid());
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}