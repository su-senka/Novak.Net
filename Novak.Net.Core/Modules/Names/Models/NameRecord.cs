namespace Novak.Net.Core.Modules.Names.Models;

/// <summary>
/// Represents a single name unit with metadata and sampling weight.
/// </summary>
public sealed class NameRecord
{
    /// <summary>Normalized text with diacritics in source dataset.</summary>
    public required string Text { get; init; }
    /// <summary>Gender for first names; Any for surnames.</summary>
    public Gender Gender { get; init; }
    /// <summary>Non-negative relative frequency (weight).</summary>
    public double Frequency { get; init; }
    /// <summary>Optional region tag.</summary>
    public string? Region { get; init; }
    /// <summary>Whether first name is archaic (ignored for surnames).</summary>
    public bool Archaic { get; init; }
    // surname flags
    public bool Genderable { get; init; }
    public bool Foreign { get; init; }
    public bool Indeclinable { get; init; }
}

