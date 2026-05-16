using System;

namespace Novak.Net.Core.Randomization;

public sealed class DefaultRandom : IRandom
{
    private readonly Random _random;
    private DefaultRandom(int? seed) => _random = seed.HasValue ? new Random(seed.Value) : new Random();

    public static DefaultRandom Create(int? seed) => new(seed);

    public int Next(int maxExclusive) => _random.Next(maxExclusive);
    public int Next(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);
    public double NextDouble() => _random.NextDouble();
}