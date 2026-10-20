namespace MockApi.Core.Generation;

/// <summary>
/// Runtime knobs passed to <see cref="JsonGenerator"/>. List sizes are optional: when they are
/// absent the schema node's own <c>length</c> is used, which is the core-only behavior. Pagination
/// is deliberately not modeled here yet.
/// </summary>
public sealed record GenerationOptions
{
    public int? ListMin { get; init; }

    public int? ListMax { get; init; }

    public int MaxValues { get; init; } = 100_000;

    public DateTimeOffset? ReferenceTime { get; init; }
}

public sealed class ResponseTooLargeException : Exception
{
    public ResponseTooLargeException(int maxValues)
        : base($"Generation exceeded the {maxValues} generated-value limit.")
    {
        MaxValues = maxValues;
    }

    public int MaxValues { get; }
}
