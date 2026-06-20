using Novak.Net.RodneCislo;
using Novak.Net.RodneCislo.Models;
using Novak.Net.RodneCislo.Options;

namespace Novak.Net.RodneCislo.UnitTests;

public class RodneCisloGeneratorTests
{
    [Test]
    public async Task Generate_Is_Deterministic_For_Same_Seed()
    {
        var generator = new RodneCisloGenerator();
        var options = new RodneCisloGeneratorOptions { Seed = 42 };

        var first = generator.Generate(options).ToString();
        var second = generator.Generate(options).ToString();

        await Assert.That(first).IsEqualTo(second);
    }

    [Test]
    public async Task Generate_With_Slash_Format_Contains_Slash()
    {
        var generator = new RodneCisloGenerator();
        var rc = generator.Generate(new RodneCisloGeneratorOptions { Seed = 1, Format = RodneCisloFormat.WithSlash });
        await Assert.That(rc.Value.Contains("/")).IsTrue();
    }

    [Test]
    public async Task Generate_Without_Slash_Format_Has_No_Slash()
    {
        var generator = new RodneCisloGenerator();
        var rc = generator.Generate(new RodneCisloGeneratorOptions { Seed = 1, Format = RodneCisloFormat.WithoutSlash });
        await Assert.That(rc.Value.Contains("/")).IsFalse();
    }

    [Test]
    public async Task Generate_For_Year_1954_Plus_Is_Divisible_By_11()
    {
        var generator = new RodneCisloGenerator();
        for (int seed = 0; seed < 50; seed++)
        {
            var rc = generator.Generate(new RodneCisloGeneratorOptions
            {
                Seed = seed,
                MinYear = 1954,
                MaxYear = 2005,
                Format = RodneCisloFormat.WithoutSlash
            });
            long n = long.Parse(rc.Value);
            await Assert.That(n % 11).IsEqualTo(0L);
        }
    }

    [Test]
    public async Task Generate_Female_Has_Month_In_51_To_62_Range()
    {
        var generator = new RodneCisloGenerator();
        for (int seed = 0; seed < 50; seed++)
        {
            var rc = generator.Generate(new RodneCisloGeneratorOptions
            {
                Seed = seed,
                Gender = Gender.Female,
                Format = RodneCisloFormat.WithoutSlash
            });
            int mm = int.Parse(rc.Digits[2..4]);
            await Assert.That(mm >= 51 && mm <= 62).IsTrue();
        }
    }

    [Test]
    public async Task Generate_Male_Has_Month_In_01_To_12_Range()
    {
        var generator = new RodneCisloGenerator();
        for (int seed = 0; seed < 50; seed++)
        {
            var rc = generator.Generate(new RodneCisloGeneratorOptions
            {
                Seed = seed,
                Gender = Gender.Male,
                Format = RodneCisloFormat.WithoutSlash
            });
            int mm = int.Parse(rc.Digits[2..4]);
            await Assert.That(mm >= 1 && mm <= 12).IsTrue();
        }
    }

    [Test]
    public async Task Validate_Accepts_Generated_Numbers()
    {
        var generator = new RodneCisloGenerator();
        for (int seed = 0; seed < 50; seed++)
        {
            var rc = generator.Generate(new RodneCisloGeneratorOptions
            {
                Seed = seed,
                MinYear = 1954,
                MaxYear = 2005
            });
            await Assert.That(RodneCisloGenerator.Validate(rc.Value)).IsTrue();
        }
    }

    [Test]
    public async Task Validate_Rejects_Invalid_Input()
    {
        await Assert.That(RodneCisloGenerator.Validate("")).IsFalse();
        await Assert.That(RodneCisloGenerator.Validate("1234567")).IsFalse();  // 7 digits
        await Assert.That(RodneCisloGenerator.Validate("800615428X")).IsFalse(); // non-digit
        await Assert.That(RodneCisloGenerator.Validate("8006154281")).IsFalse(); // wrong check digit
    }
}
