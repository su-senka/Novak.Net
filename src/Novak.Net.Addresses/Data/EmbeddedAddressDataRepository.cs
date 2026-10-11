using Novak.Net.Addresses.Models;
using Novak.Net.Common.Data;

namespace Novak.Net.Addresses.Data;

/// <summary>
/// Built-in sample of 20 000 real address points from RÚIAN (© ČÚZK, CC BY 4.0), drawn in proportion to the
/// population of each municipality, so Prague, Brno and Ostrava appear about as often as people live there.
/// </summary>
public sealed class EmbeddedAddressDataRepository : IAddressDataRepository
{
    private static readonly Lazy<Address[]> Addresses = new(Load);

    public IReadOnlyList<Address> GetAddresses() => Addresses.Value;

    private static Address[] Load()
    {
        var rows = EmbeddedData.ReadRows(typeof(EmbeddedAddressDataRepository).Assembly, "addresses_cz.tsv.gz", '\t');
        var result = new Address[rows.Count];
        for (int i = 0; i < rows.Count; i++)
        {
            // kraj, obec, momc, praha (post district), cast_obce, ulice, cp, co, co_znak, psc
            var r = rows[i];
            result[i] = new Address
            {
                Region = (CzechRegion)int.Parse(r[0]),
                Municipality = string.Intern(r[1]),
                District = r[2].Length == 0 ? null : string.Intern(r[2]),
                PostTown = string.Intern(r[3].Length == 0 ? r[1] : r[3]),
                CityPart = string.Intern(r[4]),
                Street = r[5].Length == 0 ? null : string.Intern(r[5]),
                HouseNumber = r[6],
                OrientationNumber = r[7].Length == 0 ? null : r[7] + r[8],
                PostalCode = r[9]
            };
        }
        return result;
    }
}
