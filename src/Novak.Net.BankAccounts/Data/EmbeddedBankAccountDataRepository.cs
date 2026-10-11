using System.Globalization;
using Novak.Net.BankAccounts.Models;
using Novak.Net.Common.Data;

namespace Novak.Net.BankAccounts.Data;

/// <summary>Built-in CNB list of Czech bank codes with BICs and market-share-based weights.</summary>
public sealed class EmbeddedBankAccountDataRepository : IBankAccountDataRepository
{
    private static readonly Lazy<Bank[]> Banks = new(() =>
        EmbeddedData.ReadRows(typeof(EmbeddedBankAccountDataRepository).Assembly, "bank_codes_cz.csv", ';', hasHeader: true)
            .Select(r => new Bank(
                r[0],
                r[1],
                string.IsNullOrWhiteSpace(r[2]) ? null : r[2],
                double.Parse(r[3], CultureInfo.InvariantCulture)))
            .ToArray());

    public IReadOnlyList<Bank> GetBanks() => Banks.Value;
}
