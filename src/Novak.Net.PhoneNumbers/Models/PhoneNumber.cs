using Novak.Net.PhoneNumbers.Options;

namespace Novak.Net.PhoneNumbers.Models;

/// <summary>Kind of phone number.</summary>
public enum PhoneNumberType
{
    Mobile,
    Landline
}

/// <summary>A Czech phone number.</summary>
/// <param name="NationalNumber">The 9-digit national number without the +420 country code, e.g. "603123456".</param>
/// <param name="Type">Mobile or landline.</param>
/// <param name="Format">The format used by <see cref="Value"/> and <see cref="ToString()"/>.</param>
public readonly record struct PhoneNumber(string NationalNumber, PhoneNumberType Type, PhoneNumberFormat Format = PhoneNumberFormat.E164)
{
    /// <summary>Czech country calling code.</summary>
    public const string CountryCode = "420";

    /// <summary>The number in <see cref="Format"/>.</summary>
    public string Value => ToString(Format);

    /// <summary>Formats the number: E164 "+420603123456", International "+420 603 123 456",
    /// National "603 123 456", Compact "603123456".</summary>
    public string ToString(PhoneNumberFormat format)
    {
        var n = NationalNumber;
        var grouped = n.Length == 9 ? $"{n[..3]} {n[3..6]} {n[6..]}" : n;
        return format switch
        {
            PhoneNumberFormat.International => $"+{CountryCode} {grouped}",
            PhoneNumberFormat.National => grouped,
            PhoneNumberFormat.Compact => n,
            _ => $"+{CountryCode}{n}"
        };
    }

    public override string ToString() => Value;
}
