using Novak.Net.Common.Options;
using Novak.Net.Names.Models;

namespace Novak.Net.Names.Options;

/// <summary>
/// Options for <see cref="NameGenerator"/>. The text filters (<see cref="StartsWith"/>, <see cref="MinLength"/>,
/// <see cref="MaxLength"/>, <see cref="Include"/>, <see cref="Exclude"/>) apply to the part being generated;
/// for full names they apply to the first name only.
/// </summary>
public sealed class NameGeneratorOptions : ISeededOptions
{
    /// <summary>Gender of the person. <see cref="Gender.Any"/> picks one at random.</summary>
    public Gender Gender { get; init; } = Gender.Any;

    /// <summary>Keep Czech diacritics (default) or output plain ASCII letters.</summary>
    public bool WithDiacritics { get; init; } = true;

    public Casing Casing { get; init; } = Casing.Original;
    public int? MinLength { get; init; }
    public int? MaxLength { get; init; }
    public char? StartsWith { get; init; }

    /// <summary>Names never to return (case- and, without diacritics, accent-insensitive).</summary>
    public IReadOnlyCollection<string>? Exclude { get; init; }

    /// <summary>When set, only these names may be returned.</summary>
    public IReadOnlyCollection<string>? Include { get; init; }

    /// <summary>Restrict to common, normal or rare names. Null means any.</summary>
    public NameFrequency? FrequencyBucket { get; init; }

    public int? Seed { get; init; }
}
