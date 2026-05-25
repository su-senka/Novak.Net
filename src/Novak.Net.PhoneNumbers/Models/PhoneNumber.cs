namespace Novak.Net.PhoneNumbers.Models;

public readonly record struct PhoneNumber(string Value)
{
    public override string ToString() => Value;
}
