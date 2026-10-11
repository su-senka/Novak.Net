namespace Novak.Net.Emails.Models;

/// <summary>An e-mail address.</summary>
public readonly record struct Email(string Value)
{
    /// <summary>The part before '@'.</summary>
    public string LocalPart => Value[..Value.IndexOf('@')];

    /// <summary>The part after '@'.</summary>
    public string Domain => Value[(Value.IndexOf('@') + 1)..];

    public override string ToString() => Value;
}
