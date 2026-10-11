using System.Text;
using Novak.Net;
using Novak.Net.Addresses.Models;
using Novak.Net.Addresses.Options;
using Novak.Net.People;
using Novak.Net.PhoneNumbers.Options;

Console.OutputEncoding = Encoding.UTF8;

var seed = args.Length > 0 && int.TryParse(args[0], out var parsedSeed) ? parsedSeed : 42;

// One seeded client = the same people on every run.
var faker = new NovakClient(seed);
var today = new DateOnly(2026, 1, 1);

Console.WriteLine($"Novak.Net sample (seed {seed})");
Console.WriteLine();

for (int i = 0; i < 3; i++)
{
    var p = faker.People.GeneratePerson(new PersonGeneratorOptions { ReferenceDate = today });
    Console.WriteLine($"{p.FullName} ({p.Gender}, {p.AgeOn(today)} let)");
    Console.WriteLine($"  Rodné číslo : {p.RodneCislo}   narozen(a) {p.BirthDate:d. M. yyyy}");
    Console.WriteLine($"  E-mail      : {p.Email}");
    Console.WriteLine($"  Telefon     : {p.Phone.ToString(PhoneNumberFormat.International)}");
    Console.WriteLine($"  Adresa      : {p.Address}  ({p.Address.Region.ToCzechName()})");
    Console.WriteLine($"  Účet        : {p.BankAccount}  {p.BankAccount.BankName}");
    Console.WriteLine($"  IBAN        : {p.BankAccount.ToIban(spaced: true)}  BIC {p.BankAccount.Bic}");
    Console.WriteLine();
}

Console.WriteLine("Addresses in Brno:");
for (int i = 0; i < 3; i++)
    Console.WriteLine("  " + faker.Addresses.GenerateAddress(new AddressGeneratorOptions { City = "Brno" }));
