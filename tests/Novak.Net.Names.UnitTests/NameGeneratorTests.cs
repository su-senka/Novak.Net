using Novak.Net.Names;
using Novak.Net.Names.Models;
using Novak.Net.Names.Options;

namespace Novak.Net.Names.UnitTests;

public class NameGeneratorTests
{
    [Test]
    public async Task FullName_Is_Deterministic_For_Same_Seed()
    {
        var generator = new NameGenerator(new DataSource());
        var options = new NameGeneratorOptions { Gender = Gender.Female, Seed = 42 };

        var first = generator.GenerateFullName(options);
        var second = generator.GenerateFullName(options);

        await Assert.That(first).IsEqualTo(second);
    }

    [Test]
    public async Task GenerateFirstName_Respects_StartsWith_Filter()
    {
        var generator = new NameGenerator(new DataSource());

        var value = generator.GenerateFirstName(new NameGeneratorOptions { StartsWith = 'J', Seed = 7 });

        await Assert.That(value.StartsWith("J", StringComparison.OrdinalIgnoreCase)).IsTrue();
    }
}
