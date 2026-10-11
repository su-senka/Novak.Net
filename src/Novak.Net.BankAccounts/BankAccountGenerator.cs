using Novak.Net.BankAccounts.Data;
using Novak.Net.BankAccounts.Models;
using Novak.Net.BankAccounts.Options;
using Novak.Net.Common.Randomization;

namespace Novak.Net.BankAccounts;

/// <summary>Generates account numbers that pass the CNB weighted mod-11 checksum (Decree 169/2011).</summary>
public class BankAccountGenerator : IBankAccountGenerator
{
    private readonly IBankAccountDataRepository _repo;
    private readonly IRandom _random;

    /// <param name="repo">Bank list; defaults to the built-in CNB list.</param>
    /// <param name="random">Random source used when options carry no seed.</param>
    public BankAccountGenerator(IBankAccountDataRepository? repo = null, IRandom? random = null)
    {
        _repo = repo ?? new EmbeddedBankAccountDataRepository();
        _random = random ?? DefaultRandom.Shared;
    }

    public BankAccount GenerateBankAccount(BankAccountGeneratorOptions? options = null)
    {
        options ??= new BankAccountGeneratorOptions();
        var rng = _random.For(options);
        var banks = _repo.GetBanks();

        Bank? bank;
        string bankCode;
        if (options.BankCode is { } requested)
        {
            if (requested.Length != 4 || !requested.All(char.IsAsciiDigit))
                throw new ArgumentException("BankCode must be exactly 4 digits.", nameof(options));
            bankCode = requested;
            bank = banks.FirstOrDefault(b => b.Code == requested);
        }
        else
        {
            if (!banks.Any(b => b.Weight > 0)) throw new InvalidOperationException("No bank codes available.");
            bank = rng.PickWeighted(banks, b => b.Weight);
            bankCode = bank.Code;
        }

        bool usePrefix = options.WithPrefix ?? rng.Chance(0.15);
        var prefix = usePrefix ? GenerateNumber(rng, 6) : "000000";
        var account = GenerateNumber(rng, 10);

        return new BankAccount(prefix, account, bankCode) { BankName = bank?.Name, Bic = bank?.Bic };
    }

    /// <summary>
    /// Returns true when <paramref name="value"/> ("prefix-account/bank" or "account/bank") is well-formed and both
    /// numbers pass the checksum. See <see cref="BankAccount.TryParse"/>.
    /// </summary>
    public static bool Validate(string? value) => BankAccount.IsValid(value);

    // A zero-padded number of `length` digits with at least two non-zero digits whose weighted sum is ≡ 0 (mod 11).
    // Prefixes are kept short (2–6 significant digits) and account numbers long (8–10), as in real life.
    private static string GenerateNumber(IRandom rng, int length)
    {
        int significant = length == 6 ? rng.Next(2, 7) : rng.Next(8, 11);
        var digits = new int[length];
        while (true)
        {
            Array.Clear(digits);
            int sum = 0;
            for (int i = length - significant; i < length - 1; i++)
            {
                digits[i] = i == length - significant ? rng.Next(1, 10) : rng.Next(10);
                sum += digits[i] * BankAccount.Weight(length, i);
            }

            int check = (11 - sum % 11) % 11; // last weight is 1
            if (check == 10) continue;
            digits[length - 1] = check;

            if (digits.Count(d => d != 0) >= 2) return string.Concat(digits);
        }
    }
}
