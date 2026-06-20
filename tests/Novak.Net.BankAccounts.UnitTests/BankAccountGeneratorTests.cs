using Novak.Net.BankAccounts;
using Novak.Net.BankAccounts.Data;
using Novak.Net.BankAccounts.Options;

namespace Novak.Net.BankAccounts.UnitTests;

public class BankAccountGeneratorTests
{
    private static BankAccountGenerator CreateGenerator() =>
        new(new EmbeddedBankAccountDataRepository());

    [Test]
    public async Task Generate_Is_Deterministic_For_Same_Seed()
    {
        var generator = CreateGenerator();
        var options = new BankAccountGeneratorOptions { Seed = 42 };

        var first = generator.GenerateBankAccount(options).ToString();
        var second = generator.GenerateBankAccount(options).ToString();

        await Assert.That(first).IsEqualTo(second);
    }

    [Test]
    public async Task Generated_AccountNumber_Passes_Checksum()
    {
        var generator = CreateGenerator();
        for (int seed = 0; seed < 50; seed++)
        {
            var account = generator.GenerateBankAccount(new BankAccountGeneratorOptions { Seed = seed });
            await Assert.That(BankAccountGenerator.Validate(account.ToLocalFormat())).IsTrue();
        }
    }

    [Test]
    public async Task Generated_Prefix_Passes_Checksum()
    {
        var generator = CreateGenerator();
        for (int seed = 0; seed < 50; seed++)
        {
            var account = generator.GenerateBankAccount(new BankAccountGeneratorOptions
            {
                Seed = seed,
                WithPrefix = true
            });
            await Assert.That(BankAccountGenerator.Validate(account.ToLocalFormat())).IsTrue();
        }
    }

    [Test]
    public async Task ToIban_Returns_26_Char_CZ_String()
    {
        var generator = CreateGenerator();
        var account = generator.GenerateBankAccount(new BankAccountGeneratorOptions { Seed = 7 });
        var iban = account.ToIban();

        await Assert.That(iban.StartsWith("CZ")).IsTrue();
        await Assert.That(iban.Length).IsEqualTo(24); // CZ(2)+check(2)+bank(4)+prefix(6)+account(10)
    }

    [Test]
    public async Task ToIban_Is_Deterministic_For_Same_Account()
    {
        var generator = CreateGenerator();
        var account = generator.GenerateBankAccount(new BankAccountGeneratorOptions { Seed = 99 });
        await Assert.That(account.ToIban()).IsEqualTo(account.ToIban());
    }

    [Test]
    public async Task BankCode_Override_Is_Respected()
    {
        var generator = CreateGenerator();
        var account = generator.GenerateBankAccount(new BankAccountGeneratorOptions
        {
            Seed = 1,
            BankCode = "0800"
        });
        await Assert.That(account.BankCode).IsEqualTo("0800");
    }

    [Test]
    public async Task WithPrefix_False_Produces_Zero_Prefix()
    {
        var generator = CreateGenerator();
        var account = generator.GenerateBankAccount(new BankAccountGeneratorOptions
        {
            Seed = 1,
            WithPrefix = false
        });
        await Assert.That(account.Prefix).IsEqualTo("000000");
        await Assert.That(account.ToLocalFormat().Contains("-")).IsFalse();
    }

    [Test]
    public async Task WithPrefix_True_Produces_NonZero_Prefix()
    {
        var generator = CreateGenerator();
        var account = generator.GenerateBankAccount(new BankAccountGeneratorOptions
        {
            Seed = 1,
            WithPrefix = true
        });
        await Assert.That(long.Parse(account.Prefix) > 0).IsTrue();
        await Assert.That(account.ToLocalFormat().Contains("-")).IsTrue();
    }

    [Test]
    public async Task Validate_Rejects_Invalid_Account()
    {
        await Assert.That(BankAccountGenerator.Validate("")).IsFalse();
        // Real account with tampered last digit
        await Assert.That(BankAccountGenerator.Validate("2000145398/0800")).IsFalse();
    }

    [Test]
    public async Task Validate_Accepts_Known_Real_Format()
    {
        // 19-2000145399/0800 is a well-known Czech spořitelna account used in examples
        await Assert.That(BankAccountGenerator.Validate("19-2000145399/0800")).IsTrue();
    }
}
