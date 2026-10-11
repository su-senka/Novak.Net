# Novak.Net.Addresses

Real Czech postal addresses from the official address register **RÚIAN** (Registr územní identifikace, adres a
nemovitostí).

```csharp
using Novak.Net.Addresses;
using Novak.Net.Addresses.Models;
using Novak.Net.Addresses.Options;

var addresses = new AddressGenerator();

var a = addresses.GenerateAddress();
a.ToString();       // "Arménská 506/11, 625 00 Brno"
a.ToMultiline();    // "Arménská 506/11\n625 00 Brno"
a.Region.ToCzechName(); // "Jihomoravský kraj"

addresses.GenerateAddress(new() { Region = CzechRegion.Moravskoslezsky });
addresses.GenerateAddress(new() { City = "Brno" });
addresses.GenerateAddress(new() { City = "Praha 5" });     // Prague post district
addresses.GenerateAddress(new() { RequireStreet = true }); // skip "Lhota 25"-style village addresses
```

## Data

The package includes 20,000 address points sampled from the RÚIAN export of 30 September 2026. They cover 5,178
municipalities in all 14 regions.

- **Weighting.** The sample is proportional to each municipality's population, so Prague, Brno and Ostrava appear
  about as often as people actually live there.
- **What's included.** Only buildings with a *číslo popisné* (residential numbering) are sampled.
- **Data size.** The sample is gzip-compressed inside the assembly (about 220 KB) and loaded on first use.

To refresh the sample from a newer RÚIAN export, run [`tools/build_address_data.py`](../tools/build_address_data.py).

## Model (`Address`)

| Property | Example |
|---|---|
| `Street` | `"Vodičkova"` (null in villages without streets) |
| `HouseNumber` (číslo popisné) | `"681"` |
| `OrientationNumber` (číslo orientační) | `"14"`, `"96a"` |
| `Number` | `"681/14"` |
| `Municipality` (obec) | `"Praha"` |
| `CityPart` (část obce) | `"Nové Město"` |
| `District` (MČ / MO) | `"Praha 1"`, `"Brno-střed"` |
| `PostTown` | `"Praha 1"` (Prague post districts), otherwise the municipality |
| `PostalCode` / `PostalCodeFormatted` | `"11000"` / `"110 00"` |
| `Region` | `CzechRegion.Praha` |
| `StreetLine`, `TownLine`, `ToString()`, `ToMultiline()` | Czech Post address format |

## Licence

The address data comes from © ČÚZK, RÚIAN, under the [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/) licence.
The addresses are real buildings. Use them as test data only.
