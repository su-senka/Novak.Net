using Novak.Net.PhoneNumbers;
using Novak.Net.PhoneNumbers.Data;
using Novak.Net.PhoneNumbers.Options;

namespace Novak.Net.PhoneNumbers.UnitTests;

public class PhoneNumberGeneratorTests
{
    [Test]
    public async Task GeneratePhoneNumber_Is_Deterministic_For_Same_Seed()
    {
        var generator = new PhoneNumberGenerator(new EmbeddedPhoneNumberDataRepository());
        var options = new PhoneNumberGeneratorOptions { Seed = 42 };

        var first = generator.GeneratePhoneNumber(options).ToString();
        var second = generator.GeneratePhoneNumber(options).ToString();

        await Assert.That(first).IsEqualTo(second);
    }

    [Test]
    public async Task Spaced_Format_Contains_Separators()
    {
        var generator = new PhoneNumberGenerator(new EmbeddedPhoneNumberDataRepository());

        var phone = generator.GeneratePhoneNumber(new PhoneNumberGeneratorOptions
        {
            Seed = 5,
            Format = PhoneNumberFormat.Spaced
        }).ToString();

        await Assert.That(phone.Contains(" ")).IsTrue();
        await Assert.That(phone.StartsWith("+")).IsTrue();
    }
}
