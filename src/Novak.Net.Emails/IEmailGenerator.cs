using Novak.Net.Emails.Models;
using Novak.Net.Emails.Options;

namespace Novak.Net.Emails;

/// <summary>Generates e-mail addresses.</summary>
public interface IEmailGenerator
{
    Email GenerateEmail(EmailGeneratorOptions? options = null);
}
