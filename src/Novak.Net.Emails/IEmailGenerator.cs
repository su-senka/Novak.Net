using Novak.Net.Emails.Models;
using Novak.Net.Emails.Options;

namespace Novak.Net.Emails;

public interface IEmailGenerator
{
    Email GenerateEmail(EmailGeneratorOptions? options = null);
}

