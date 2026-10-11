using System.Diagnostics.CodeAnalysis;

namespace Novak.Net.BankAccounts.Models;

/// <summary>
/// A Czech bank account number. Components are stored zero-padded: <see cref="Prefix"/> 6 digits,
/// <see cref="AccountNumber"/> 10 digits, <see cref="BankCode"/> 4 digits.
/// </summary>
public readonly record struct BankAccount(string Prefix, string AccountNumber, string BankCode)
{
    // CNB weights for the 10-digit account number; the 6-digit prefix uses the last six.
    private static readonly int[] Weights = [6, 3, 7, 9, 10, 5, 8, 4, 2, 1];

    /// <summary>Name of the bank, when known.</summary>
    public string? BankName { get; init; }

    /// <summary>SWIFT/BIC of the bank, when known.</summary>
    public string? Bic { get; init; }

    /// <summary>Czech local format without leading zeros: "19-2000145399/0800" or "2000145399/0800".</summary>
    public string ToLocalFormat()
    {
        var prefix = Prefix.TrimStart('0');
        var account = AccountNumber.TrimStart('0');
        return prefix.Length > 0 ? $"{prefix}-{account}/{BankCode}" : $"{account}/{BankCode}";
    }

    /// <summary>Czech IBAN (24 characters), e.g. "CZ6508000000192000145399"; spaced: "CZ65 0800 0000 1920 0014 5399".</summary>
    public string ToIban(bool spaced = false)
    {
        var bban = BankCode + Prefix + AccountNumber;
        // ISO 13616: move "CZ00" to the end, letters → numbers (C=12, Z=35), check = 98 - mod 97.
        int remainder = 0;
        foreach (char c in bban + "123500")
            remainder = (remainder * 10 + (c - '0')) % 97;
        var iban = $"CZ{98 - remainder:D2}{bban}";
        return spaced ? string.Join(' ', iban.Chunk(4).Select(c => new string(c))) : iban;
    }

    public override string ToString() => ToLocalFormat();

    /// <summary>
    /// Parses "prefix-account/bank" or "account/bank" (bank code required) and validates both checksums.
    /// </summary>
    public static bool TryParse([NotNullWhen(true)] string? value, out BankAccount account)
    {
        account = default;
        if (string.IsNullOrWhiteSpace(value)) return false;

        var parts = value.Trim().Split('/');
        if (parts.Length != 2 || !IsDigits(parts[1], 4, 4)) return false;

        var numbers = parts[0].Split('-');
        string prefix = numbers.Length == 2 ? numbers[0] : "0";
        string number = numbers[^1];
        if (numbers.Length > 2 || !IsDigits(prefix, 1, 6) || !IsDigits(number, 2, 10)) return false;

        prefix = prefix.PadLeft(6, '0');
        number = number.PadLeft(10, '0');
        if (!HasValidChecksum(prefix) || !HasValidChecksum(number)) return false;
        if (number.Count(c => c != '0') < 2) return false; // CNB: account number has at least two non-zero digits

        account = new BankAccount(prefix, number, parts[1]);
        return true;
    }

    /// <summary>True when <paramref name="value"/> is a well-formed account number with valid checksums.</summary>
    public static bool IsValid([NotNullWhen(true)] string? value) => TryParse(value, out _);

    /// <summary>True when the zero-padded 6- or 10-digit string satisfies the CNB weighted mod-11 rule.</summary>
    internal static bool HasValidChecksum(string digits)
    {
        int sum = 0, offset = Weights.Length - digits.Length;
        for (int i = 0; i < digits.Length; i++)
            sum += (digits[i] - '0') * Weights[offset + i];
        return sum % 11 == 0;
    }

    internal static int Weight(int length, int position) => Weights[Weights.Length - length + position];

    private static bool IsDigits(string s, int minLength, int maxLength) =>
        s.Length >= minLength && s.Length <= maxLength && s.All(c => c is >= '0' and <= '9');
}
