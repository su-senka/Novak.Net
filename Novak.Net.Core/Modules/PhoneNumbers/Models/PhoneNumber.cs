namespace Novak.Net.Core.Modules.PhoneNumbers.Models;

public readonly record struct PhoneNumber(string Value)
{
    public override string ToString() => Value;
}
