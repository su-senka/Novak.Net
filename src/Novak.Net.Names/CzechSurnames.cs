namespace Novak.Net.Names;

/// <summary>Czech surname helpers.</summary>
public static class CzechSurnames
{
    /// <summary>
    /// Derives the feminine form of a Czech masculine surname using the regular rules:
    /// Černý → Černá, Krejčí → Krejčí, Janů → Janů, Svoboda → Svobodová, Šimek → Šimková,
    /// Vaněk → Vaňková, Němec → Němcová, Havel → Havlová, Novák → Nováková.
    /// Irregular cases (e.g. Kadlec → Kadlecová) cannot be derived and should come from data.
    /// Surnames that already look feminine (-ová, -á) are returned unchanged.
    /// </summary>
    public static string Feminize(string surname)
    {
        if (string.IsNullOrWhiteSpace(surname)) return surname;
        var s = surname.Trim();

        if (s.EndsWith("ová", StringComparison.OrdinalIgnoreCase)) return s;
        if (s.EndsWith('á') || s.EndsWith('í') || s.EndsWith('ů')) return s;
        if (s.EndsWith('ý')) return s[..^1] + "á";
        if (s.EndsWith('a') || s.EndsWith('e') || s.EndsWith('ě') || s.EndsWith('o')) return s[..^1] + "ová";
        if (s.EndsWith("ěk", StringComparison.Ordinal) && s.Length > 3)
        {
            // The dropped ě softens a preceding d/t/n: Vaněk → Vaňková.
            var stem = s[..^2];
            return stem[..^1] + Soften(stem[^1]) + "ková";
        }
        if (s.EndsWith("ek", StringComparison.Ordinal) && s.Length > 3) return s[..^2] + "ková";
        if (s.EndsWith("ec", StringComparison.Ordinal) && s.Length > 3) return s[..^2] + "cová";
        if (s.EndsWith("el", StringComparison.Ordinal) && s.Length > 3) return s[..^2] + "lová";
        return s + "ová";
    }

    private static char Soften(char c) => c switch
    {
        'n' => 'ň',
        'd' => 'ď',
        't' => 'ť',
        _ => c
    };
}
