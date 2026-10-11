using Novak.Net.Common.Options;

namespace Novak.Net.Emails.Options;

/// <summary>How the local part (before '@') is built.</summary>
public enum EmailStyle
{
    /// <summary>From a person's name: jan.novak, jnovak, novak.jan85, ...</summary>
    NameBased,

    /// <summary>Random letters and digits: k3vx9qpa.</summary>
    Random
}

/// <summary>Options for <see cref="EmailGenerator"/>.</summary>
public sealed class EmailGeneratorOptions : ISeededOptions
{
    public int? Seed { get; init; }

    public EmailStyle Style { get; init; } = EmailStyle.NameBased;

    /// <summary>First name to build the address from; a random Czech name is used when null.</summary>
    public string? FirstName { get; init; }

    /// <summary>Surname to build the address from; a random Czech surname is used when null.</summary>
    public string? LastName { get; init; }

    /// <summary>
    /// Use only the reserved domains example.com / example.net / example.org (RFC 2606), so that a test
    /// system can never e-mail a real mailbox. Off by default because real-looking providers are the point
    /// of realistic data; turn it on for anything that might actually send mail.
    /// </summary>
    public bool SafeDomains { get; init; }

    /// <summary>Explicit domain pool, overrides both the built-in list and <see cref="SafeDomains"/>.</summary>
    public IReadOnlyList<string>? AllowedDomains { get; init; }

    /// <summary>Lower-case the whole address (default). When false, the domain keeps the given casing.</summary>
    public bool Lowercase { get; init; } = true;

    /// <summary>Maximum length of the local part (the RFC limit is 64).</summary>
    public int? MaxLocalPartLength { get; init; }
}
