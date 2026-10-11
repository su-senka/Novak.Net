# Novak.Net.RodneCislo

Valid Czech birth numbers (*rodné číslo*), with a strict validator and parser.

```csharp
using Novak.Net.RodneCislo;
using Novak.Net.RodneCislo.Models;
using Novak.Net.RodneCislo.Options;

var generator = new RodneCisloGenerator();

var rc = generator.Generate(new() { Gender = Gender.Female, BirthDate = new DateOnly(1985, 2, 28) });
rc.Value;      // "855228/2486"
rc.Digits;     // "8552282486"
rc.BirthDate;  // 1985-02-28
rc.Gender;     // Female

RodneCisloGenerator.Validate("736028/5163");          // true
RodneCislo.TryParse("736028/5163", out var parsed);   // parsed.BirthDate == 1973-10-28
```

## Rules

- Format `YYMMDD/SSSC`. Births before 1954 have 9 digits (`YYMMDD/SSS`) and no check digit.
- From 1954 on, the 10-digit number is divisible by 11. A check digit of `0` for remainder 10 was allowed until 1985.
  The validator accepts it, and the generator never produces it.
- Women have 50 added to the month. Since 2004, a further +20 is allowed when serial numbers run out. The validator
  accepts it, and the generator doesn't need it.
- The validator checks the actual calendar date, so 30 February is rejected.
- Years 1900–2053 are supported.

## Options (`RodneCisloGeneratorOptions`)

| Property | Default | Description |
|---|---|---|
| `Seed` | `null` | Fixed seed for this call |
| `Gender` | `Any` | `Male`, `Female`, or random |
| `BirthDate` | `null` | Exact date to encode. Overrides the year range. |
| `MinYear` / `MaxYear` | `1940` / `2005` | Birth year range for a random date |
| `Format` | `WithSlash` | `WithSlash` or `WithoutSlash` |
