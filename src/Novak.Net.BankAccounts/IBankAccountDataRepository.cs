using System.Collections.Generic;

namespace Novak.Net.BankAccounts;

public interface IBankAccountDataRepository
{
    IReadOnlyList<string> GetBankCodes();
}
