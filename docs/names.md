# Novak.Net.Names

Realistic Czech names. Generated full names keep the gender consistent, so a woman gets the feminine surname form.

```csharp
using Novak.Net.Names;
using Novak.Net.Names.Models;
using Novak.Net.Names.Options;

var names = new NameGenerator();

names.GenerateName();                                        // PersonName("Jana", "Nováková", Female)
names.GenerateFullName(new() { Gender = Gender.Male });     // "Tomáš Dvořák"
names.GenerateFirstName(new() { StartsWith = 'K' });         // "Kateřina"
names.GenerateSurname(new() { Gender = Gender.Female });     // "Šťastná"

CzechSurnames.Feminize("Vaněk");                             // "Vaňková"
```

## Data

The package includes about 175 of the most common Czech first names and about 135 of the most common surnames. Each
name is weighted by its approximate number of bearers, rounded from the last public MVČR name statistics. Each surname
has an explicit feminine form (Kadlec → Kadlecová, Němec → Němcová, Krejčí → Krejčí). With a custom `IDataSource`
that has no feminine forms, the regular Czech rules in `CzechSurnames.Feminize` are used instead.

## Options (`NameGeneratorOptions`)

| Property | Default | Description |
|---|---|---|
| `Gender` | `Any` | `Male`, `Female` or `Any` (random) |
| `Seed` | `null` | Fixed seed for this call |
| `WithDiacritics` | `true` | `false` outputs ASCII only ("Jiri Richa") |
| `Casing` | `Original` | `Original`, `Upper`, `Lower`, `Title` |
| `MinLength` / `MaxLength` | `null` | Length bounds |
| `StartsWith` | `null` | First letter (case-insensitive) |
| `Include` / `Exclude` | `null` | Allow-list / deny-list of names |
| `FrequencyBucket` | `null` | `Common`, `Normal` or `Rare` (top 20 %, middle, bottom 20 % of bearers) |

The text filters (`StartsWith`, length bounds, `Include`, `Exclude`) apply to the name part being generated. In
`GenerateName`/`GenerateFullName` they apply to the first name only.

`GenerateMany(() => options, count)` shares one random sequence when the options carry a seed, so the batch is both
reproducible and varied.
