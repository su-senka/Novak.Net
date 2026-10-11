using Novak.Net.Addresses.Models;
using Novak.Net.Addresses.Options;

namespace Novak.Net.Addresses;

/// <summary>Generates Czech postal addresses.</summary>
public interface IAddressGenerator
{
    Address GenerateAddress(AddressGeneratorOptions? options = null);
}
