using Novak.Net.Common.Options;
using Novak.Net.Names.Models;
using Novak.Net.Names.Options;

namespace Novak.Net.Names.UnitTests;

public class NameGeneratorTests
{
    private static readonly NameGenerator Generator = new();

    [Test]
    public async Task FullName_Is_Deterministic_For_Same_Seed()
    {
        var options = new NameGeneratorOptions { Gender = Gender.Female, Seed = 42 };

        await Assert.That(Generator.GenerateFullName(options)).IsEqualTo(Generator.GenerateFullName(options));
    }

    [Test]
    public async Task Female_Names_Use_Feminine_Surnames()
    {
        for (int seed = 0; seed < 200; seed++)
        {
            var name = Generator.GenerateName(new NameGeneratorOptions { Gender = Gender.Female, Seed = seed });
            await Assert.That(name.Gender).IsEqualTo(Gender.Female);
            await Assert.That(name.LastName.EndsWith('á') || name.LastName.EndsWith('í') || name.LastName.EndsWith('ů'))
                .IsTrue().Because($"'{name.LastName}' is not a feminine surname");
        }
    }

    [Test]
    public async Task Any_Gender_Full_Name_Is_Consistent()
    {
        var data = new DataSource();
        var female = data.GetFirstNames(new NameGeneratorOptions { Gender = Gender.Female }).Select(r => r.Text).ToHashSet();
        var masculineSurnames = data.GetSurnames(new NameGeneratorOptions()).Select(r => r.Text).ToHashSet();

        for (int seed = 0; seed < 300; seed++)
        {
            var name = Generator.GenerateName(new NameGeneratorOptions { Seed = seed });
            bool isFemale = female.Contains(name.FirstName);
            await Assert.That(name.Gender).IsEqualTo(isFemale ? Gender.Female : Gender.Male);
            if (!isFemale)
                await Assert.That(masculineSurnames).Contains(name.LastName);
        }
    }

    [Test]
    [Arguments("Novák", "Nováková")]
    [Arguments("Černý", "Černá")]
    [Arguments("Svoboda", "Svobodová")]
    [Arguments("Šimek", "Šimková")]
    [Arguments("Vaněk", "Vaňková")]
    [Arguments("Němec", "Němcová")]
    [Arguments("Havel", "Havlová")]
    [Arguments("Krejčí", "Krejčí")]
    [Arguments("Janů", "Janů")]
    [Arguments("Beneš", "Benešová")]
    [Arguments("Nováková", "Nováková")]
    public async Task Feminize_Follows_Czech_Rules(string masculine, string feminine)
    {
        await Assert.That(CzechSurnames.Feminize(masculine)).IsEqualTo(feminine);
    }

    [Test]
    public async Task Embedded_Feminine_Forms_Match_Rules_Or_Are_Known_Exceptions()
    {
        // Irregular forms the rule-based fallback cannot derive.
        var exceptions = new HashSet<string> { "Kadlec" };
        foreach (var r in new DataSource().GetSurnames(new NameGeneratorOptions()))
        {
            if (exceptions.Contains(r.Text)) continue;
            await Assert.That(r.FeminineForm).IsEqualTo(CzechSurnames.Feminize(r.Text)).Because(r.Text);
        }
    }

    [Test]
    public async Task GenerateFirstName_Respects_StartsWith_Filter()
    {
        for (int seed = 0; seed < 20; seed++)
        {
            var value = Generator.GenerateFirstName(new NameGeneratorOptions { StartsWith = 'j', Seed = seed });
            await Assert.That(value.StartsWith('J')).IsTrue();
        }
    }

    [Test]
    public async Task Length_Include_And_Exclude_Filters_Work()
    {
        var shortName = Generator.GenerateFirstName(new NameGeneratorOptions { MaxLength = 3, Seed = 1 });
        await Assert.That(shortName.Length).IsLessThanOrEqualTo(3);

        var only = Generator.GenerateFirstName(new NameGeneratorOptions { Include = ["tomáš"], Seed = 1 });
        await Assert.That(only).IsEqualTo("Tomáš");

        for (int seed = 0; seed < 50; seed++)
        {
            var name = Generator.GenerateFirstName(new NameGeneratorOptions { Gender = Gender.Male, Exclude = ["Jiří", "Jan"], Seed = seed });
            await Assert.That(name is "Jiří" or "Jan").IsFalse();
        }
    }

    [Test]
    public async Task Without_Diacritics_And_Upper_Casing()
    {
        for (int seed = 0; seed < 50; seed++)
        {
            var name = Generator.GenerateFullName(new NameGeneratorOptions { WithDiacritics = false, Casing = Casing.Upper, Seed = seed });
            await Assert.That(name.All(c => c is ' ' or >= 'A' and <= 'Z')).IsTrue().Because(name);
        }
    }

    [Test]
    public async Task Frequency_Buckets_Are_Disjoint()
    {
        var common = Enumerable.Range(0, 100).Select(s => Generator.GenerateFirstName(new NameGeneratorOptions { Gender = Gender.Male, FrequencyBucket = NameFrequency.Common, Seed = s })).ToHashSet();
        var rare = Enumerable.Range(0, 100).Select(s => Generator.GenerateFirstName(new NameGeneratorOptions { Gender = Gender.Male, FrequencyBucket = NameFrequency.Rare, Seed = s })).ToHashSet();

        await Assert.That(common.Overlaps(rare)).IsFalse();
        await Assert.That(common).Contains("Jiří");
    }

    [Test]
    public async Task GenerateMany_With_Seed_Is_Reproducible_And_Varied()
    {
        var a = Generator.GenerateMany(() => new NameGeneratorOptions { Seed = 7 }, 20).ToList();
        var b = Generator.GenerateMany(() => new NameGeneratorOptions { Seed = 7 }, 20).ToList();

        await Assert.That(a).IsEquivalentTo(b);
        await Assert.That(a.Distinct().Count()).IsGreaterThan(10);
    }

    [Test]
    public async Task Impossible_Filters_Throw()
    {
        await Assert.That(() => Generator.GenerateFirstName(new NameGeneratorOptions { StartsWith = 'Q' }))
            .Throws<InvalidOperationException>();
        await Assert.That(() => Generator.GenerateFirstName(new NameGeneratorOptions { MinLength = 5, MaxLength = 2 }))
            .Throws<ArgumentException>();
    }
}
