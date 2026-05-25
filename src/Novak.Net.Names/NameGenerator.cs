using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Novak.Net.Names.Models;
using Novak.Net.Names.Options;
using Novak.Net.Names.Sampling;
using Novak.Net.Common.Options;
using Novak.Net.Common.Randomization;

namespace Novak.Net.Names;

public sealed class NameGenerator : INameGenerator
{
    private readonly IDataSource _repo;
    private static readonly CultureInfo CsCulture = new("cs-CZ");

    public NameGenerator(IDataSource repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public string GenerateFirstName(NameGeneratorOptions? options = null)
        => GenerateOne(NamePart.First, options ?? new NameGeneratorOptions(), rng: null);

    public string GenerateSurname(NameGeneratorOptions? options = null)
        => GenerateOne(NamePart.Surname, options ?? new NameGeneratorOptions(), rng: null);

    public string GenerateFullName(NameGeneratorOptions? options = null)
    {
        var opts = options ?? new NameGeneratorOptions();
        var rng = DefaultRandom.Create(opts.Seed);
        var first = GenerateOne(NamePart.First, opts, rng);
        var last = GenerateOne(NamePart.Surname, opts, rng);
        return first + " " + last;
    }

    public IEnumerable<string> GenerateMany(Func<NameGeneratorOptions> optionsFactory, int count)
    {
        if (optionsFactory is null) throw new ArgumentNullException(nameof(optionsFactory));
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
        DefaultRandom? seeded = null;
        for (int i = 0; i < count; i++)
        {
            var opts = optionsFactory();
            if (opts is null) throw new ArgumentException("optionsFactory returned null", nameof(optionsFactory));
            IRandom? rng = null;
            if (opts.Seed.HasValue)
            {
                seeded ??= DefaultRandom.Create(opts.Seed);
                rng = seeded;
            }
            yield return GenerateOne(NamePart.First, opts, rng) + " " + GenerateOne(NamePart.Surname, opts, rng);
        }
    }

    private string GenerateOne(NamePart part, NameGeneratorOptions opts, IRandom? rng)
    {
        ValidateOptions(opts);
        IReadOnlyList<NameRecord> pool = part == NamePart.First
            ? _repo.GetFirstNames(opts)
            : _repo.GetSurnames(opts);

        pool = ApplyIncludeExcludeAndBasicFilters(pool, part, opts, CsCulture);
        pool = ApplyFrequencyBucket(pool, opts);

        if (pool.Count == 0)
            throw new InvalidOperationException($"No {part} names available after applying filters.");

        int n = pool.Count;
        var weights = ArrayPool<double>.Shared.Rent(n);
        try
        {
            for (int i = 0; i < n; i++)
                weights[i] = Math.Max(0, pool[i].Frequency);

            var random = rng ?? DefaultRandom.Create(opts.Seed);

            if (n < 64)
            {
                double sum = 0;
                for (int i = 0; i < n; i++) sum += weights[i];
                if (sum <= 0) throw new InvalidOperationException("All weights are zero.");
                double r = random.NextDouble() * sum;
                double acc = 0;
                int index = 0;
                for (int i = 0; i < n; i++) { acc += weights[i]; if (r <= acc) { index = i; break; } }
                return TransformOutput(pool[index], part, opts, CsCulture);
            }
            else
            {
                var span = new ReadOnlySpan<double>(weights, 0, n);
                var sampler = new AliasSampler(span);
                return TransformOutput(pool[sampler.Next(random)], part, opts, CsCulture);
            }
        }
        finally
        {
            ArrayPool<double>.Shared.Return(weights);
        }
    }

    private static void ValidateOptions(NameGeneratorOptions opts)
    {
        if (opts.MinLength.HasValue && opts.MaxLength.HasValue && opts.MinLength > opts.MaxLength)
            throw new ArgumentException("MinLength cannot be greater than MaxLength.");
        if (opts.StartsWith.HasValue && !char.IsLetter(opts.StartsWith.Value))
            throw new ArgumentException("StartsWith must be a letter.");
        if (opts.FrequencyBucket is not null)
        {
            var b = opts.FrequencyBucket;
            if (b != "Common" && b != "Normal" && b != "Rare")
                throw new ArgumentException("FrequencyBucket must be one of: Common, Normal, Rare");
        }
    }

    private static IReadOnlyList<NameRecord> ApplyIncludeExcludeAndBasicFilters(
        IReadOnlyList<NameRecord> pool, NamePart part, NameGeneratorOptions opts, CultureInfo culture)
    {
        if (pool.Count == 0) return pool;

        HashSet<string>? include = null, exclude = null;
        if (opts.Include is { Count: > 0 })
            include = new HashSet<string>(NormalizeSet(opts.Include, opts, culture));
        if (opts.Exclude is { Count: > 0 })
            exclude = new HashSet<string>(NormalizeSet(opts.Exclude, opts, culture));

        var list = new List<NameRecord>(pool.Count);
        foreach (var r in pool)
        {
            string candidateForFilter = NormalizeForCompare(r.Text, opts, culture);
            if (opts.StartsWith.HasValue)
            {
                var ch = NormalizeCharForCompare(opts.StartsWith.Value, opts, culture);
                if (candidateForFilter.Length == 0) continue;
                if (char.ToUpper(candidateForFilter[0], culture) != char.ToUpper(ch, culture)) continue;
            }
            if (opts.MinLength.HasValue && candidateForFilter.Length < opts.MinLength.Value) continue;
            if (opts.MaxLength.HasValue && candidateForFilter.Length > opts.MaxLength.Value) continue;
            if (include is { Count: > 0 } && !include.Contains(candidateForFilter)) continue;
            if (exclude is { Count: > 0 } && exclude.Contains(candidateForFilter)) continue;
            list.Add(r);
        }
        return list;
    }

    private static IEnumerable<string> NormalizeSet(IReadOnlyCollection<string> set, NameGeneratorOptions opts, CultureInfo culture)
    {
        foreach (var s in set)
        {
            if (!string.IsNullOrWhiteSpace(s))
                yield return NormalizeForCompare(s.Trim(), opts, culture);
        }
    }

    private static string NormalizeForCompare(string s, NameGeneratorOptions opts, CultureInfo culture)
        => (opts.WithDiacritics ? s : RemoveDiacritics(s)).ToUpper(culture);

    private static char NormalizeCharForCompare(char c, NameGeneratorOptions opts, CultureInfo culture)
        => NormalizeForCompare(new string(c, 1), opts, culture)[0];

    private static string TransformOutput(NameRecord record, NamePart part, NameGeneratorOptions opts, CultureInfo culture)
    {
        string text = record.Text;
        if (part == NamePart.Surname)
            text = MaybeFeminize(text, record, opts, culture);
        if (!opts.WithDiacritics) text = RemoveDiacritics(text);
        return ApplyCasing(text, opts.Casing, culture);
    }

    private static string ApplyCasing(string text, Casing casing, CultureInfo culture) => casing switch
    {
        Casing.Upper => text.ToUpper(culture),
        Casing.Lower => text.ToLower(culture),
        Casing.Title => culture.TextInfo.ToTitleCase(text.ToLower(culture)),
        _ => text
    };

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalizedString.Length);
        foreach (var c in normalizedString)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    private static string MaybeFeminize(string surname, NameRecord record, NameGeneratorOptions opts, CultureInfo culture)
    {
        if (opts.Gender != Gender.Female) return surname;
        if (!record.Genderable || record.Foreign || record.Indeclinable) return surname;
        if (surname.EndsWith("ov", StringComparison.OrdinalIgnoreCase)) return surname;

        var exceptions = new Dictionary<string, string>(StringComparer.Create(culture, true))
        {
            ["Novk"] = "Novkov",
            ["Dvořk"] = "Dvořkov",
            ["Svoboda"] = "Svobodov",
            ["Kučera"] = "Kučerov",
        };
        if (exceptions.TryGetValue(surname, out var fem)) return fem;

        if (EndsWithAny(surname, culture, "ek", "ec", "el") || IsCzechConsonant(surname[^1], culture))
            return surname + "ov";

        return surname + "ov";
    }

