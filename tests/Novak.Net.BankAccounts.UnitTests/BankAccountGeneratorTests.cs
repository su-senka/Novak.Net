using Novak.Net.BankAccounts.Data;
using Novak.Net.BankAccounts.Models;
using Novak.Net.BankAccounts.Options;

namespace Novak.Net.BankAccounts.UnitTests;

public class BankAccountGeneratorTests
{
    private static readonly BankAccountGenerator Generator = new();

    [Test]
    public async Task Generate_Is_Deterministic_For_Same_Seed()
    {
        var options = new BankAccountGeneratorOptions { Seed = 42 };

        await Assert.That(Generator.GenerateBankAccount(options)).IsEqualTo(Generator.GenerateBankAccount(options));
    }

    [Test]
    public async Task Generated_Accounts_Pass_Validation()
    {
        for (int seed = 0; seed < 300; seed++)
        {
            var account = Generator.GenerateBankAccount(new BankAccountGeneratorOptions { Seed = seed, WithPrefix = seed % 2 == 0 });
            await Assert.That(BankAccountGenerator.Validate(account.ToLocalFormat())).IsTrue().Because(account.ToLocalFormat());
            await Assert.That(account.Prefix.Length).IsEqualTo(6);
            await Assert.That(account.AccountNumber.Length).IsEqualTo(10);
        }
    }

    [Test]
    public async Task Random_Banks_Are_Retail_Banks_With_Names()
    {
        var retail = new EmbeddedBankAccountDataRepository().GetBanks().Where(b => b.Weight > 0).Select(b => b.Code).ToHashSet();
        var seen = new HashSet<string>();
        for (int seed = 0; seed < 300; seed++)
        {
            var account = Generator.GenerateBankAccount(new BankAccountGeneratorOptions { Seed = seed });
            await Assert.That(retail).Contains(account.BankCode);
            await Assert.That(account.BankName).IsNotNull();
            seen.Add(account.BankCode);
        }
        await Assert.That(seen).Contains("0800");
        await Assert.That(seen.Contains("0710")).IsFalse(); // central bank is never picked
    }

    [Test]
    public async Task Bank_List_Matches_Cnb_Codes()
    {
        var banks = new EmbeddedBankAccountDataRepository().GetBanks().ToDictionary(b => b.Code);

        await Assert.That(banks["0800"].Bic).IsEqualTo("GIBACZPX");
        await Assert.That(banks["2250"].Name).Contains("CREDITAS");
        await Assert.That(banks["3030"].Name).Contains("Air Bank");
        await Assert.That(banks.Keys.All(k => k.Length == 4 && k.All(char.IsAsciiDigit))).IsTrue();
    }

    [Test]
    public async Task Known_Iban_Is_Computed_Correctly()
    {
        var account = new BankAccount("000019", "2000145399", "0800");

        await Assert.That(account.ToLocalFormat()).IsEqualTo("19-2000145399/0800");
        await Assert.That(account.ToIban()).IsEqualTo("CZ6508000000192000145399");
        await Assert.That(account.ToIban(spaced: true)).IsEqualTo("CZ65 0800 0000 1920 0014 5399");
    }

    [Test]
    public async Task Generated_Ibans_Pass_Mod97()
    {
        for (int seed = 0; seed < 100; seed++)
        {
            var iban = Generator.GenerateBankAccount(new BankAccountGeneratorOptions { Seed = seed }).ToIban();
            await Assert.That(iban.Length).IsEqualTo(24);
            var rearranged = iban[4..] + "1235" + iban[2..4];
            await Assert.That(System.Numerics.BigInteger.Parse(rearranged) % 97).IsEqualTo(System.Numerics.BigInteger.One);
        }
    }

    [Test]
    public async Task BankCode_Override_Is_Respected()
    {
        var known = Generator.GenerateBankAccount(new BankAccountGeneratorOptions { Seed = 1, BankCode = "2010" });
        var unknown = Generator.GenerateBankAccount(new BankAccountGeneratorOptions { Seed = 1, BankCode = "9999" });

        await Assert.That(known.BankCode).IsEqualTo("2010");
        await Assert.That(known.Bic).IsEqualTo("FIOBCZPP");
        await Assert.That(unknown.BankCode).IsEqualTo("9999");
        await Assert.That(unknown.BankName).IsNull();
        await Assert.That(() => Generator.GenerateBankAccount(new BankAccountGeneratorOptions { BankCode = "80" })).Throws<ArgumentException>();
    }

    [Test]
    public async Task WithPrefix_Controls_Prefix()
    {
        for (int seed = 0; seed < 50; seed++)
        {
            var without = Generator.GenerateBankAccount(new BankAccountGeneratorOptions { Seed = seed, WithPrefix = false });
            var with = Generator.GenerateBankAccount(new BankAccountGeneratorOptions { Seed = seed, WithPrefix = true });

            await Assert.That(without.Prefix).IsEqualTo("000000");
            await Assert.That(without.ToLocalFormat().Contains('-')).IsFalse();
            await Assert.That(long.Parse(with.Prefix)).IsGreaterThan(0L);
            await Assert.That(with.ToLocalFormat().Contains('-')).IsTrue();
        }
    }

    [Test]
    public async Task Validate_Rejects_Invalid_Accounts()
    {
        await Assert.That(BankAccountGenerator.Validate("")).IsFalse();
        await Assert.That(BankAccountGenerator.Validate(null)).IsFalse();
        await Assert.That(BankAccountGenerator.Validate("2000145398/0800")).IsFalse(); // tampered digit
        await Assert.That(BankAccountGenerator.Validate("abc/0800")).IsFalse();
        await Assert.That(BankAccountGenerator.Validate("2000145399")).IsFalse();      // no bank code
        await Assert.That(BankAccountGenerator.Validate("2000145399/08")).IsFalse();
        await Assert.That(BankAccountGenerator.Validate("0/0800")).IsFalse();           // all zeros
        await Assert.That(BankAccountGenerator.Validate("1-2-3/0800")).IsFalse();
    }

    [Test]
    public async Task TryParse_Round_Trips()
    {
        await Assert.That(BankAccount.TryParse("19-2000145399/0800", out var parsed)).IsTrue();
        await Assert.That(parsed).IsEqualTo(new BankAccount("000019", "2000145399", "0800"));
    }
}
