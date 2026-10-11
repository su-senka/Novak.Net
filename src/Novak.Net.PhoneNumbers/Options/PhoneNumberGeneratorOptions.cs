using Novak.Net.Common.Options;
using Novak.Net.PhoneNumbers.Models;

namespace Novak.Net.PhoneNumbers.Options;

/// <summary>Output format of a phone number.</summary>
public enum PhoneNumberFormat
{
    /// <summary>+420603123456</summary>
    E164,

    /// <summary>+420 603 123 456</summary>
    International,

    /// <summary>603 123 456</summary>
    National,

    /// <summary>603123456</summary>
    Compact
}

/// <summary>Options for <see cref="PhoneNumberGenerator"/>.</summary>
public sealed class PhoneNumberGeneratorOptions : ISeededOptions
{
    public int? Seed { get; init; }

    public PhoneNumberFormat Format { get; init; } = PhoneNumberFormat.E164;

    /// <summary>Mobile (default) or landline. Null picks either, roughly 85 % mobile like real contact data.</summary>
    public PhoneNumberType? Type { get; init; } = PhoneNumberType.Mobile;
}
