using Novak.Net.Core.Modules.Emails.Models;
using Novak.Net.Core.Modules.Emails.Options;

namespace Novak.Net.Core.Modules.Emails;

public interface IEmailGenerator
{
    Email GenerateEmail(EmailGeneratorOptions? options = null);
}

