using Novak.Net.Addresses.Data;
using Novak.Net.Addresses.Models;
using Novak.Net.Addresses.Options;

namespace Novak.Net.Addresses.UnitTests;

public class AddressGeneratorTests
{
    private static readonly AddressGenerator Generator = new();

    [Test]
    public async Task Generate_Is_Deterministic_For_Same_Seed()
    {
        var options = new AddressGeneratorOptions { Seed = 42 };

        await Assert.That(Generator.GenerateAddress(options)).IsEqualTo(Generator.GenerateAddress(options));
    }

    [Test]
    public async Task Embedded_Data_Is_Complete_And_Well_Formed()
    {
        var all = new EmbeddedAddressDataRepository().GetAddresses();

        await Assert.That(all.Count).IsEqualTo(20_000);
        await Assert.That(all.Select(a => a.Region).Distinct().Count()).IsEqualTo(14);
        foreach (var a in all.Take(2000))
        {
            await Assert.That(a.PostalCode.Length == 5 && a.PostalCode.All(char.IsAsciiDigit)).IsTrue().Because(a.ToString());
            await Assert.That(a.HouseNumber.All(char.IsAsciiDigit)).IsTrue().Because(a.ToString());
            await Assert.That(Enum.IsDefined(a.Region)).IsTrue();
        }
    }

    [Test]
    public async Task Prague_Share_Reflects_Population()
    {
        var all = new EmbeddedAddressDataRepository().GetAddresses();
        double prague = all.Count(a => a.Region == CzechRegion.Praha) / (double)all.Count;

        await Assert.That(prague).IsBetween(0.10, 0.16);
    }

    [Test]
    public async Task Region_And_City_Filters()
    {
        for (int seed = 0; seed < 30; seed++)
        {
            var moravia = Generator.GenerateAddress(new AddressGeneratorOptions { Region = CzechRegion.Jihomoravsky, Seed = seed });
            await Assert.That(moravia.Region).IsEqualTo(CzechRegion.Jihomoravsky);

            var brno = Generator.GenerateAddress(new AddressGeneratorOptions { City = "brno", Seed = seed });
            await Assert.That(brno.Municipality).IsEqualTo("Brno");
            await Assert.That(brno.PostalCode).StartsWith("6");

            var praha5 = Generator.GenerateAddress(new AddressGeneratorOptions { City = "Praha 5", Seed = seed });
            await Assert.That(praha5.PostTown).IsEqualTo("Praha 5");
            await Assert.That(praha5.Region).IsEqualTo(CzechRegion.Praha);
        }
    }

    [Test]
    public async Task RequireStreet_Filters_Village_Addresses()
    {
        for (int seed = 0; seed < 50; seed++)
        {
            var a = Generator.GenerateAddress(new AddressGeneratorOptions { RequireStreet = true, Region = CzechRegion.Vysocina, Seed = seed });
            await Assert.That(a.Street).IsNotNull();
        }
    }

    [Test]
    public async Task Unknown_City_Throws()
    {
        await Assert.That(() => Generator.GenerateAddress(new AddressGeneratorOptions { City = "Atlantis" }))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task Formatting_Follows_Czech_Post_Conventions()
    {
        var prague = new Address
        {
            Street = "Vodičkova", HouseNumber = "681", OrientationNumber = "14", Municipality = "Praha",
            CityPart = "Nové Město", District = "Praha 1", PostTown = "Praha 1", PostalCode = "11000", Region = CzechRegion.Praha
        };
        var village = new Address
        {
            HouseNumber = "25", Municipality = "Chudobín", CityPart = "Lhota", PostTown = "Chudobín",
            PostalCode = "78321", Region = CzechRegion.Olomoucky
        };

        await Assert.That(prague.ToString()).IsEqualTo("Vodičkova 681/14, 110 00 Praha 1");
        await Assert.That(prague.ToMultiline()).IsEqualTo("Vodičkova 681/14\n110 00 Praha 1");
        await Assert.That(village.ToString()).IsEqualTo("Lhota 25, 783 21 Chudobín");
        await Assert.That(CzechRegion.Vysocina.ToCzechName()).IsEqualTo("Kraj Vysočina");
    }
}
