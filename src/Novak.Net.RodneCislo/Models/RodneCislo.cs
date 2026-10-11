using System.Diagnostics.CodeAnalysis;

namespace Novak.Net.RodneCislo.Models;

/// <summary>A Czech birth number (rodné číslo), e.g. "855203/1236".</summary>
public readonly record struct RodneCislo(string Value)
{
    /// <summary>The digits without slash or spaces.</summary>
    public string Digits => (Value ?? "").Replace("/", "").Replace(" ", "");

    /// <summary>Birth date encoded in the number.</summary>
    public DateOnly BirthDate => Decode().BirthDate;

    /// <summary>Gender encoded in the number (<see cref="Models.Gender.Male"/> or <see cref="Models.Gender.Female"/>).</summary>
    public Gender Gender => Decode().Gender;

    public override string ToString() => Value;

    /// <summary>
    /// Checks the format, the encoded date, and the mod-11 checksum, including the historical rules:
    /// 9-digit numbers only for births before 1954; month +20 (men) / +70 (women) only from 2004;
    /// before 1986 a check digit of 0 was allowed when the first nine digits give remainder 10.
    /// </summary>
    public static bool IsValid([NotNullWhen(true)] string? value) => TryParse(value, out _);

    /// <summary>Parses and validates a birth number in "YYMMDD/XXXX", "YYMMDDXXXX" or 9-digit form.</summary>
    public static bool TryParse([NotNullWhen(true)] string? value, out RodneCislo result)
    {
        result = default;
        if (value is null || !TryDecode(value, out _, out _)) return false;
        result = new RodneCislo(value.Trim());
        return true;
    }

    /// <summary>Parses and validates a birth number, throwing <see cref="FormatException"/> when invalid.</summary>
    public static RodneCislo Parse(string value) =>
        TryParse(value, out var result) ? result : throw new FormatException($"'{value}' is not a valid rodné číslo.");

    private (DateOnly BirthDate, Gender Gender) Decode() =>
        TryDecode(Value ?? "", out var date, out var gender)
            ? (date, gender)
            : throw new FormatException($"'{Value}' is not a valid rodné číslo.");

    private static bool TryDecode(string value, out DateOnly birthDate, out Gender gender)
    {
        birthDate = default;
        gender = Gender.Any;

        var digits = value.Trim().Replace("/", "").Replace(" ", "");
        if (digits.Length is not (9 or 10)) return false;
        foreach (var c in digits)
            if (c is < '0' or > '9') return false;

        int yy = int.Parse(digits.AsSpan(0, 2));
        int mm = int.Parse(digits.AsSpan(2, 2));
        int dd = int.Parse(digits.AsSpan(4, 2));

        int year;
        if (digits.Length == 9)
        {
            if (yy >= 54) return false; // 9-digit numbers were issued only until 1953
            year = 1900 + yy;
        }
        else
        {
            year = yy >= 54 ? 1900 + yy : 2000 + yy;
        }

        gender = mm > 50 ? Gender.Female : Gender.Male;
        int month = mm > 50 ? mm - 50 : mm;
        if (month > 20)
        {
            if (year < 2004) return false; // the +20 extension exists only since 2004
            month -= 20;
        }
        if (month is < 1 or > 12) return false;
        if (dd < 1 || dd > DateTime.DaysInMonth(year, month)) return false;
        birthDate = new DateOnly(year, month, dd);

        if (digits.Length == 10)
        {
            long first9 = long.Parse(digits.AsSpan(0, 9));
            int remainder = (int)(first9 % 11);
            int check = digits[9] - '0';
            if (remainder == 10)
                return check == 0 && year < 1986;
            return check == remainder;
        }
        return true;
    }
}
