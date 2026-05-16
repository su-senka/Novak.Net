namespace Novak.Net.Core.Options;

public sealed class CzechMintOptions
{
    public int? Seed { get; init; }
    public string Culture { get; init; } = "cs-CZ";
}
