namespace Novak.Net.Names.Models;

/// <summary>A gender-consistent Czech first name and surname, e.g. "Jana Nováková".</summary>
public sealed record PersonName(string FirstName, string LastName, Gender Gender)
{
    /// <summary>"FirstName LastName".</summary>
    public string FullName => $"{FirstName} {LastName}";

    public override string ToString() => FullName;
}
