namespace Novak.Net.Emails;

/// <summary>Provides the pool of e-mail domains.</summary>
public interface IEmailDataRepository
{
    /// <summary>Domains with relative weights.</summary>
    IReadOnlyList<(string Domain, double Weight)> GetEmailDomains();
}
