using System.Collections.Generic;

namespace Novak.Net.Core.Modules.PhoneNumbers;

public interface IPhoneNumberDataRepository
{
    IReadOnlyList<string> GetPhonePrefixes(string? region, bool mobileOnly);
    string GetCountryCode(string? region);
}

