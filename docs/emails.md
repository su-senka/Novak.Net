# Novak.Net.Emails

E-mail addresses that look like the ones Czech people actually use: built from a name and spread across popular Czech
providers.

```csharp
using Novak.Net.Emails;
using Novak.Net.Emails.Options;

var emails = new EmailGenerator();

emails.GenerateEmail();                                                       // "petra.horakova@seznam.cz"
emails.GenerateEmail(new() { FirstName = "Jiří", LastName = "Šťastný" });     // "jiri.stastny85@email.cz"
emails.GenerateEmail(new() { SafeDomains = true });                           // "...@example.com"
emails.GenerateEmail(new() { Style = EmailStyle.Random });                    // "k3vx9qpa1m@gmail.com"
```

Local parts use common patterns: `jan.novak`, `jannovak`, `novak.jan`, `jnovak`, `novakj`, `jan_novak`, `jan.novak85`.
Diacritics are removed. When no name is given, a random Czech name is used.

The domains come from a weighted list: seznam.cz, gmail.com, email.cz, centrum.cz, post.cz, volny.cz, outlook.com,
atlas.cz, icloud.com and others.

## Options (`EmailGeneratorOptions`)

| Property | Default | Description |
|---|---|---|
| `Seed` | `null` | Fixed seed for this call |
| `Style` | `NameBased` | `NameBased` or `Random` |
| `FirstName` / `LastName` | `null` | Name to build the address from |
| `SafeDomains` | `false` | Only `example.com`/`.net`/`.org` (RFC 2606). Use this whenever a system might actually send mail. |
| `AllowedDomains` | `null` | Your own domain pool. Overrides the two options above. |
| `Lowercase` | `true` | Lower-case the whole address |
| `MaxLocalPartLength` | `null` | Cap the part before `@` (max 64) |

> Realistic addresses on real providers may belong to real people. Never send e-mail to generated addresses unless
> `SafeDomains` or your own test domains are used.
