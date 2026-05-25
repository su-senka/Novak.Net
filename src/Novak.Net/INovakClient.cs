using Novak.Net.Emails;
using Novak.Net.Names;
using Novak.Net.PhoneNumbers;

namespace Novak.Net;

public interface INovakClient
{
    INameGenerator Names { get; }
    IEmailGenerator Emails { get; }
    IPhoneNumberGenerator PhoneNumbers { get; }
}

