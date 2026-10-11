# Novak.Net.BankAccounts

Valid Czech bank account numbers, with IBAN and BIC, based on the official Czech National Bank list of bank codes.

```csharp
using Novak.Net.BankAccounts;
using Novak.Net.BankAccounts.Models;
using Novak.Net.BankAccounts.Options;

var generator = new BankAccountGenerator();

var account = generator.GenerateBankAccount();
account.ToString();               // "9540064479/0800"
account.BankName;                 // "Česká spořitelna, a.s."
account.Bic;                      // "GIBACZPX"
account.ToIban();                 // "CZ3308000000009540064479"
account.ToIban(spaced: true);     // "CZ33 0800 0000 0095 4006 4479"

generator.GenerateBankAccount(new() { BankCode = "2010", WithPrefix = true }); // "954007-5472535751/2010"

BankAccountGenerator.Validate("19-2000145399/0800");          // true
BankAccount.TryParse("19-2000145399/0800", out var parsed);  // parsed.ToIban() == "CZ6508000000192000145399"
```

## Rules

These rules follow CNB Decree 169/2011:

- The prefix (*předčíslí*, up to 6 digits) and the account number (2–10 digits) are each zero-padded. Each must have
  a weighted digit sum divisible by 11. The weights are `6, 3, 7, 9, 10, 5, 8, 4, 2, 1`, and the prefix uses the last
  six.
- The account number must have at least two non-zero digits.
- A Czech IBAN is `CZ` + 2 check digits + bank code (4) + prefix (6) + account number (10), 24 characters in total.

## Banks

The bundled list is the CNB code list as of October 2026, with names and BICs. Random picks are weighted by each bank's
approximate share of personal accounts: Česká spořitelna, ČSOB, Komerční banka, Fio, Raiffeisenbank, MONETA, Air Bank,
mBank, UniCredit and others. Codes that aren't retail banks are valid but never picked at random. These include the
CNB, building societies, payment institutions and banks in liquidation. You can still request any of them with
`BankCode`.

## Options (`BankAccountGeneratorOptions`)

| Property | Default | Description |
|---|---|---|
| `Seed` | `null` | Fixed seed for this call |
| `BankCode` | `null` | 4-digit bank code. Weighted random when null. |
| `WithPrefix` | `null` | `true` always adds a prefix, `false` never does, `null` adds one about 15 % of the time |
