using Novak.Net.Common.Options;

namespace Novak.Net.PhoneNumbers.Options;

public enum PhoneNumberFormat { E164, Plain, Spaced }

public sealed class PhoneNumberGeneratorOptions : ISeededOptions, IRegionalOptions
{
    public int? Seed { get; init; }
    public string? Region { get; init; }
    public PhoneNumberFormat Format { get; init; } = PhoneNumberFormat.E164;
    public bool MobileOnly { get; init; } = true;
    public string? CountryCode { get; init; }
}
