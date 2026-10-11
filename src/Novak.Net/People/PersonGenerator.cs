using Novak.Net.Addresses;
using Novak.Net.Addresses.Options;
using Novak.Net.BankAccounts;
using Novak.Net.BankAccounts.Options;
using Novak.Net.Common.Randomization;
using Novak.Net.Emails;
using Novak.Net.Emails.Options;
using Novak.Net.Names;
using Novak.Net.Names.Models;
using Novak.Net.Names.Options;
using Novak.Net.PhoneNumbers;
using Novak.Net.PhoneNumbers.Options;
using Novak.Net.RodneCislo;
using Novak.Net.RodneCislo.Options;
using RcGender = Novak.Net.RodneCislo.Models.Gender;

namespace Novak.Net.People;

/// <summary>Default <see cref="IPersonGenerator"/> built on top of the individual generators.</summary>
public sealed class PersonGenerator : IPersonGenerator
{
    private readonly INameGenerator _names;
    private readonly IEmailGenerator _emails;
    private readonly IPhoneNumberGenerator _phones;
    private readonly IAddressGenerator _addresses;
    private readonly IRodneCisloGenerator _rodneCisla;
    private readonly IBankAccountGenerator _bankAccounts;
    private readonly IRandom _random;

    public PersonGenerator(
        INameGenerator names,
        IEmailGenerator emails,
        IPhoneNumberGenerator phones,
        IAddressGenerator addresses,
        IRodneCisloGenerator rodneCisla,
        IBankAccountGenerator bankAccounts,
        IRandom? random = null)
    {
        _names = names ?? throw new ArgumentNullException(nameof(names));
        _emails = emails ?? throw new ArgumentNullException(nameof(emails));
        _phones = phones ?? throw new ArgumentNullException(nameof(phones));
        _addresses = addresses ?? throw new ArgumentNullException(nameof(addresses));
        _rodneCisla = rodneCisla ?? throw new ArgumentNullException(nameof(rodneCisla));
        _bankAccounts = bankAccounts ?? throw new ArgumentNullException(nameof(bankAccounts));
        _random = random ?? DefaultRandom.Shared;
    }

    public Person GeneratePerson(PersonGeneratorOptions? options = null)
    {
        options ??= new PersonGeneratorOptions();
        if (options.MinAge < 0 || options.MinAge > options.MaxAge || options.MaxAge > 120)
            throw new ArgumentException("Ages must satisfy 0 <= MinAge <= MaxAge <= 120.", nameof(options));

        var rng = _random.For(options);
        // Each part gets its own seed drawn from one sequence: reproducible, and independent of the parts' internals.
        int NextSeed() => rng.Next(int.MaxValue);

        var name = _names.GenerateName(new NameGeneratorOptions { Gender = options.Gender, Seed = NextSeed() });
        var birthDate = RandomBirthDate(options, rng);

        var rodneCislo = _rodneCisla.Generate(new RodneCisloGeneratorOptions
        {
            Seed = NextSeed(),
            BirthDate = birthDate,
            Gender = name.Gender == Gender.Female ? RcGender.Female : RcGender.Male
        });

        var email = _emails.GenerateEmail(new EmailGeneratorOptions
        {
            Seed = NextSeed(),
            FirstName = name.FirstName,
            LastName = name.LastName,
            SafeDomains = options.SafeEmailDomains
        });

        var phone = _phones.GeneratePhoneNumber(new PhoneNumberGeneratorOptions { Seed = NextSeed() });
        var address = _addresses.GenerateAddress(new AddressGeneratorOptions
        {
            Seed = NextSeed(),
            Region = options.Region,
            City = options.City
        });
        var bankAccount = _bankAccounts.GenerateBankAccount(new BankAccountGeneratorOptions { Seed = NextSeed() });

        return new Person
        {
            FirstName = name.FirstName,
            LastName = name.LastName,
            Gender = name.Gender,
            BirthDate = birthDate,
            RodneCislo = rodneCislo,
            Email = email,
            Phone = phone,
            Address = address,
            BankAccount = bankAccount
        };
    }

    private static DateOnly RandomBirthDate(PersonGeneratorOptions options, IRandom rng)
    {
        var today = options.ReferenceDate ?? DateOnly.FromDateTime(DateTime.Today);
        var latest = today.AddYears(-options.MinAge);                     // turned MinAge today
        var earliest = today.AddYears(-options.MaxAge - 1).AddDays(1);    // turns MaxAge + 1 tomorrow
        return earliest.AddDays(rng.Next(latest.DayNumber - earliest.DayNumber + 1));
    }
}
