using Novak.Net.Common.Options;

namespace Novak.Net.BankAccounts.Options;

/// <summary>Options for <see cref="BankAccountGenerator"/>.</summary>
public sealed class BankAccountGeneratorOptions : ISeededOptions
{
    public int? Seed { get; init; }

    /// <summary>Bank code (4 digits, e.g. "0800"). When null, a retail bank is picked weighted by market share.</summary>
    public string? BankCode { get; init; }

    /// <summary>
    /// True: always generate a non-zero prefix (předčíslí). False: never. Null (default): ~15 % of accounts get one.
    /// </summary>
    public bool? WithPrefix { get; init; }
}
