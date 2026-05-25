using Novak.Net;

namespace Novak.Net.Facade.UnitTests;

public class NovakClientFacadeTests
{
    [Test]
    public async Task Creates_All_Implemented_Modules()
    {
        var client = new NovakClient();

        await Assert.That(client.Names).IsNotNull();
        await Assert.That(client.Emails).IsNotNull();
        await Assert.That(client.PhoneNumbers).IsNotNull();
    }

    [Test]
    public async Task Compatibility_Type_Still_Constructs()
    {
        var legacyClient = new Novak.Net.Core.NovakNetClient();

        await Assert.That(legacyClient).IsNotNull();
    }
}
