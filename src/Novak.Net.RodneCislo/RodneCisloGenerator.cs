using System;
using System.Linq;
using Novak.Net.Common.Randomization;
using Novak.Net.RodneCislo.Models;
using Novak.Net.RodneCislo.Options;

namespace Novak.Net.RodneCislo;

public class RodneCisloGenerator : IRodneCisloGenerator
{
    public Models.RodneCislo Generate(RodneCisloGeneratorOptions? options = null)
    {
        options ??= new RodneCisloGeneratorOptions();
        var rng = DefaultRandom.Create(options.Seed);

        int minYear = options.MinYear ?? 1940;
        int maxYear = options.MaxYear ?? 2005;
        if (minYear > maxYear)
            throw new ArgumentException("MinYear must be less than or equal to MaxYear.");

        int year = rng.Next(minYear, maxYear + 1);
        int yy = year % 100;

        bool isFemale = options.Gender switch
        {
            Gender.Female => true,
            Gender.Male   => false,
            _              => rng.Next(2) == 0
        };

        int month = rng.Next(1, 13);
        int daysInMonth = DateTime.DaysInMonth(year, month);
        int day = rng.Next(1, daysInMonth + 1);

        // Female: month + 50; when all sequences exhausted (rare edge case), month + 70 — not needed for generation
        int encodedMonth = isFemale ? month + 50 : month;

        string digits;
        if (year < 1954)
        {
            int seq = rng.Next(0, 1000);
            digits = $"{yy:D2}{encodedMonth:D2}{day:D2}{seq:D3}";
        }
        else
        {
            // 10-digit format: full number must be divisible by 11.
            // First 9 digits are YYMMDDXXX; check digit d satisfies (n9*10 + d) % 11 == 0.
            int seq;
            int checkDigit;
            do
            {
                seq = rng.Next(0, 1000);
                // Shift n9 one decimal place left, then find the digit that makes the result divisible by 11.
                long n9 = long.Parse($"{yy:D2}{encodedMonth:D2}{day:D2}{seq:D3}");
                checkDigit = (int)((11 - (n9 * 10 % 11)) % 11);
                // checkDigit == 10 means no valid single digit can satisfy divisibility — retry with next seq.
            } while (checkDigit == 10);

            digits = $"{yy:D2}{encodedMonth:D2}{day:D2}{seq:D3}{checkDigit}";
        }

        var value = options.Format == RodneCisloFormat.WithoutSlash
            ? digits
            : $"{digits[..6]}/{digits[6..]}";

        return new Models.RodneCislo(value);
    }

    /// <summary>
    /// Returns true when the given string is a structurally valid Czech birth number.
    /// </summary>
    public static bool Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        var digits = value.Replace("/", "").Replace(" ", "");
        if (digits.Length != 9 && digits.Length != 10) return false;
        if (!digits.All(char.IsDigit)) return false;

        int mm = int.Parse(digits[2..4]);
        int dd = int.Parse(digits[4..6]);

        // Decode month: female adds 50, overflow variant adds 20/70
        int month = mm;
        if (month > 70) month -= 70;
        else if (month > 50) month -= 50;
        else if (month > 20) month -= 20;

        if (month < 1 || month > 12) return false;
        if (dd < 1 || dd > 31) return false;

        // 10-digit numbers must be divisible by 11 (with rare historical exceptions where the last digit is 0)
        if (digits.Length == 10)
        {
            long n = long.Parse(digits);
            return n % 11 == 0;
        }

        return true;
    }
}
