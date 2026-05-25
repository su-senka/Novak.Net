# Names module (`src/Novak.Net.Names`)

Current scope:
- Generate first name, surname, and full name.
- Support filtering (`Gender`, `Region`, include/exclude, length bounds, starts-with).
- Support deterministic output via `Seed`.

Implementation notes:
- Data source: embedded CSV files in `src/Novak.Net.Names/Data/`.
- Weighted sampling uses linear cumulative logic for small pools and alias sampling for larger pools.
- Main files: `NameGenerator.cs`, `DataSource.cs`, `Sampling/AliasSampler.cs`.

