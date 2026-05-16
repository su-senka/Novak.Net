using Novak.Net.Core.Modules.PhoneNumbers.Models;
using Novak.Net.Core.Modules.PhoneNumbers.Options;

namespace Novak.Net.Core.Modules.PhoneNumbers;

public interface IPhoneNumberGenerator
{
    PhoneNumber GeneratePhoneNumber(PhoneNumberGeneratorOptions? options = null);
}

