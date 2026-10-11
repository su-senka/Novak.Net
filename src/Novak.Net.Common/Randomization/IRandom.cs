namespace Novak.Net.Common.Randomization;

/// <summary>
/// Source of randomness used by all generators. Implement it to plug in your own RNG.
/// </summary>
public interface IRandom
{
    /// <summary>Returns a non-negative integer less than <paramref name="maxExclusive"/>.</summary>
    int Next(int maxExclusive);

    /// <summary>Returns an integer in the range [<paramref name="minInclusive"/>, <paramref name="maxExclusive"/>).</summary>
    int Next(int minInclusive, int maxExclusive);

    /// <summary>Returns a double in the range [0, 1).</summary>
    double NextDouble();
}
