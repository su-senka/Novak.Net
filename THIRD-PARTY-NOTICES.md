# Third-party data notices

Novak.Net's source code is MIT-licensed. The reference data bundled in the packages comes from the sources below.

## RÚIAN address points (`Novak.Net.Addresses`)

- **Source:** Registr územní identifikace, adres a nemovitostí (RÚIAN), Český úřad zeměměřický a katastrální (ČÚZK).
  CSV export of 30 September 2026 (`20260930_OB_ADR_csv.zip`, `20260930_strukt_ADR.csv.zip`) from
  https://vdp.cuzk.gov.cz/
- **Licence:** [Creative Commons Attribution 4.0 International (CC BY 4.0)](https://creativecommons.org/licenses/by/4.0/)
- **Attribution:** © ČÚZK, RÚIAN
- **Changes:** a population-weighted random sample of 20,000 address points (číslo popisné only). Coordinates and
  internal codes are dropped, and municipalities are mapped to regions. See `tools/build_address_data.py`.

## Bank codes (`Novak.Net.BankAccounts`)

- **Source:** Česká národní banka, list of payment system codes (*Kódy bank*), downloaded October 2026 from
  https://www.cnb.cz/cs/platebni-styk/ucty-kody-bank/
- **Changes:** sampling weights added. These are rough estimates by the Novak.Net authors, not CNB data.

## Phone number ranges (`Novak.Net.PhoneNumbers`)

- Number ranges follow the Czech numbering plan of the Český telekomunikační úřad (ČTÚ). The sampling weights are
  rough estimates.

## Names (`Novak.Net.Names`)

- Common Czech first names and surnames. The weights are rounded approximations of the last name-frequency statistics
  published by the Ministry of the Interior (MVČR). They are meant for realistic sampling, not as statistics.
