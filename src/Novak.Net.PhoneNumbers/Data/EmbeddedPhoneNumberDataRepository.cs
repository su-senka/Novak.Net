using System.Globalization;
using Novak.Net.Common.Data;
using Novak.Net.PhoneNumbers.Models;

namespace Novak.Net.PhoneNumbers.Data;

/// <summary>Built-in Czech mobile and geographic prefixes.</summary>
public sealed class EmbeddedPhoneNumberDataRepository : IPhoneNumberDataRepository
{
    private static readonly Lazy<PhonePrefix[]> Prefixes = new(() =>
        EmbeddedData.ReadRows(typeof(EmbeddedPhoneNumberDataRepository).Assembly, "phone_prefixes_cz.csv", hasHeader: true)
            .Select(r => new PhonePrefix(
                r[0],
                Enum.Parse<PhoneNumberType>(r[1], ignoreCase: true),
                string.IsNullOrWhiteSpace(r[2]) ? null : r[2],
                double.Parse(r[3], CultureInfo.InvariantCulture)))
            .ToArray());

    public IReadOnlyList<PhonePrefix> GetPhonePrefixes() => Prefixes.Value;
}
