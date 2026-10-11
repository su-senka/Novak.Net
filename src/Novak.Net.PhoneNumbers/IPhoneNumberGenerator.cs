using Novak.Net.PhoneNumbers.Models;
using Novak.Net.PhoneNumbers.Options;

namespace Novak.Net.PhoneNumbers;

/// <summary>Generates Czech phone numbers.</summary>
public interface IPhoneNumberGenerator
{
    PhoneNumber GeneratePhoneNumber(PhoneNumberGeneratorOptions? options = null);
}
