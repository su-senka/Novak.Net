using Novak.Net.Emails;
using Novak.Net.Emails.Data;
using Novak.Net.Emails.Options;

namespace Novak.Net.Emails.UnitTests;

public class EmailGeneratorTests
{
    [Test]
    public async Task GenerateEmail_Is_Deterministic_For_Same_Seed()
    {
        var generator = new EmailGenerator(new EmbeddedEmailDataRepository());
        var options = new EmailGeneratorOptions { Seed = 42 };

        var first = generator.GenerateEmail(options).ToString();
        var second = generator.GenerateEmail(options).ToString();

        await Assert.That(first).IsEqualTo(second);
    }

    [Test]
    public async Task GenerateEmail_Uses_AllowedDomains_When_Provided()
    {
        var generator = new EmailGenerator(new EmbeddedEmailDataRepository());

        var email = generator.GenerateEmail(new EmailGeneratorOptions
        {
            Seed = 5,
            AllowedDomains = new[] { "example.test" }
        }).ToString();

        await Assert.That(email.EndsWith("@example.test", StringComparison.OrdinalIgnoreCase)).IsTrue();
    }
}
