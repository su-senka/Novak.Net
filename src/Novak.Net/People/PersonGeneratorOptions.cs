using Novak.Net.Addresses.Models;
using Novak.Net.Common.Options;
using Novak.Net.Names.Models;

namespace Novak.Net.People;

/// <summary>Options for <see cref="PersonGenerator"/>.</summary>
public sealed class PersonGeneratorOptions : ISeededOptions
{
    public int? Seed { get; init; }

    public Gender Gender { get; init; } = Gender.Any;

    /// <summary>Minimum age in years (inclusive), default 18.</summary>
    public int MinAge { get; init; } = 18;

    /// <summary>Maximum age in years (inclusive), default 80.</summary>
    public int MaxAge { get; init; } = 80;

    /// <summary>
    /// Date the ages are computed against; defaults to today. Set it when you need seeded output that stays
    /// identical on every day the tests run.
    /// </summary>
    public DateOnly? ReferenceDate { get; init; }

    /// <summary>Only addresses in this region.</summary>
    public CzechRegion? Region { get; init; }

    /// <summary>Only addresses in this municipality, e.g. "Brno".</summary>
    public string? City { get; init; }

    /// <summary>Use reserved example.com/.net/.org e-mail domains. See <see cref="Emails.Options.EmailGeneratorOptions.SafeDomains"/>.</summary>
    public bool SafeEmailDomains { get; init; }
}
