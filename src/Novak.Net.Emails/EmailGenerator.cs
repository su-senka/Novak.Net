using System.Text;
using Novak.Net.Common.Randomization;
using Novak.Net.Common.Text;
using Novak.Net.Emails.Data;
using Novak.Net.Emails.Models;
using Novak.Net.Emails.Options;
using Novak.Net.Names;

namespace Novak.Net.Emails;

/// <summary>Default <see cref="IEmailGenerator"/> implementation.</summary>
public class EmailGenerator : IEmailGenerator
{
    private static readonly string[] SafeDomainPool = ["example.com", "example.net", "example.org"];

    private readonly IEmailDataRepository _repo;
    private readonly INameGenerator _names;
    private readonly IRandom _random;

    /// <param name="repo">Domain data; defaults to the built-in Czech list.</param>
    /// <param name="names">Name source for name-based addresses when no name is given.</param>
    /// <param name="random">Random source used when options carry no seed.</param>
    public EmailGenerator(IEmailDataRepository? repo = null, INameGenerator? names = null, IRandom? random = null)
    {
        _repo = repo ?? new EmbeddedEmailDataRepository();
        _random = random ?? DefaultRandom.Shared;
        _names = names ?? new NameGenerator(random: _random);
    }

    public Email GenerateEmail(EmailGeneratorOptions? options = null)
    {
        options ??= new EmailGeneratorOptions();
        var rng = _random.For(options);

        var domain = PickDomain(options, rng);
        var local = options.Style == EmailStyle.Random ? RandomLocalPart(rng) : NameBasedLocalPart(options, rng);

        int max = Math.Clamp(options.MaxLocalPartLength ?? 64, 1, 64);
        if (local.Length > max) local = local[..max].TrimEnd('.', '_', '-');

        var email = $"{local}@{domain}";
        return new Email(options.Lowercase ? email.ToLowerInvariant() : email);
    }

    private string PickDomain(EmailGeneratorOptions options, IRandom rng)
    {
        if (options.AllowedDomains is { } allowed)
        {
            if (allowed.Count == 0) throw new ArgumentException("AllowedDomains must not be empty.", nameof(options));
            return rng.Pick(allowed);
        }
        if (options.SafeDomains) return rng.Pick(SafeDomainPool);

        var domains = _repo.GetEmailDomains();
        if (domains.Count == 0) throw new InvalidOperationException("No e-mail domains available.");
        return rng.PickWeighted(domains, d => d.Weight).Domain;
    }

    private string NameBasedLocalPart(EmailGeneratorOptions options, IRandom rng)
    {
        string first, last;
        if (options.FirstName is null || options.LastName is null)
        {
            var name = _names.GenerateName(new Names.Options.NameGeneratorOptions { Seed = rng.Next(int.MaxValue) });
            first = options.FirstName ?? name.FirstName;
            last = options.LastName ?? name.LastName;
        }
        else
        {
            first = options.FirstName;
            last = options.LastName;
        }

        var f = Sanitize(first);
        var l = Sanitize(last);
        if (f.Length == 0 && l.Length == 0) return RandomLocalPart(rng);
        if (f.Length == 0) f = l;
        if (l.Length == 0) l = f;

        string number = rng.Chance(0.5) ? rng.Next(70, 100).ToString() : rng.Next(1, 1000).ToString();
        return rng.Next(10) switch
        {
            0 or 1 or 2 => $"{f}.{l}",
            3 => $"{f}{l}",
            4 => $"{l}.{f}",
            5 => $"{f[0]}{l}",
            6 => $"{f}.{l}{number}",
            7 => $"{l}{f[0]}",
            8 => $"{f}_{l}",
            _ => $"{f}{l}{number}"
        };
    }

    private static string RandomLocalPart(IRandom rng)
    {
        int len = rng.Next(8, 13);
        var sb = new StringBuilder(len);
        sb.Append((char)('a' + rng.Next(26))); // start with a letter
        for (int i = 1; i < len; i++)
        {
            int bucket = rng.Next(36);
            sb.Append(bucket < 26 ? (char)('a' + bucket) : (char)('0' + bucket - 26));
        }
        return sb.ToString();
    }

    // "Jan-Křtitel Nováková" → "jankrtitel" / "novakova": ASCII letters and digits only.
    private static string Sanitize(string s)
    {
        var plain = CzechText.RemoveDiacritics(s).ToLowerInvariant();
        var sb = new StringBuilder(plain.Length);
        foreach (var c in plain)
            if (c is >= 'a' and <= 'z' or >= '0' and <= '9') sb.Append(c);
        return sb.ToString();
    }
}
