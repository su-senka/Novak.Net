using System.Collections.Generic;
using Novak.Net.Core.Modules.Names.Models;
using Novak.Net.Core.Options;

namespace Novak.Net.Core.Modules.Names.Options;

public sealed class NameGeneratorOptions : ISeededOptions, ICasingOptions, IRegionalOptions
{
    public Gender Gender { get; init; } = Gender.Any;
    public bool WithDiacritics { get; init; } = true;
    public Casing Casing { get; init; } = Casing.Original;
    public int? MinLength { get; init; }
    public int? MaxLength { get; init; }
    public char? StartsWith { get; init; }
    public IReadOnlyCollection<string>? Exclude { get; init; }
    public IReadOnlyCollection<string>? Include { get; init; }
    public string? FrequencyBucket { get; init; }
    public string? Region { get; init; }
    public int? Seed { get; init; }
}
