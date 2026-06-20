using Novak.Net.BankAccounts;
using Novak.Net.Emails;
using Novak.Net.Names;
using Novak.Net.PhoneNumbers;
using Novak.Net.RodneCislo;

namespace Novak.Net;

public interface INovakClient
{
    INameGenerator Names { get; }
    IEmailGenerator Emails { get; }
    IPhoneNumberGenerator PhoneNumbers { get; }
    IRodneCisloGenerator RodneCislo { get; }
    IBankAccountGenerator BankAccounts { get; }
}
