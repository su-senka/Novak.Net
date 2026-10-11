using Novak.Net.Common.Options;
using Novak.Net.RodneCislo.Models;

namespace Novak.Net.RodneCislo.Options;

/// <summary>Output format.</summary>
public enum RodneCisloFormat
{
    /// <summary>855203/1236</summary>
    WithSlash,

    /// <summary>8552031236</summary>
    WithoutSlash
}

/// <summary>Options for <see cref="RodneCisloGenerator"/>.</summary>
public sealed class RodneCisloGeneratorOptions : ISeededOptions
{
    public int? Seed { get; init; }

    /// <summary>Gender to encode; <see cref="Gender.Any"/> picks one at random.</summary>
    public Gender Gender { get; init; } = Gender.Any;

    /// <summary>Exact birth date to encode (1900–2053). Overrides <see cref="MinYear"/>/<see cref="MaxYear"/>.</summary>
    public DateOnly? BirthDate { get; init; }

    /// <summary>Earliest birth year (inclusive), default 1940.</summary>
    public int? MinYear { get; init; }

    /// <summary>Latest birth year (inclusive), default 2005.</summary>
    public int? MaxYear { get; init; }

    public RodneCisloFormat Format { get; init; } = RodneCisloFormat.WithSlash;
}
