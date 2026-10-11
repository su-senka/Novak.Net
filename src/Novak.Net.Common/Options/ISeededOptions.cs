namespace Novak.Net.Common.Options;

/// <summary>Options that can pin a generator call to a fixed seed.</summary>
public interface ISeededOptions
{
    /// <summary>
    /// When set, the call is fully deterministic: the same seed always yields the same value.
    /// Leave null to draw from the generator's own random source (which may itself be seeded).
    /// </summary>
    int? Seed { get; }
}
