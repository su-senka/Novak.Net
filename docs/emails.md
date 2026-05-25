# Emails module (`src/Novak.Net.Emails`)

Current scope:
- Generate random local-part + domain email values.
- Deterministic output via `Seed`.
- Domain control with `AllowedDomains`.

Implementation notes:
- Main files: `EmailGenerator.cs`, `Data/EmbeddedEmailDataRepository.cs`.
- If no domain data is available, repository fallback is `example.com`.

