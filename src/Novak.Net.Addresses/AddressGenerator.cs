using System.Collections.Concurrent;
using Novak.Net.Addresses.Data;
using Novak.Net.Addresses.Models;
using Novak.Net.Addresses.Options;
using Novak.Net.Common.Randomization;

namespace Novak.Net.Addresses;

/// <summary>Default <see cref="IAddressGenerator"/> implementation.</summary>
public class AddressGenerator : IAddressGenerator
{
    private readonly IAddressDataRepository _repo;
    private readonly IRandom _random;
    private readonly ConcurrentDictionary<(CzechRegion?, string?, bool), Address[]> _pools = new();

    /// <param name="repo">Address data; defaults to the built-in RÚIAN sample.</param>
    /// <param name="random">Random source used when options carry no seed.</param>
    public AddressGenerator(IAddressDataRepository? repo = null, IRandom? random = null)
    {
        _repo = repo ?? new EmbeddedAddressDataRepository();
        _random = random ?? DefaultRandom.Shared;
    }

    public Address GenerateAddress(AddressGeneratorOptions? options = null)
    {
        options ??= new AddressGeneratorOptions();
        var rng = _random.For(options);
        var city = string.IsNullOrWhiteSpace(options.City) ? null : options.City.Trim();

        var pool = _pools.GetOrAdd((options.Region, city, options.RequireStreet), key => _repo.GetAddresses()
            .Where(a => key.Item1 is null || a.Region == key.Item1)
            .Where(a => key.Item2 is null
                        || string.Equals(a.Municipality, key.Item2, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(a.PostTown, key.Item2, StringComparison.OrdinalIgnoreCase))
            .Where(a => !key.Item3 || a.Street is not null)
            .ToArray());

        if (pool.Length == 0)
            throw new InvalidOperationException("No address matches the given options.");
        return rng.Pick(pool);
    }
}
