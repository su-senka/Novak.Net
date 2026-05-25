using System;
using System.Text;
using Novak.Net.Emails.Models;
using Novak.Net.Emails.Options;
using Novak.Net.Common.Randomization;

namespace Novak.Net.Emails;

public class EmailGenerator : IEmailGenerator
{
    private readonly IEmailDataRepository _repo;

    public EmailGenerator(IEmailDataRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public Email GenerateEmail(EmailGeneratorOptions? options = null)
    {
        options ??= new EmailGeneratorOptions();
        var rng = DefaultRandom.Create(options.Seed);

        var domains = options.AllowedDomains ?? _repo.GetEmailDomains(options.Region);
        if (domains is null || domains.Count == 0)
            throw new InvalidOperationException("No email domains available.");
        var domain = domains[rng.Next(domains.Count)];

        int len = 8 + rng.Next(5);
        if (options.MaxLocalPartLength is int max && max > 0)
            len = Math.Min(len, max);

        var sb = new StringBuilder(len);
        for (int i = 0; i < len; i++)
        {
            int bucket = rng.Next(36);
            sb.Append(bucket < 26 ? (char)('a' + bucket) : (char)('0' + (bucket - 26)));
        }

        var local = sb.ToString();
        var email = options.Lowercase ? $"{local}@{domain.ToLowerInvariant()}" : $"{local}@{domain}";
        return new Email(email);
    }
}
