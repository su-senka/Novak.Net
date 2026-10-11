using Novak.Net.Addresses;
using Novak.Net.BankAccounts;
using Novak.Net.Common.Randomization;
using Novak.Net.Emails;
using Novak.Net.Names;
using Novak.Net.People;
using Novak.Net.PhoneNumbers;
using Novak.Net.RodneCislo;

namespace Novak.Net;

/// <summary>
/// Creates and wires all generators with the built-in Czech data. Thread-safe.
/// </summary>
/// <example>
/// <code>
/// var faker = new NovakClient(seed: 42);          // same sequence of values on every run
/// var person = faker.People.GeneratePerson();
/// var iban = faker.BankAccounts.GenerateBankAccount().ToIban();
/// </code>
/// </example>
public class NovakClient : INovakClient
{
    /// <summary>Creates a client with an unseeded random source.</summary>
    public NovakClient() : this((IRandom?)null)
    {
    }

    /// <summary>
    /// Creates a client whose generators share one random sequence. With a seed, the sequence of generated
    /// values is reproducible (a per-call <c>Seed</c> option still overrides it for that call).
    /// </summary>
    public NovakClient(int? seed) : this(seed.HasValue ? DefaultRandom.Create(seed) : null)
    {
    }

    /// <summary>Creates a client that draws from a custom random source.</summary>
    public NovakClient(IRandom? random)
    {
        var rng = random ?? DefaultRandom.Shared;
        Names = new NameGenerator(random: rng);
        Emails = new EmailGenerator(names: Names, random: rng);
        PhoneNumbers = new PhoneNumberGenerator(random: rng);
        Addresses = new AddressGenerator(random: rng);
        RodneCislo = new RodneCisloGenerator(rng);
        BankAccounts = new BankAccountGenerator(random: rng);
        People = new PersonGenerator(Names, Emails, PhoneNumbers, Addresses, RodneCislo, BankAccounts, rng);
    }

    public INameGenerator Names { get; }
    public IEmailGenerator Emails { get; }
    public IPhoneNumberGenerator PhoneNumbers { get; }
    public IAddressGenerator Addresses { get; }
    public IRodneCisloGenerator RodneCislo { get; }
    public IBankAccountGenerator BankAccounts { get; }
    public IPersonGenerator People { get; }
}
