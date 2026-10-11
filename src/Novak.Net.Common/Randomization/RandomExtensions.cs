using Novak.Net.Common.Options;

namespace Novak.Net.Common.Randomization;

/// <summary>Convenience helpers on top of <see cref="IRandom"/>.</summary>
public static class RandomExtensions
{
    /// <summary>
    /// Returns a fresh seeded instance when <paramref name="options"/> carries a seed, otherwise <paramref name="fallback"/>.
    /// </summary>
    public static IRandom For(this IRandom fallback, ISeededOptions? options) =>
        options?.Seed is int seed ? DefaultRandom.Create(seed) : fallback;

    /// <summary>Returns true with the given probability (0–1).</summary>
    public static bool Chance(this IRandom random, double probability) => random.NextDouble() < probability;

    /// <summary>Picks a uniformly random element.</summary>
    public static T Pick<T>(this IRandom random, IReadOnlyList<T> items)
    {
        if (items.Count == 0) throw new ArgumentException("Cannot pick from an empty list.", nameof(items));
        return items[random.Next(items.Count)];
    }

    /// <summary>Picks an element with probability proportional to its non-negative weight.</summary>
    public static T PickWeighted<T>(this IRandom random, IReadOnlyList<T> items, Func<T, double> weight)
    {
        double total = 0;
        foreach (var item in items) total += Math.Max(0, weight(item));
        if (total <= 0) throw new ArgumentException("At least one item must have a positive weight.", nameof(items));

        double target = random.NextDouble() * total;
        T? last = default;
        foreach (var item in items)
        {
            double w = Math.Max(0, weight(item));
            if (w <= 0) continue;
            last = item;
            target -= w;
            if (target < 0) return item;
        }
        return last!; // floating-point rounding guard
    }
}
