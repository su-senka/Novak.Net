using System.Collections.Generic;
using Novak.Net.Common.Options;

namespace Novak.Net.Emails.Options;

public sealed class EmailGeneratorOptions : ISeededOptions, IRegionalOptions
{
    public int? Seed { get; init; }
    public string? Region { get; init; }
    public bool Lowercase { get; init; } = true;
    public IReadOnlyList<string>? AllowedDomains { get; init; }
    public int? MaxLocalPartLength { get; init; }
}
