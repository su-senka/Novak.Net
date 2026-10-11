# Changelog

All notable changes to this project are documented here. The format follows [Keep a Changelog](https://keepachangelog.com/),
and the project uses [Semantic Versioning](https://semver.org/).

## [0.1.0] - unreleased

First public release.

### Added
- `Novak.Net.Addresses`: 20,000 real RÚIAN addresses weighted by population, with filters for region (kraj), city or
  Prague post district, and street-only addresses. Formats follow Czech Post conventions.
- `People`: generates a whole person (name, gender, birth date, rodné číslo, e-mail, phone, address, bank account)
  with consistent fields.
- `NovakClient(seed)`: all generators share one seeded sequence, giving reproducible data that still varies from call
  to call. Every generator also accepts an `IRandom`.
- Every generator now has a parameterless constructor that uses the built-in data.
- Names: about 175 common first names and about 135 surnames weighted by frequency, each surname with an explicit
  feminine form. Added `GenerateName()` returning `PersonName`, `CzechSurnames.Feminize`, and a `NameFrequency` enum.
- Emails: name-based local parts, a weighted list of Czech mail providers, the `SafeDomains` option (RFC 2606
  domains), and `EmailStyle`.
- Phone numbers: real ČTÚ mobile ranges and geographic landline prefixes. `PhoneNumberType` and the `International`,
  `National` and `Compact` formats.
- Rodné číslo: `BirthDate` option. `RodneCislo.TryParse`/`Parse`/`IsValid` with `BirthDate` and `Gender`.
- Bank accounts: the official CNB bank list with names and BICs, banks weighted by market share,
  `BankAccount.TryParse`/`IsValid`, and `ToIban(spaced: true)`.
- Packages target .NET 8 and .NET 10 and ship with Source Link, symbol packages and XML docs.

### Fixed
- Female surnames were broken ("Nováková" was generated as "Novákov") and full names mixed genders ("Jana Novák").
- E-mail domains and phone prefixes were read from CSV files that didn't exist, so every e-mail was random
  `@example.com` and every phone number started with 602.
- `Region = "CZ"` on names threw an exception because the data had no regions.
- The bank code list had wrong entries (2310/2250 swapped, defunct banks) and included the central bank. It is now
  generated from the CNB list.
- The rodné číslo validator accepted impossible dates (31 February), did not handle the +20/+70 month extension
  correctly, and rejected valid pre-1986 numbers with check digit 0.
- The bank account validator accepted non-digit input and all-zero accounts.
- Generated account numbers could have a zero prefix even with `WithPrefix = true`.
- `IsPackable` detection broke on Windows paths. LICENSE named the wrong copyright holder, and the repository URL was
  wrong.

### Changed (breaking, pre-release)
- Removed the `Region` and `CountryCode` options that did nothing (the library is Czech-only).
- `PhoneNumberGeneratorOptions.MobileOnly` was replaced by `Type`. `PhoneNumberFormat.Plain`/`Spaced` became
  `Compact`/`International`, and `National` was added.
- `NameGeneratorOptions.FrequencyBucket` is now an enum.
- `BankAccountGenerator.Validate` now requires the bank code (`account/bank`).
- Repository interfaces now return richer records (`Bank`, `PhonePrefix`, weighted domains).
