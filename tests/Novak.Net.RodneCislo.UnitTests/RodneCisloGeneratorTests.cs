using Novak.Net.RodneCislo.Models;
using Novak.Net.RodneCislo.Options;

namespace Novak.Net.RodneCislo.UnitTests;

public class RodneCisloGeneratorTests
{
    private static readonly RodneCisloGenerator Generator = new();

    [Test]
    public async Task Generate_Is_Deterministic_For_Same_Seed()
    {
        var options = new RodneCisloGeneratorOptions { Seed = 42 };

        await Assert.That(Generator.Generate(options)).IsEqualTo(Generator.Generate(options));
    }

    [Test]
    public async Task Formats()
    {
        var withSlash = Generator.Generate(new RodneCisloGeneratorOptions { Seed = 1, Format = RodneCisloFormat.WithSlash });
        var without = Generator.Generate(new RodneCisloGeneratorOptions { Seed = 1, Format = RodneCisloFormat.WithoutSlash });

        await Assert.That(withSlash.Value[6]).IsEqualTo('/');
        await Assert.That(without.Value.Contains('/')).IsFalse();
        await Assert.That(withSlash.Digits).IsEqualTo(without.Value);
    }

    [Test]
    public async Task Generated_Numbers_Are_Valid_And_Divisible_By_11_From_1954()
    {
        for (int seed = 0; seed < 500; seed++)
        {
            var rc = Generator.Generate(new RodneCisloGeneratorOptions { Seed = seed, MinYear = 1900, MaxYear = 2050 });
            await Assert.That(RodneCisloGenerator.Validate(rc.Value)).IsTrue().Because(rc.Value);
            if (rc.BirthDate.Year >= 1954)
            {
                await Assert.That(rc.Digits.Length).IsEqualTo(10);
                await Assert.That(long.Parse(rc.Digits) % 11).IsEqualTo(0L);
            }
            else
            {
                await Assert.That(rc.Digits.Length).IsEqualTo(9);
            }
        }
    }

    [Test]
    public async Task Encodes_Requested_Birth_Date_And_Gender()
    {
        var date = new DateOnly(1985, 2, 28);
        var female = Generator.Generate(new RodneCisloGeneratorOptions { BirthDate = date, Gender = Gender.Female, Seed = 3 });
        var male = Generator.Generate(new RodneCisloGeneratorOptions { BirthDate = date, Gender = Gender.Male, Seed = 3 });

        await Assert.That(female.Value).StartsWith("855228/");
        await Assert.That(female.BirthDate).IsEqualTo(date);
        await Assert.That(female.Gender).IsEqualTo(Gender.Female);
        await Assert.That(male.Value).StartsWith("850228/");
        await Assert.That(male.Gender).IsEqualTo(Gender.Male);
    }

    [Test]
    public async Task Year_Range_Is_Respected()
    {
        for (int seed = 0; seed < 100; seed++)
        {
            var rc = Generator.Generate(new RodneCisloGeneratorOptions { Seed = seed, MinYear = 1990, MaxYear = 1991 });
            await Assert.That(rc.BirthDate.Year).IsBetween(1990, 1991);
        }
    }

    [Test]
    [Arguments("736028/5163")]   // standard female number
    [Arguments("7360285163")]    // without slash
    [Arguments("530101/123")]    // 9 digits, born before 1954
    [Arguments(" 736028 5163 ")] // stray spaces
    public async Task Validate_Accepts_Real_World_Shapes(string value)
    {
        await Assert.That(RodneCisloGenerator.Validate(value)).IsTrue();
    }

    [Test]
    public async Task Leap_Day_Is_Valid()
    {
        var rc = Generator.Generate(new RodneCisloGeneratorOptions { BirthDate = new DateOnly(2004, 2, 29), Seed = 1 });

        await Assert.That(rc.BirthDate).IsEqualTo(new DateOnly(2004, 2, 29));
    }

    [Test]
    public async Task Validate_Accepts_Pre_1986_Remainder_10_Exception()
    {
        // Find a serial whose first nine digits give remainder 10, then use check digit 0.
        var value = Enumerable.Range(0, 1000).Select(s => $"780123{s:D3}").First(n => long.Parse(n) % 11 == 10) + "0";
        await Assert.That(RodneCislo.Models.RodneCislo.IsValid(value)).IsTrue();

        var after1985 = Enumerable.Range(0, 1000).Select(s => $"900123{s:D3}").First(n => long.Parse(n) % 11 == 10) + "0";
        await Assert.That(RodneCislo.Models.RodneCislo.IsValid(after1985)).IsFalse();
    }

    [Test]
    public async Task Validate_Accepts_Plus_20_Month_Only_From_2004()
    {
        await Assert.That(RodneCisloGenerator.Validate(WithCheck("052115123"))).IsTrue();   // man, Jan 15 2005 (+20)
        await Assert.That(RodneCisloGenerator.Validate(WithCheck("057115124"))).IsTrue();   // woman, Jan 15 2005 (+70)
        await Assert.That(RodneCisloGenerator.Validate(WithCheck("852115123"))).IsFalse();  // 1985 cannot use +20
    }

    [Test]
    public async Task Validate_Rejects_Invalid_Input()
    {
        await Assert.That(RodneCisloGenerator.Validate("")).IsFalse();
        await Assert.That(RodneCisloGenerator.Validate(null)).IsFalse();
        await Assert.That(RodneCisloGenerator.Validate("1234567")).IsFalse();             // 7 digits
        await Assert.That(RodneCisloGenerator.Validate("736028516X")).IsFalse();          // non-digit
        await Assert.That(RodneCisloGenerator.Validate("736028/5164")).IsFalse();         // wrong check digit
        await Assert.That(RodneCisloGenerator.Validate(WithCheck("850230123"))).IsFalse(); // 30 February
        await Assert.That(RodneCisloGenerator.Validate("600101/123")).IsFalse();          // 9 digits after 1953
    }

    [Test]
    public async Task Parse_Exposes_Birth_Date_And_Gender()
    {
        var rc = RodneCislo.Models.RodneCislo.Parse("736028/5163");

        await Assert.That(rc.BirthDate).IsEqualTo(new DateOnly(1973, 10, 28));
        await Assert.That(rc.Gender).IsEqualTo(Gender.Female);
        await Assert.That(() => RodneCislo.Models.RodneCislo.Parse("nope")).Throws<FormatException>();
    }

    private static string WithCheck(string first9) => first9 + (long.Parse(first9) % 11 % 10);
}
