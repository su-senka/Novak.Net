using System.Globalization;
using System.Text;

namespace Novak.Net.Common.Text;

/// <summary>Text helpers for Czech strings.</summary>
public static class CzechText
{
    /// <summary>The cs-CZ culture used for casing and comparisons.</summary>
    public static CultureInfo Culture { get; } = CultureInfo.GetCultureInfo("cs-CZ");

    /// <summary>Strips diacritics: "Říha Šťastná" → "Riha Stastna".</summary>
    public static string RemoveDiacritics(string text)
    {
        var decomposed = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decomposed.Length);
        foreach (var c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
