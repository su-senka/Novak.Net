using System;

namespace Novak.Net.BankAccounts.Models;

/// <summary>
/// Represents a Czech bank account number with its three components stored in canonical zero-padded form.
/// </summary>
public readonly record struct BankAccount(string Prefix, string AccountNumber, string BankCode)
{
    /// <summary>Standard Czech local format, e.g. "19-2000145399/0800" or "2000145399/0800".</summary>
    public string ToLocalFormat()
    {
        long prefixNum = long.Parse(Prefix);
        long accountNum = long.Parse(AccountNumber);
        return prefixNum > 0
            ? $"{prefixNum}-{accountNum}/{BankCode}"
            : $"{accountNum}/{BankCode}";
    }

    /// <summary>Czech IBAN (26 characters), e.g. "CZ6508000000192000145399".</summary>
    public string ToIban()
    {
        // Rearrange: bankCode(4) + prefix(6) + account(10) + country numeric + "00"
        // CZ → C=12, Z=35 → "1235"
        var rearranged = BankCode + Prefix + AccountNumber + "1235" + "00";
        var mod = Mod97(rearranged);
        var check = (98 - mod).ToString("D2");
        return $"CZ{check}{BankCode}{Prefix}{AccountNumber}";
    }

    public override string ToString() => ToLocalFormat();

    // Iterative mod-97 for arbitrary-length numeric strings, avoids BigInteger dependency.
    private static int Mod97(string digits)
    {
        int remainder = 0;
        foreach (char c in digits)
            remainder = (remainder * 10 + (c - '0')) % 97;
        return remainder;
    }
}
