using Novak.Net.Addresses.Models;

namespace Novak.Net.Addresses;

/// <summary>Provides the pool of addresses to sample from. Each address is equally likely to be picked.</summary>
public interface IAddressDataRepository
{
    IReadOnlyList<Address> GetAddresses();
}
