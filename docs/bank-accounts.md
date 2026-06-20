# Bank accounts module (`src/Novak.Net.BankAccounts`)

Status: available.

## Overview

Generates valid Czech bank account numbers following the CNB weighted-checksum rule and produces both local format and IBAN.

### Validation rules

Both the **prefix** (6 digits) and the **account number** (10 digits) must satisfy the weighted checksum:

| Position | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 |
|---|---|---|---|---|---|---|---|---|---|---|
| Weight | 6 | 3 | 7 | 9 | 10 | 5 | 8 | 4 | 2 | 1 |

Sum of `digit × weight` (zero-padded, left to right) must be ≡ 0 (mod 11).

### IBAN

Czech IBAN: `CZ` + 2 check digits + bank code (4) + prefix (6) + account (10) = **24 characters**.

## Quick start

```csharp
using Novak.Net.BankAccounts;
using Novak.Net.BankAccounts.Data;
using Novak.Net.BankAccounts.Options;

var generator = new BankAccountGenerator(new EmbeddedBankAccountDataRepository());

var account = generator.GenerateBankAccount(new BankAccountGeneratorOptions
{
    Seed       = 42,
    WithPrefix = false,
});

Console.WriteLine(account);          // e.g. 2253325080/6700
Console.WriteLine(account.ToIban()); // e.g. CZ4767000000002253325080
```

## Options (`BankAccountGeneratorOptions`)

| Property | Type | Default | Description |
|---|---|---|---|
| `Seed` | `int?` | `null` | Fixed seed for deterministic output |
| `BankCode` | `string?` | `null` | Override bank code (4 digits, e.g. `"0800"`). Picked randomly when null. |
| `WithPrefix` | `bool?` | `null` | `true` = always include non-zero prefix; `false` = no prefix; `null` = random (~33 % chance) |

## Model (`BankAccount`)

| Property / Method | Description |
|---|---|
| `Prefix` | Zero-padded 6-digit prefix string (e.g. `"000019"`) |
| `AccountNumber` | Zero-padded 10-digit account string |
| `BankCode` | 4-digit bank code string |
| `ToLocalFormat()` / `ToString()` | Czech local format, e.g. `"19-2000145399/0800"` |
| `ToIban()` | Czech IBAN, e.g. `"CZ6508000000192000145399"` |

## Validation

```csharp
bool ok  = BankAccountGenerator.Validate("19-2000145399/0800"); // true
bool bad = BankAccountGenerator.Validate("2000145398/0800");    // false
```

## Embedded bank codes

The module ships with a curated list of active Czech bank codes sourced from the CNB register, covering all major retail and savings banks (e.g. 0800 Česká spořitelna, 0100 Komerční banka, 5500 Raiffeisenbank, 3030 Air Bank, and 30+ others). Override with `BankCode` to pin a specific bank.
