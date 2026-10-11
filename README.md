# Novak.Net

Realistic, internally consistent **Czech test data** for .NET: people with names, rodné číslo, e-mails,
phone numbers, real postal addresses and valid bank accounts.

```csharp
using Novak.Net;

var faker = new NovakClient(seed: 42);       // same data on every run
var p = faker.People.GeneratePerson();

// e.g. Iveta Poláková, 536118/944, iveta.polakova@gmail.com, +420722249673
// Holcova 1289, 530 02 Pardubice
// 1260367954/6363 (Partners Banka), CZ1963630000001260367954
```

- **Correct Czech rules.** Feminine surnames (Novák → Nováková, Vaněk → Vaňková, Černý → Černá). Rodné číslo with
  every historical rule. Account numbers using the CNB mod-11 checksum, with IBAN and BIC.
- **Real reference data.** 20,000 real addresses from the RÚIAN register, weighted by population. The official CNB bank
  code list, weighted by market share. Real mobile and landline ranges from the ČTÚ numbering plan. The most common
  Czech names, weighted by frequency.
- **Consistent people.** The rodné číslo encodes the person's birth date and gender, and the e-mail is built from the name.
- **Deterministic.** Seed the whole client for reproducible sequences, or seed a single call.
- **Modular.** Use the `Novak.Net` facade, or reference only the packages you need.

## Install

```bash
dotnet add package Novak.Net
```

Targets .NET 8 and .NET 10. No dependencies outside the `Novak.Net.*` packages.

| Package | Contents |
|---|---|
| `Novak.Net` | `NovakClient` facade and the `Person` generator. References all modules below. |
| `Novak.Net.Names` | First names, surnames, full names. [docs](docs/names.md) |
| `Novak.Net.Emails` | Name-based e-mail addresses. [docs](docs/emails.md) |
| `Novak.Net.PhoneNumbers` | +420 mobile and landline numbers. [docs](docs/phone-numbers.md) |
| `Novak.Net.Addresses` | Real RÚIAN postal addresses. [docs](docs/addresses.md) |
| `Novak.Net.RodneCislo` | Birth number generator, validator and parser. [docs](docs/rodne-cislo.md) |
| `Novak.Net.BankAccounts` | Account numbers, IBAN, BIC, validator. [docs](docs/bank-accounts.md) |
| `Novak.Net.Common` | Shared randomness primitives. |

## People

```csharp
using Novak.Net.People;
using Novak.Net.Addresses.Models;
using Novak.Net.Names.Models;

var person = faker.People.GeneratePerson(new PersonGeneratorOptions
{
    Gender = Gender.Female,
    MinAge = 25,
    MaxAge = 40,
    Region = CzechRegion.Jihomoravsky,   // or City = "Brno"
    SafeEmailDomains = true,             // example.com / .net / .org only
});

person.FullName;                    // "Kateřina Dvořáková"
person.BirthDate;                   // 1991-04-17
person.RodneCislo.Value;            // "915417/2489" (matches the birth date and gender)
person.Email.Value;                 // "katerina.dvorakova@example.com"
person.Phone.ToString(PhoneNumberFormat.International); // "+420 731 234 567"
person.Address.ToMultiline();       // "Arménská 506/11\n625 00 Brno"
person.BankAccount.ToIban(spaced: true);
```

Ages are computed against today unless you set `ReferenceDate`. Set it if a seeded person must stay identical on every
day your tests run.

## Determinism

```csharp
var faker = new NovakClient(seed: 123);    // all generators share one seeded sequence
faker.Names.GenerateFullName();            // differs on each call, identical on every run

faker.Names.GenerateFullName(new NameGeneratorOptions { Seed = 7 }); // pinned: always the same value
```

Every generator also has a constructor that accepts an optional `IRandom`. A seeded sequence is reproducible on a given
.NET major version, because `System.Random` does not guarantee the same sequence across runtime versions.

## Individual generators

Every generator works without arguments and has an options object.

```csharp
new NameGenerator().GenerateName(new NameGeneratorOptions { Gender = Gender.Male });            // Jakub Novotný
new EmailGenerator().GenerateEmail(new EmailGeneratorOptions { FirstName = "Jiří", LastName = "Říha" }); // jiri.riha@seznam.cz
new PhoneNumberGenerator().GeneratePhoneNumber();                                                // +420603123456
new AddressGenerator().GenerateAddress(new AddressGeneratorOptions { City = "Praha 5" });      // Barrandovská 346/58, 152 00 Praha 5
new RodneCisloGenerator().Generate(new RodneCisloGeneratorOptions { BirthDate = new(1985, 2, 28) });
new BankAccountGenerator().GenerateBankAccount(new BankAccountGeneratorOptions { BankCode = "0800" });

RodneCisloGenerator.Validate("736028/5163");     // true
BankAccountGenerator.Validate("19-2000145399/0800"); // true
```

## Run the sample

```bash
dotnet run --project samples/Novak.Net.Sample -- 42
```

## Build and test

```bash
dotnet build
dotnet test
```

Tests use [TUnit](https://tunit.dev) on Microsoft.Testing.Platform (configured in `global.json`).

## A note on realism

Generated values can coincide with real ones. The addresses are real buildings, valid phone numbers and e-mail
addresses may belong to someone, and a random account number can exist. Treat the data as test data. Don't send
e-mails, SMS or payments to it from a system that can reach the outside world. For that case, use
`SafeDomains`/`SafeEmailDomains`, or your own `AllowedDomains`.

## Data sources and licences

The code is MIT-licensed. The bundled reference data comes from public Czech open data, listed in
[THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md). Addresses come from RÚIAN (© ČÚZK, CC BY 4.0) and bank codes come from
the Czech National Bank. Run `tools/build_address_data.py` to regenerate the address sample from a newer RÚIAN export.
