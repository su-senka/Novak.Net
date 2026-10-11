using Novak.Net.BankAccounts.Models;
using Novak.Net.BankAccounts.Options;

namespace Novak.Net.BankAccounts;

/// <summary>Generates Czech bank account numbers.</summary>
public interface IBankAccountGenerator
{
    BankAccount GenerateBankAccount(BankAccountGeneratorOptions? options = null);
}
