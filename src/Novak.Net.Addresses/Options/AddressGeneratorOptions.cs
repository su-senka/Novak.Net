using Novak.Net.Addresses.Models;
using Novak.Net.Common.Options;

namespace Novak.Net.Addresses.Options;

/// <summary>Options for <see cref="AddressGenerator"/>.</summary>
public sealed class AddressGeneratorOptions : ISeededOptions
{
    public int? Seed { get; init; }

    /// <summary>Only addresses in this region (kraj).</summary>
    public CzechRegion? Region { get; init; }

    /// <summary>Only addresses in this municipality, e.g. "Brno" (case-insensitive). Also matches Prague post towns like "Praha 5".</summary>
    public string? City { get; init; }

    /// <summary>Only addresses with a named street (skips village addresses like "Lhota 25").</summary>
    public bool RequireStreet { get; init; }
}
