using Novak.Net.Common.Options;

namespace Novak.Net.BankAccounts.Options;

public sealed class BankAccountGeneratorOptions : ISeededOptions
{
    public int? Seed { get; init; }

    /// <summary>Override the bank code (4 digits, e.g. "0800"). Picked randomly when null.</summary>
    public string? BankCode { get; init; }

    /// <summary>
    /// When true, always generate a non-zero prefix (pre-account number).
    /// When false, always generate a zero prefix (no prefix displayed).
    /// When null (default), randomly decide.
    /// </summary>
    public bool? WithPrefix { get; init; }
}
