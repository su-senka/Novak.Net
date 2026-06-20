using System;
using Novak.Net.BankAccounts.Models;
using Novak.Net.BankAccounts.Options;
using Novak.Net.Common.Randomization;

namespace Novak.Net.BankAccounts;

public class BankAccountGenerator : IBankAccountGenerator
{
    // Weights for a 10-digit account number (left to right). CNB standard.
    private static readonly int[] AccountWeights = { 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };

    // Weights for a 6-digit prefix = last 6 of AccountWeights.
    private static readonly int[] PrefixWeights = { 10, 5, 8, 4, 2, 1 };

    private readonly IBankAccountDataRepository _repo;

    public BankAccountGenerator(IBankAccountDataRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public BankAccount GenerateBankAccount(BankAccountGeneratorOptions? options = null)
    {
        options ??= new BankAccountGeneratorOptions();
        var rng = DefaultRandom.Create(options.Seed);

        var codes = _repo.GetBankCodes();
        if (codes.Count == 0) throw new InvalidOperationException("No bank codes available.");
        var bankCode = options.BankCode ?? codes[rng.Next(codes.Count)];

        bool usePrefix = options.WithPrefix ?? (rng.Next(3) == 0); // ~33% chance of prefix by default
        var prefix = usePrefix ? GenerateValidNumber(rng, PrefixWeights, 6) : "000000";
        var account = GenerateValidNumber(rng, AccountWeights, 10);

        return new BankAccount(prefix, account, bankCode);
    }

    // Generates a zero-padded numeric string of `length` digits where the weighted sum is 0 mod 11.
    // The last digit (weight = 1) is computed as the check digit.
    private static string GenerateValidNumber(IRandom rng, int[] weights, int length)
    {
        var digits = new int[length];
        int checkDigit;
        do
        {
            for (int i = 0; i < length - 1; i++)
                digits[i] = rng.Next(10);

            int partial = 0;
            for (int i = 0; i < length - 1; i++)
                partial += digits[i] * weights[i];

            // Weight of last position is always 1 in both weight tables, so:
            // (partial + checkDigit * 1) % 11 == 0  =>  checkDigit = (11 - partial%11) % 11
            checkDigit = (11 - partial % 11) % 11;
            // If checkDigit == 10, no single digit satisfies the constraint — regenerate.
        } while (checkDigit == 10);

        digits[length - 1] = checkDigit;
        return string.Concat(digits);
    }

    /// <summary>
    /// Returns true when both the prefix and account number pass the CZ weighted-checksum rule.
    /// Accepts local format "prefix-account/bankCode", "account/bankCode", or raw digits.
    /// </summary>
    public static bool Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        // Strip bank code if present
        var slashIdx = value.IndexOf('/');
        var withoutBank = slashIdx >= 0 ? value[..slashIdx] : value;

        var dashIdx = withoutBank.IndexOf('-');
        string prefixStr, accountStr;
        if (dashIdx >= 0)
        {
            prefixStr = withoutBank[..dashIdx].PadLeft(6, '0');
            accountStr = withoutBank[(dashIdx + 1)..].PadLeft(10, '0');
        }
        else
        {
            prefixStr = "000000";
            accountStr = withoutBank.PadLeft(10, '0');
        }

        if (prefixStr.Length > 6 || accountStr.Length > 10) return false;

        return WeightedSum(prefixStr, PrefixWeights) % 11 == 0
            && WeightedSum(accountStr, AccountWeights) % 11 == 0;
    }

    private static int WeightedSum(string digits, int[] weights)
    {
        int sum = 0;
        int offset = weights.Length - digits.Length;
        for (int i = 0; i < digits.Length; i++)
            sum += (digits[i] - '0') * weights[offset + i];
        return sum;
    }
}
