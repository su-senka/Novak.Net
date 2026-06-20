using Novak.Net.BankAccounts;
using Novak.Net.BankAccounts.Data;
using Novak.Net.Emails;
using Novak.Net.Emails.Data;
using Novak.Net.Names;
using Novak.Net.PhoneNumbers;
using Novak.Net.PhoneNumbers.Data;
using Novak.Net.RodneCislo;

namespace Novak.Net;

public class NovakClient : INovakClient
{
    private readonly INameGenerator _nameGenerator;
    private readonly IEmailGenerator _emailGenerator;
    private readonly IPhoneNumberGenerator _phoneNumberGenerator;
    private readonly IRodneCisloGenerator _rodneCisloGenerator;
    private readonly IBankAccountGenerator _bankAccountGenerator;

    public NovakClient()
    {
        _nameGenerator = new NameGenerator(new DataSource());
        _emailGenerator = new EmailGenerator(new EmbeddedEmailDataRepository());
        _phoneNumberGenerator = new PhoneNumberGenerator(new EmbeddedPhoneNumberDataRepository());
        _rodneCisloGenerator = new RodneCisloGenerator();
        _bankAccountGenerator = new BankAccountGenerator(new EmbeddedBankAccountDataRepository());
    }

    public INameGenerator Names => _nameGenerator;
    public IEmailGenerator Emails => _emailGenerator;
    public IPhoneNumberGenerator PhoneNumbers => _phoneNumberGenerator;
    public IRodneCisloGenerator RodneCislo => _rodneCisloGenerator;
    public IBankAccountGenerator BankAccounts => _bankAccountGenerator;
}
