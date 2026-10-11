namespace Novak.Net.Names.Models;

/// <summary>
/// A single name in a data source together with its sampling weight.
/// </summary>
public sealed class NameRecord
{
    /// <summary>The name as written, with diacritics. For surnames this is the masculine form.</summary>
    public required string Text { get; init; }

    /// <summary>Gender of a first name; <see cref="Gender.Any"/> for surnames.</summary>
    public Gender Gender { get; init; } = Gender.Any;

    /// <summary>Non-negative relative frequency used as the sampling weight.</summary>
    public double Frequency { get; init; }

    /// <summary>
    /// Feminine form of a surname (Novák → Nováková, Krejčí → Krejčí). When null, the form is derived
    /// with <see cref="CzechSurnames.Feminize"/>.
    /// </summary>
    public string? FeminineForm { get; init; }
}
