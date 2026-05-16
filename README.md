# Novak.Net

A .NET library for generating synthetic Czech personal data — names, emails, and phone numbers — for use in testing, seeding databases, and demo applications.

## Quick start

The simplest entry point is `NovakNetClient`, which wires up all generators automatically:

```csharp
using Novak.Net.Core;

var client = new NovakNetClient();

var name  = client.Names.GenerateFullName();
var email = client.Emails.GenerateEmail();
var phone = client.PhoneNumbers.GeneratePhoneNumber();
```

## Names

```csharp
using Novak.Net.Core.Modules.Names;
using Novak.Net.Core.Modules.Names.Models;
using Novak.Net.Core.Modules.Names.Options;

var generator = new NameGenerator(new DataSource());

// Single name
var full = generator.GenerateFullName(new NameGeneratorOptions { Gender = Gender.Female });

// Deterministic sequence
var names = generator.GenerateMany(
    () => new NameGeneratorOptions { Gender = Gender.Male, Seed = 42 },
    count: 10);
```

### `NameGeneratorOptions`

| Property | Type | Default | Description |
|---|---|---|---|
| `Gender` | `Gender` | `Any` | `Male`, `Female`, or `Any` |
| `Seed` | `int?` | `null` | Fixed seed for deterministic output |
| `WithDiacritics` | `bool` | `true` | Preserve Czech diacritics |
| `Casing` | `Casing` | `Original` | `Original`, `Upper`, `Lower`, `Title` |
| `MinLength` / `MaxLength` | `int?` | `null` | Character length bounds |
| `StartsWith` | `char?` | `null` | Filter by starting letter |
| `Include` / `Exclude` | `IReadOnlyCollection<string>?` | `null` | Allow- / deny-list of specific names |
| `FrequencyBucket` | `string?` | `null` | `"Common"`, `"Normal"`, or `"Rare"` |
| `Region` | `string?` | `null` | Region filter (e.g. `"CZ"`) |

## Emails

```csharp
using Novak.Net.Core.Modules.Emails;
using Novak.Net.Core.Modules.Emails.Data;
using Novak.Net.Core.Modules.Emails.Options;

var generator = new EmailGenerator(new EmbeddedEmailDataRepository());
var email = generator.GenerateEmail(new EmailGeneratorOptions { Seed = 42 });
Console.WriteLine(email); // e.g. xk3ab9fz@example.cz
```

### `EmailGeneratorOptions`

| Property | Type | Default | Description |
|---|---|---|---|
| `Seed` | `int?` | `null` | Fixed seed for deterministic output |
| `Region` | `string?` | `null` | Region used to select domains |
| `Lowercase` | `bool` | `true` | Force lowercase output |
| `AllowedDomains` | `IReadOnlyList<string>?` | `null` | Override the domain pool |
| `MaxLocalPartLength` | `int?` | `null` | Cap the local-part length |

## Phone numbers

```csharp
using Novak.Net.Core.Modules.PhoneNumbers;
using Novak.Net.Core.Modules.PhoneNumbers.Data;
using Novak.Net.Core.Modules.PhoneNumbers.Options;

var generator = new PhoneNumberGenerator(new EmbeddedPhoneNumberDataRepository());
var phone = generator.GeneratePhoneNumber(new PhoneNumberGeneratorOptions
{
    Format = PhoneNumberFormat.Spaced,
    Seed   = 42
});
Console.WriteLine(phone); // e.g. +420 603 123 456
```

### `PhoneNumberGeneratorOptions`

| Property | Type | Default | Description |
|---|---|---|---|
| `Seed` | `int?` | `null` | Fixed seed for deterministic output |
| `Region` | `string?` | `null` | Region used to select country code and prefixes |
| `Format` | `PhoneNumberFormat` | `E164` | `E164`, `Plain`, or `Spaced` |
| `MobileOnly` | `bool` | `true` | Restrict to mobile prefixes |
| `CountryCode` | `string?` | `null` | Override country code (e.g. `"420"`) |

## Install

Until published on NuGet, reference the project directly:

```xml
<ProjectReference Include="../Novak.Net.Core/Novak.Net.Core.csproj" />
```

## Status

| Generator | Status |
|---|---|
| Names (first / surname, gender-aware) | ✅ Available |
| Emails | ✅ Available |
| Phone numbers (CZ) | ✅ Available |
| Addresses | 🔜 Planned |
| Rodné číslo | 🔜 Planned |
| Bank accounts | 🔜 Planned |

