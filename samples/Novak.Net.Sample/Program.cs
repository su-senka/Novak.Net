using Novak.Net;
using Novak.Net.Emails.Options;
using Novak.Net.Names.Models;
using Novak.Net.Names.Options;
using Novak.Net.PhoneNumbers.Options;

var seed = args.Length > 0 && int.TryParse(args[0], out var parsedSeed) ? parsedSeed : 42;

INovakClient client = new NovakClient();

var name = client.Names.GenerateFullName(new NameGeneratorOptions
{
    Gender = Gender.Female,
    Seed = seed
});

var email = client.Emails.GenerateEmail(new EmailGeneratorOptions
{
    Seed = seed,
    AllowedDomains = new[] { "example.cz" }
});

var phone = client.PhoneNumbers.GeneratePhoneNumber(new PhoneNumberGeneratorOptions
{
    Seed = seed,
    Format = PhoneNumberFormat.Spaced
});

Console.WriteLine("Novak.Net sample output");
Console.WriteLine($"Seed : {seed}");
Console.WriteLine($"Name : {name}");
Console.WriteLine($"Email: {email}");
Console.WriteLine($"Phone: {phone}");

