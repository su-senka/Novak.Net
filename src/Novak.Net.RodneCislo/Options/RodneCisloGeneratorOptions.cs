using Novak.Net.Common.Options;
using Novak.Net.RodneCislo.Models;

namespace Novak.Net.RodneCislo.Options;

public enum RodneCisloFormat { WithSlash, WithoutSlash }

public sealed class RodneCisloGeneratorOptions : ISeededOptions
{
    public int? Seed { get; init; }
    public Gender Gender { get; init; } = Gender.Any;
    public int? MinYear { get; init; }
    public int? MaxYear { get; init; }
    public RodneCisloFormat Format { get; init; } = RodneCisloFormat.WithSlash;
}
