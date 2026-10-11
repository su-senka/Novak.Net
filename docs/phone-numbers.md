# Novak.Net.PhoneNumbers

Czech phone numbers from the real ranges of the ČTÚ numbering plan. Every number has 9 national digits after `+420`.

```csharp
using Novak.Net.PhoneNumbers;
using Novak.Net.PhoneNumbers.Models;
using Novak.Net.PhoneNumbers.Options;

var phones = new PhoneNumberGenerator();

var mobile = phones.GeneratePhoneNumber();                                    // +420603123456
mobile.ToString(PhoneNumberFormat.International);                             // +420 603 123 456
mobile.ToString(PhoneNumberFormat.National);                                  // 603 123 456
phones.GeneratePhoneNumber(new() { Type = PhoneNumberType.Landline });       // +420541234567 (Brno)
phones.GeneratePhoneNumber(new() { Type = null });                            // ~85 % mobile, ~15 % landline
```

- Mobile numbers use 601–608, 720–739, 770–779 and 790–799.
- Landlines use geographic prefixes: 22x–28x for Prague, 37x for Plzeň, 54x for Brno, 59x for Ostrava, and so on.

## Options (`PhoneNumberGeneratorOptions`)

| Property | Default | Description |
|---|---|---|
| `Seed` | `null` | Fixed seed for this call |
| `Type` | `Mobile` | `Mobile`, `Landline`, or `null` for a realistic mix |
| `Format` | `E164` | `E164` (+420603123456), `International` (+420 603 123 456), `National` (603 123 456), `Compact` (603123456) |

`PhoneNumber` exposes `NationalNumber`, `Type`, `Value` and `ToString(format)`.

> Czech numbering has no range reserved for fiction, so a generated number may be in use. Don't call or text it.
