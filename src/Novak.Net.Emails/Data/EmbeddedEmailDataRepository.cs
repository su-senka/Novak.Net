using System.Globalization;
using Novak.Net.Common.Data;

namespace Novak.Net.Emails.Data;

/// <summary>Built-in list of free-mail domains popular in Czechia (seznam.cz, gmail.com, email.cz, ...).</summary>
public sealed class EmbeddedEmailDataRepository : IEmailDataRepository
{
    private static readonly Lazy<(string, double)[]> Domains = new(() =>
        EmbeddedData.ReadRows(typeof(EmbeddedEmailDataRepository).Assembly, "email_domains_cz.csv", hasHeader: true)
            .Select(r => (r[0].Trim(), double.Parse(r[1], CultureInfo.InvariantCulture)))
            .ToArray());

    public IReadOnlyList<(string Domain, double Weight)> GetEmailDomains() => Domains.Value;
}
