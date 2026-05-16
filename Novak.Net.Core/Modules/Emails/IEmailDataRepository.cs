using System.Collections.Generic;

namespace Novak.Net.Core.Modules.Emails;

public interface IEmailDataRepository
{
    IReadOnlyList<string> GetEmailDomains(string? region);
}

