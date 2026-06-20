using Novak.Net;
using Novak.Net.BankAccounts.Options;
using Novak.Net.Emails.Options;
using Novak.Net.Names.Options;
using Novak.Net.PhoneNumbers.Options;
using Novak.Net.RodneCislo.Options;
using NamesGender = Novak.Net.Names.Models.Gender;
using RcGender = Novak.Net.RodneCislo.Models.Gender;

var seed = args.Length > 0 && int.TryParse(args[0], out var parsedSeed) ? parsedSeed : 42;

INovakClient client = new NovakClient();

var name = client.Names.GenerateFullName(new NameGeneratorOptions
{
    Gender = NamesGender.Female,
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

var rodneCislo = client.RodneCislo.Generate(new RodneCisloGeneratorOptions
{
    Seed = seed,
    Gender = RcGender.Female
});

var bankAccount = client.BankAccounts.GenerateBankAccount(new BankAccountGeneratorOptions
{
    Seed = seed,
    WithPrefix = false
});

Console.WriteLine("Novak.Net sample output");
Console.WriteLine($"Seed        : {seed}");
Console.WriteLine($"Name        : {name}");
Console.WriteLine($"Email       : {email}");
Console.WriteLine($"Phone       : {phone}");
Console.WriteLine($"Rodné číslo : {rodneCislo}");
Console.WriteLine($"Bank account: {bankAccount}  (IBAN: {bankAccount.ToIban()})");
