using Novak.Net.Names.Models;
using Novak.Net.People;
using RcGender = Novak.Net.RodneCislo.Models.Gender;

namespace Novak.Net.Facade.UnitTests;

public class NovakClientFacadeTests
{
    private static readonly DateOnly Today = new(2026, 10, 11);

    [Test]
    public async Task Creates_All_Modules()
    {
        INovakClient client = new NovakClient();

        await Assert.That(client.Names).IsNotNull();
        await Assert.That(client.Emails).IsNotNull();
        await Assert.That(client.PhoneNumbers).IsNotNull();
        await Assert.That(client.Addresses).IsNotNull();
        await Assert.That(client.RodneCislo).IsNotNull();
        await Assert.That(client.BankAccounts).IsNotNull();
        await Assert.That(client.People).IsNotNull();
    }

    [Test]
    public async Task Seeded_Client_Produces_Reproducible_But_Varied_Sequences()
    {
        var a = new NovakClient(seed: 7);
        var b = new NovakClient(seed: 7);

        var first = Enumerable.Range(0, 10).Select(_ => a.Names.GenerateFullName()).ToList();
        var second = Enumerable.Range(0, 10).Select(_ => b.Names.GenerateFullName()).ToList();

        await Assert.That(first).IsEquivalentTo(second);
        await Assert.That(first.Distinct().Count()).IsGreaterThan(5);
    }

    [Test]
    public async Task Person_Fields_Are_Consistent()
    {
        var client = new NovakClient(seed: 1);
        for (int i = 0; i < 200; i++)
        {
            var p = client.People.GeneratePerson(new PersonGeneratorOptions { ReferenceDate = Today });

            await Assert.That(p.RodneCislo.BirthDate).IsEqualTo(p.BirthDate);
            await Assert.That(p.RodneCislo.Gender).IsEqualTo(p.Gender == Gender.Female ? RcGender.Female : RcGender.Male);
            await Assert.That(p.AgeOn(Today)).IsBetween(18, 80);
            await Assert.That(Novak.Net.RodneCislo.RodneCisloGenerator.Validate(p.RodneCislo.Value)).IsTrue();
            await Assert.That(Novak.Net.BankAccounts.BankAccountGenerator.Validate(p.BankAccount.ToString())).IsTrue();
            if (p.Gender == Gender.Female)
                await Assert.That(p.LastName.EndsWith('á') || p.LastName.EndsWith('í') || p.LastName.EndsWith('ů')).IsTrue().Because(p.LastName);
        }
    }

    [Test]
    public async Task Person_Email_Is_Derived_From_Name()
    {
        var p = new NovakClient(seed: 3).People.GeneratePerson(new PersonGeneratorOptions { ReferenceDate = Today, SafeEmailDomains = true });
        var plainLast = Novak.Net.Common.Text.CzechText.RemoveDiacritics(p.LastName).ToLowerInvariant();
        var plainFirst = Novak.Net.Common.Text.CzechText.RemoveDiacritics(p.FirstName).ToLowerInvariant();

        await Assert.That(p.Email.LocalPart.Contains(plainLast) || p.Email.LocalPart.Contains(plainFirst)).IsTrue().Because(p.Email.Value);
        await Assert.That(p.Email.Domain).StartsWith("example.");
    }

    [Test]
    public async Task Person_Options_Are_Applied()
    {
        var people = new NovakClient(seed: 5).People;
        for (int i = 0; i < 50; i++)
        {
            var p = people.GeneratePerson(new PersonGeneratorOptions
            {
                Gender = Gender.Male, MinAge = 30, MaxAge = 30, City = "Ostrava", ReferenceDate = Today
            });
            await Assert.That(p.Gender).IsEqualTo(Gender.Male);
            await Assert.That(p.AgeOn(Today)).IsEqualTo(30);
            await Assert.That(p.Address.Municipality).IsEqualTo("Ostrava");
        }
    }

    [Test]
    public async Task Seeded_Person_Is_Stable()
    {
        var options = new PersonGeneratorOptions { Seed = 42, ReferenceDate = Today };

        await Assert.That(new NovakClient().People.GeneratePerson(options))
            .IsEqualTo(new NovakClient().People.GeneratePerson(options));
    }
}
