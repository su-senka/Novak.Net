namespace Novak.Net.Common.Randomization;

/// <summary>
/// Thread-safe <see cref="IRandom"/> backed by <see cref="System.Random"/>.
/// </summary>
public sealed class DefaultRandom : IRandom
{
    private readonly Random _random;
    private readonly object? _sync;

    private DefaultRandom(Random random, bool synchronized)
    {
        _random = random;
        _sync = synchronized ? new object() : null;
    }

    /// <summary>Process-wide unseeded instance (wraps <see cref="Random.Shared"/>).</summary>
    public static IRandom Shared { get; } = new DefaultRandom(Random.Shared, synchronized: false);

    /// <summary>
    /// Creates a new instance. With a seed, the sequence of values is reproducible across runs
    /// on the same .NET major version.
    /// </summary>
    public static DefaultRandom Create(int? seed) =>
        new(seed.HasValue ? new Random(seed.Value) : new Random(), synchronized: true);

    public int Next(int maxExclusive)
    {
        if (_sync is null) return _random.Next(maxExclusive);
        lock (_sync) return _random.Next(maxExclusive);
    }

    public int Next(int minInclusive, int maxExclusive)
    {
        if (_sync is null) return _random.Next(minInclusive, maxExclusive);
        lock (_sync) return _random.Next(minInclusive, maxExclusive);
    }

    public double NextDouble()
    {
        if (_sync is null) return _random.NextDouble();
        lock (_sync) return _random.NextDouble();
    }
}
