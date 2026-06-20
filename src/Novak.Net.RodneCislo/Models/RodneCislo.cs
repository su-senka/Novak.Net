namespace Novak.Net.RodneCislo.Models;

public readonly record struct RodneCislo(string Value)
{
    public string Digits => Value.Replace("/", "");
    public override string ToString() => Value;
}