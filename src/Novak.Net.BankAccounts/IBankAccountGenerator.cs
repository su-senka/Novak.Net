using Novak.Net.BankAccounts.Models;
using Novak.Net.BankAccounts.Options;

namespace Novak.Net.BankAccounts;

public interface IBankAccountGenerator
{
    BankAccount GenerateBankAccount(BankAccountGeneratorOptions? options = null);
}
