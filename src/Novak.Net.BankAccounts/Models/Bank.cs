namespace Novak.Net.BankAccounts.Models;

/// <summary>A bank from the CNB payment system code list.</summary>
/// <param name="Code">4-digit bank code, e.g. "0800".</param>
/// <param name="Name">Registered name.</param>
/// <param name="Bic">SWIFT/BIC code, when the bank has one.</param>
/// <param name="Weight">Relative weight for random selection; 0 means never picked at random.</param>
public sealed record Bank(string Code, string Name, string? Bic, double Weight);
