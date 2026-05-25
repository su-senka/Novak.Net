namespace Novak.Net.Emails.Models;

public readonly record struct Email(string Value)
{
    public override string ToString() => Value;
}
