using Novak.Net.Emails;
using Novak.Net.Emails.Data;
using Novak.Net.Names;
using Novak.Net.PhoneNumbers;
using Novak.Net.PhoneNumbers.Data;

namespace Novak.Net;

public class NovakClient : INovakClient
{
    private readonly INameGenerator _nameGenerator;
    private readonly IEmailGenerator _emailGenerator;
    private readonly IPhoneNumberGenerator _phoneNumberGenerator;

    public NovakClient()
    {
        _nameGenerator = new NameGenerator(new DataSource());
        _emailGenerator = new EmailGenerator(new EmbeddedEmailDataRepository());
        _phoneNumberGenerator = new PhoneNumberGenerator(new EmbeddedPhoneNumberDataRepository());
    }

    public INameGenerator Names => _nameGenerator;
    public IEmailGenerator Emails => _emailGenerator;
    public IPhoneNumberGenerator PhoneNumbers => _phoneNumberGenerator;
}

