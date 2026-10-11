using Novak.Net.Common.Randomization;
using Novak.Net.RodneCislo.Models;
using Novak.Net.RodneCislo.Options;

namespace Novak.Net.RodneCislo;

/// <summary>
/// Generates valid Czech birth numbers: 9 digits for births before 1954, otherwise 10 digits divisible by 11.
/// Women have 50 added to the month.
/// </summary>
public class RodneCisloGenerator : IRodneCisloGenerator
{
    private const int MinSupportedYear = 1900;
    private const int MaxSupportedYear = 2053;

    private readonly IRandom _random;

    /// <param name="random">Random source used when options carry no seed.</param>
    public RodneCisloGenerator(IRandom? random = null)
    {
        _random = random ?? DefaultRandom.Shared;
    }

    public Models.RodneCislo Generate(RodneCisloGeneratorOptions? options = null)
    {
        options ??= new RodneCisloGeneratorOptions();
        var rng = _random.For(options);

        var date = options.BirthDate ?? RandomDate(options, rng);
        if (date.Year is < MinSupportedYear or > MaxSupportedYear)
            throw new ArgumentOutOfRangeException(nameof(options), $"Birth year must be between {MinSupportedYear} and {MaxSupportedYear}.");

        bool female = options.Gender switch
        {
            Gender.Female => true,
            Gender.Male => false,
            _ => rng.Chance(0.5)
        };

        string head = $"{date.Year % 100:D2}{date.Month + (female ? 50 : 0):D2}{date.Day:D2}";
        string digits;
        if (date.Year < 1954)
        {
            digits = $"{head}{rng.Next(1000):D3}";
        }
        else
        {
            // Pick a serial whose 9-digit prefix leaves a remainder < 10; that remainder is the check digit.
            long first9;
            int remainder;
            do
            {
                first9 = long.Parse($"{head}{rng.Next(1000):D3}");
                remainder = (int)(first9 % 11);
            } while (remainder == 10);
            digits = $"{first9:D9}{remainder}";
        }

        var value = options.Format == RodneCisloFormat.WithoutSlash ? digits : $"{digits[..6]}/{digits[6..]}";
        return new Models.RodneCislo(value);
    }

    /// <summary>Returns true when <paramref name="value"/> is a valid birth number. See <see cref="Models.RodneCislo.IsValid"/>.</summary>
    public static bool Validate(string? value) => Models.RodneCislo.IsValid(value);

    private static DateOnly RandomDate(RodneCisloGeneratorOptions options, IRandom rng)
    {
        int minYear = options.MinYear ?? 1940;
        int maxYear = options.MaxYear ?? 2005;
        if (minYear > maxYear)
            throw new ArgumentException("MinYear must be less than or equal to MaxYear.", nameof(options));
        if (minYear < MinSupportedYear || maxYear > MaxSupportedYear)
            throw new ArgumentOutOfRangeException(nameof(options), $"Years must be between {MinSupportedYear} and {MaxSupportedYear}.");

        var from = new DateOnly(minYear, 1, 1);
        int days = new DateOnly(maxYear, 12, 31).DayNumber - from.DayNumber + 1;
        return from.AddDays(rng.Next(days));
    }
}
