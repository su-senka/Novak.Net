using System.Collections.Generic;

namespace Novak.Net.Emails;

public interface IEmailDataRepository
{
    IReadOnlyList<string> GetEmailDomains(string? region);
}

