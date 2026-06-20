# Rodné číslo module (`src/Novak.Net.RodneCislo`)

Status: available.

## Overview

Generates structurally valid Czech birth numbers (rodná čísla) following the official CNB / Ministry of Interior format:

- **Pre-1954 births**: 9-digit format `YYMMDD/XXX` — no checksum.
- **1954+ births**: 10-digit format `YYMMDD/XXXC` — the full 10-digit number is divisible by 11.
- **Female encoding**: month is increased by 50 (e.g. February → 52).
- **Slash separator**: optional via `Format` option.

## Quick start

```csharp
using Novak.Net.RodneCislo;
using Novak.Net.RodneCislo.Models;
using Novak.Net.RodneCislo.Options;

var generator = new RodneCisloGenerator();

var rc = generator.Generate(new RodneCisloGeneratorOptions
{
    Seed   = 42,
    Gender = Gender.Female,
});

Console.WriteLine(rc);        // e.g. 845204/5228
Console.WriteLine(rc.Digits); // without slash: 8452045228
```

## Options (`RodneCisloGeneratorOptions`)

| Property | Type | Default | Description |
|---|---|---|---|
| `Seed` | `int?` | `null` | Fixed seed for deterministic output |
| `Gender` | `Gender` | `Any` | `Male`, `Female`, or `Any` |
| `MinYear` | `int?` | `1940` | Minimum birth year (inclusive) |
| `MaxYear` | `int?` | `2005` | Maximum birth year (inclusive) |
| `Format` | `RodneCisloFormat` | `WithSlash` | `WithSlash` or `WithoutSlash` |

## Validation

```csharp
bool ok = RodneCisloGenerator.Validate("845204/5228"); // true
bool bad = RodneCisloGenerator.Validate("8006154281"); // false — wrong check digit
```

`Validate` checks:
1. Length is 9 or 10 digits (ignoring `/` and spaces).
2. Month encodes a value 1–12 (accounting for female offset).
3. Day is 1–31.
4. For 10-digit numbers: the value is divisible by 11.