    private static bool EndsWithAny(string s, CultureInfo culture, params string[] suffixes)
    {
        foreach (var suf in suffixes)
            if (s.EndsWith(suf, true, culture)) return true;
        return false;
    }

    private static bool IsCzechConsonant(char c, CultureInfo culture)
    {
        var ch = char.ToLower(c, culture);
        return ch is >= 'a' and <= 'z' && !("aeiouůy".Contains(ch, StringComparison.Ordinal));
    }

    private static IReadOnlyList<NameRecord> ApplyFrequencyBucket(IReadOnlyList<NameRecord> pool, NameGeneratorOptions opts)
    {
        if (pool.Count == 0 || string.IsNullOrEmpty(opts.FrequencyBucket)) return pool;
        var sorted = pool.OrderByDescending(r => r.Frequency).ToArray();
        double total = sorted.Sum(r => Math.Max(0, r.Frequency));
        if (total <= 0) return sorted;
        double cum = 0;
        var top = new List<NameRecord>();
        var mid = new List<NameRecord>();
        var bot = new List<NameRecord>();
        foreach (var r in sorted)
        {
            double w = Math.Max(0, r.Frequency);
            double prev = cum;
            cum += w / total;
            double midpoint = (prev + cum) / 2.0;
            if (midpoint <= 0.2) top.Add(r);
            else if (midpoint >= 0.8) bot.Add(r);
            else mid.Add(r);
        }
        return opts.FrequencyBucket switch
        {
            "Common" => top,
            "Normal" => mid,
            "Rare" => bot,
            _ => pool
        };
    }
}
