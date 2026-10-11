using Novak.Net.PhoneNumbers.Models;

namespace Novak.Net.PhoneNumbers;

/// <summary>A national number prefix and what it is used for.</summary>
/// <param name="Prefix">Leading digits of the 9-digit national number.</param>
/// <param name="Type">Mobile or landline.</param>
/// <param name="Area">For landlines, the region the prefix belongs to.</param>
/// <param name="Weight">Relative sampling weight.</param>
public sealed record PhonePrefix(string Prefix, PhoneNumberType Type, string? Area, double Weight);

/// <summary>Provides the number prefixes to generate from.</summary>
public interface IPhoneNumberDataRepository
{
    IReadOnlyList<PhonePrefix> GetPhonePrefixes();
}
