using Novak.Net.Core.Modules.Emails;
using Novak.Net.Core.Modules.Emails.Data;
using Novak.Net.Core.Modules.Names;
using Novak.Net.Core.Modules.PhoneNumbers;
using Novak.Net.Core.Modules.PhoneNumbers.Data;

namespace Novak.Net.Core;

public class NovakNetClient
{
    private readonly INameGenerator _nameGenerator;
    private readonly IEmailGenerator _emailGenerator;
    private readonly IPhoneNumberGenerator _phoneNumberGenerator;

    public NovakNetClient()
    {
        var nameRepo = new DataSource();
        _nameGenerator = new NameGenerator(nameRepo);

        var emailRepo = new EmbeddedEmailDataRepository();
        _emailGenerator = new EmailGenerator(emailRepo);

        var phoneRepo = new EmbeddedPhoneNumberDataRepository();
        _phoneNumberGenerator = new PhoneNumberGenerator(phoneRepo);
    }

    public INameGenerator Names => _nameGenerator;
    public IEmailGenerator Emails => _emailGenerator;
    public IPhoneNumberGenerator PhoneNumbers => _phoneNumberGenerator;
}
