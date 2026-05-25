# Phone Numbers module (`src/Novak.Net.PhoneNumbers`)

Current scope:
- Generate phone numbers in `E164`, `Plain`, or `Spaced` formats.
- Deterministic output via `Seed`.
- Region/country code and mobile prefix selection.

Implementation notes:
- Main files: `PhoneNumberGenerator.cs`, `Data/EmbeddedPhoneNumberDataRepository.cs`.
- Repository has defaults for missing dataset entries (country `+420`, prefix `602`).

