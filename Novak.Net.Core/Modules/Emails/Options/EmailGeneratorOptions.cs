using System.Collections.Generic;
using Novak.Net.Core.Options;

namespace Novak.Net.Core.Modules.Emails.Options;

public sealed class EmailGeneratorOptions : ISeededOptions, IRegionalOptions
{
    public int? Seed { get; init; }
    public string? Region { get; init; }
    public bool Lowercase { get; init; } = true;
    public IReadOnlyList<string>? AllowedDomains { get; init; }
    public int? MaxLocalPartLength { get; init; }
}
