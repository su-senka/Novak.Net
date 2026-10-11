using Novak.Net.Addresses.Models;
using Novak.Net.BankAccounts.Models;
using Novak.Net.Emails.Models;
using Novak.Net.Names.Models;
using Novak.Net.PhoneNumbers.Models;

namespace Novak.Net.People;

/// <summary>
/// A fictional Czech person whose fields agree with each other: the surname form matches the gender,
/// the rodné číslo encodes the birth date and gender, and the e-mail is derived from the name.
/// </summary>
public sealed record Person
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required Gender Gender { get; init; }
    public required DateOnly BirthDate { get; init; }
    public required RodneCislo.Models.RodneCislo RodneCislo { get; init; }
    public required Email Email { get; init; }
    public required PhoneNumber Phone { get; init; }
    public required Address Address { get; init; }
    public required BankAccount BankAccount { get; init; }

    /// <summary>"FirstName LastName".</summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>Age in whole years at <paramref name="on"/>.</summary>
    public int AgeOn(DateOnly on)
    {
        int age = on.Year - BirthDate.Year;
        return BirthDate > on.AddYears(-age) ? age - 1 : age;
    }

    public override string ToString() => FullName;
}
