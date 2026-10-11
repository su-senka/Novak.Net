using Novak.Net.BankAccounts.Models;

namespace Novak.Net.BankAccounts;

/// <summary>Provides the list of banks.</summary>
public interface IBankAccountDataRepository
{
    IReadOnlyList<Bank> GetBanks();
}
