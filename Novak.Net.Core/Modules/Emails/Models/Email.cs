namespace Novak.Net.Core.Modules.Emails.Models;

public readonly record struct Email(string Value)
{
    public override string ToString() => Value;
}
