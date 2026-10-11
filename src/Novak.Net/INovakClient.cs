using Novak.Net.Addresses;
using Novak.Net.BankAccounts;
using Novak.Net.Emails;
using Novak.Net.Names;
using Novak.Net.People;
using Novak.Net.PhoneNumbers;
using Novak.Net.RodneCislo;

namespace Novak.Net;

/// <summary>Entry point to all Novak.Net generators.</summary>
public interface INovakClient
{
    INameGenerator Names { get; }
    IEmailGenerator Emails { get; }
    IPhoneNumberGenerator PhoneNumbers { get; }
    IAddressGenerator Addresses { get; }
    IRodneCisloGenerator RodneCislo { get; }
    IBankAccountGenerator BankAccounts { get; }
    IPersonGenerator People { get; }
}
