using Novak.Net.Common.Options;
using Novak.Net.Common.Randomization;
using Novak.Net.Common.Text;
using Novak.Net.Names.Models;
using Novak.Net.Names.Options;

namespace Novak.Net.Names;

/// <summary>Default <see cref="INameGenerator"/> implementation.</summary>
public sealed class NameGenerator : INameGenerator
{
    private readonly IDataSource _repo;
    private readonly IRandom _random;

    /// <param name="repo">Name data; defaults to the built-in Czech lists.</param>
    /// <param name="random">Random source used when options carry no seed; defaults to a shared unseeded one.</param>
    public NameGenerator(IDataSource? repo = null, IRandom? random = null)
    {
        _repo = repo ?? new DataSource();
        _random = random ?? DefaultRandom.Shared;
    }

    public PersonName GenerateName(NameGeneratorOptions? options = null)
    {
        var opts = options ?? new NameGeneratorOptions();
        return GenerateName(opts, _random.For(opts));
    }

    public string GenerateFirstName(NameGeneratorOptions? options = null)
    {
        var opts = options ?? new NameGeneratorOptions();
        var rng = _random.For(opts);
        return PickFirstName(opts, rng).Text;
    }

    public string GenerateSurname(NameGeneratorOptions? options = null)
    {
        var opts = options ?? new NameGeneratorOptions();
        var rng = _random.For(opts);
        var gender = opts.Gender == Gender.Any ? (rng.Chance(0.5) ? Gender.Female : Gender.Male) : opts.Gender;
        return PickSurname(opts, gender, rng, applyTextFilters: true);
    }

    public string GenerateFullName(NameGeneratorOptions? options = null) => GenerateName(options).FullName;

    public IEnumerable<string> GenerateMany(Func<NameGeneratorOptions> optionsFactory, int count)
    {
        ArgumentNullException.ThrowIfNull(optionsFactory);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        return Iterate();

        IEnumerable<string> Iterate()
        {
            IRandom? seeded = null;
            for (int i = 0; i < count; i++)
            {
                var opts = optionsFactory() ?? throw new InvalidOperationException("optionsFactory returned null.");
                var rng = opts.Seed is int seed ? seeded ??= DefaultRandom.Create(seed) : _random;
                yield return GenerateName(opts, rng).FullName;
            }
        }
    }

    private PersonName GenerateName(NameGeneratorOptions opts, IRandom rng)
    {
        var first = PickFirstName(opts, rng);
        var gender = first.Gender == Gender.Any ? opts.Gender : first.Gender;
        if (gender == Gender.Any) gender = rng.Chance(0.5) ? Gender.Female : Gender.Male;
        var last = PickSurname(opts, gender, rng, applyTextFilters: false);
        return new PersonName(first.Text, last, gender);
    }

    private (string Text, Gender Gender) PickFirstName(NameGeneratorOptions opts, IRandom rng)
    {
        Validate(opts);
        var candidates = _repo.GetFirstNames(opts)
            .Where(r => opts.Gender == Gender.Any || r.Gender == opts.Gender || r.Gender == Gender.Any)
            .Select(r => new Candidate(r, r.Text))
            .ToList();
        var picked = Pick(candidates, opts, rng, applyTextFilters: true, "first name");
        return (Render(picked.Text, opts), picked.Record.Gender);
    }

    private string PickSurname(NameGeneratorOptions opts, Gender gender, IRandom rng, bool applyTextFilters)
    {
        Validate(opts);
        var candidates = _repo.GetSurnames(opts)
            .Select(r => new Candidate(r, gender == Gender.Female ? r.FeminineForm ?? CzechSurnames.Feminize(r.Text) : r.Text))
            .ToList();
        return Render(Pick(candidates, opts, rng, applyTextFilters, "surname").Text, opts);
    }

    private static Candidate Pick(List<Candidate> pool, NameGeneratorOptions opts, IRandom rng, bool applyTextFilters, string what)
    {
        if (applyTextFilters) pool = ApplyTextFilters(pool, opts);
        if (opts.FrequencyBucket is { } bucket) pool = ApplyFrequencyBucket(pool, bucket);
        pool.RemoveAll(c => c.Record.Frequency <= 0);
        if (pool.Count == 0)
            throw new InvalidOperationException($"No {what} matches the given options.");
        return rng.PickWeighted(pool, c => c.Record.Frequency);
    }

    private static void Validate(NameGeneratorOptions opts)
    {
        if (opts.MinLength > opts.MaxLength)
            throw new ArgumentException("MinLength cannot be greater than MaxLength.", nameof(opts));
        if (opts.StartsWith is char c && !char.IsLetter(c))
            throw new ArgumentException("StartsWith must be a letter.", nameof(opts));
    }

    private static List<Candidate> ApplyTextFilters(List<Candidate> pool, NameGeneratorOptions opts)
    {
        var include = Normalize(opts.Include, opts);
        var exclude = Normalize(opts.Exclude, opts);
        string? startsWith = opts.StartsWith is char ch ? Key(ch.ToString(), opts) : null;

        return pool.Where(c =>
        {
            var key = Key(c.Text, opts);
            if (startsWith is not null && !key.StartsWith(startsWith, StringComparison.Ordinal)) return false;
            if (opts.MinLength is int min && c.Text.Length < min) return false;
            if (opts.MaxLength is int max && c.Text.Length > max) return false;
            if (include is not null && !include.Contains(key) && !include.Contains(Key(c.Record.Text, opts))) return false;
            if (exclude is not null && (exclude.Contains(key) || exclude.Contains(Key(c.Record.Text, opts)))) return false;
            return true;
        }).ToList();
    }

    private static HashSet<string>? Normalize(IReadOnlyCollection<string>? names, NameGeneratorOptions opts) =>
        names is { Count: > 0 }
            ? names.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => Key(n.Trim(), opts)).ToHashSet(StringComparer.Ordinal)
            : null;

    // Comparison key: upper-cased, and accent-free when the output is accent-free.
    private static string Key(string s, NameGeneratorOptions opts) =>
        (opts.WithDiacritics ? s : CzechText.RemoveDiacritics(s)).ToUpper(CzechText.Culture);

    private static List<Candidate> ApplyFrequencyBucket(List<Candidate> pool, NameFrequency bucket)
    {
        double total = pool.Sum(c => Math.Max(0, c.Record.Frequency));
        if (total <= 0) return pool;

        var result = new List<Candidate>();
        double cumulative = 0;
        foreach (var c in pool.OrderByDescending(c => c.Record.Frequency))
        {
            double share = Math.Max(0, c.Record.Frequency) / total;
            double midpoint = cumulative + share / 2;
            cumulative += share;
            var band = midpoint <= 0.2 ? NameFrequency.Common : midpoint >= 0.8 ? NameFrequency.Rare : NameFrequency.Normal;
            if (band == bucket) result.Add(c);
        }
        return result;
    }

    private static string Render(string text, NameGeneratorOptions opts)
    {
        if (!opts.WithDiacritics) text = CzechText.RemoveDiacritics(text);
        var culture = CzechText.Culture;
        return opts.Casing switch
        {
            Casing.Upper => text.ToUpper(culture),
            Casing.Lower => text.ToLower(culture),
            Casing.Title => culture.TextInfo.ToTitleCase(text.ToLower(culture)),
            _ => text
        };
    }

    private sealed record Candidate(NameRecord Record, string Text);
}
