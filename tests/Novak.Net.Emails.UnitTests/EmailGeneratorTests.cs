using Novak.Net.Emails.Options;

namespace Novak.Net.Emails.UnitTests;

public class EmailGeneratorTests
{
    private static readonly EmailGenerator Generator = new();

    [Test]
    public async Task GenerateEmail_Is_Deterministic_For_Same_Seed()
    {
        var options = new EmailGeneratorOptions { Seed = 42 };

        await Assert.That(Generator.GenerateEmail(options)).IsEqualTo(Generator.GenerateEmail(options));
    }

    [Test]
    public async Task Name_Based_Email_Uses_Name_Without_Diacritics()
    {
        for (int seed = 0; seed < 50; seed++)
        {
            var email = Generator.GenerateEmail(new EmailGeneratorOptions { FirstName = "Jiří", LastName = "Šťastná", Seed = seed });
            await Assert.That(email.LocalPart.Contains("stastna") || email.LocalPart.Contains("jiri")).IsTrue().Because(email.Value);
            await Assert.That(email.Value.All(c => c is '@' or '.' or '_' or >= 'a' and <= 'z' or >= '0' and <= '9')).IsTrue().Because(email.Value);
        }
    }

    [Test]
    public async Task Default_Uses_Czech_Providers()
    {
        var domains = Enumerable.Range(0, 300).Select(s => Generator.GenerateEmail(new EmailGeneratorOptions { Seed = s }).Domain).ToHashSet();

        await Assert.That(domains).Contains("seznam.cz");
        await Assert.That(domains).Contains("gmail.com");
    }

    [Test]
    public async Task SafeDomains_Only_Uses_Reserved_Domains()
    {
        for (int seed = 0; seed < 50; seed++)
        {
            var domain = Generator.GenerateEmail(new EmailGeneratorOptions { SafeDomains = true, Seed = seed }).Domain;
            await Assert.That(domain is "example.com" or "example.net" or "example.org").IsTrue();
        }
    }

    [Test]
    public async Task AllowedDomains_Override_And_MaxLength()
    {
        var email = Generator.GenerateEmail(new EmailGeneratorOptions
        {
            Seed = 5,
            AllowedDomains = ["Example.TEST"],
            MaxLocalPartLength = 4
        });

        await Assert.That(email.Domain).IsEqualTo("example.test");
        await Assert.That(email.LocalPart.Length).IsLessThanOrEqualTo(4);
    }

    [Test]
    public async Task Random_Style_Starts_With_Letter()
    {
        var email = Generator.GenerateEmail(new EmailGeneratorOptions { Style = EmailStyle.Random, Seed = 3 });

        await Assert.That(char.IsAsciiLetterLower(email.LocalPart[0])).IsTrue();
        await Assert.That(email.LocalPart.Length).IsBetween(8, 12);
    }
}
