using System.Text.RegularExpressions;
using Novak.Net.PhoneNumbers.Models;
using Novak.Net.PhoneNumbers.Options;

namespace Novak.Net.PhoneNumbers.UnitTests;

public class PhoneNumberGeneratorTests
{
    private static readonly PhoneNumberGenerator Generator = new();

    // Czech mobile ranges per ČTÚ numbering plan.
    private static readonly Regex Mobile = new(@"^(60[1-8]|7[23]\d|77\d|79\d)\d{6}$");

    [Test]
    public async Task GeneratePhoneNumber_Is_Deterministic_For_Same_Seed()
    {
        var options = new PhoneNumberGeneratorOptions { Seed = 42 };

        await Assert.That(Generator.GeneratePhoneNumber(options)).IsEqualTo(Generator.GeneratePhoneNumber(options));
    }

    [Test]
    public async Task Mobile_Numbers_Are_In_Real_Mobile_Ranges()
    {
        var prefixes = new HashSet<string>();
        for (int seed = 0; seed < 300; seed++)
        {
            var phone = Generator.GeneratePhoneNumber(new PhoneNumberGeneratorOptions { Seed = seed });
            await Assert.That(Mobile.IsMatch(phone.NationalNumber)).IsTrue().Because(phone.NationalNumber);
            prefixes.Add(phone.NationalNumber[..3]);
        }
        await Assert.That(prefixes.Count).IsGreaterThan(20);
    }

    [Test]
    public async Task Landline_Numbers_Are_Geographic()
    {
        for (int seed = 0; seed < 100; seed++)
        {
            var phone = Generator.GeneratePhoneNumber(new PhoneNumberGeneratorOptions { Type = PhoneNumberType.Landline, Seed = seed });
            await Assert.That(phone.Type).IsEqualTo(PhoneNumberType.Landline);
            await Assert.That(phone.NationalNumber[0] is >= '2' and <= '5').IsTrue().Because(phone.NationalNumber);
            await Assert.That(phone.NationalNumber.Length).IsEqualTo(9);
        }
    }

    [Test]
    public async Task Formats()
    {
        var phone = new PhoneNumber("603123456", PhoneNumberType.Mobile);

        await Assert.That(phone.ToString(PhoneNumberFormat.E164)).IsEqualTo("+420603123456");
        await Assert.That(phone.ToString(PhoneNumberFormat.International)).IsEqualTo("+420 603 123 456");
        await Assert.That(phone.ToString(PhoneNumberFormat.National)).IsEqualTo("603 123 456");
        await Assert.That(phone.ToString(PhoneNumberFormat.Compact)).IsEqualTo("603123456");
        await Assert.That(phone.Value).IsEqualTo("+420603123456");
    }

    [Test]
    public async Task Format_Option_Is_Applied()
    {
        var phone = Generator.GeneratePhoneNumber(new PhoneNumberGeneratorOptions { Seed = 5, Format = PhoneNumberFormat.International });

        await Assert.That(Regex.IsMatch(phone.Value, @"^\+420 \d{3} \d{3} \d{3}$")).IsTrue();
    }
}
