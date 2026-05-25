using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Novak.Net.PhoneNumbers.Data;

public sealed class EmbeddedPhoneNumberDataRepository : IPhoneNumberDataRepository
{
    public IReadOnlyList<string> GetPhonePrefixes(string? region, bool mobileOnly)
    {
        var rows = ReadCsv("phone_prefixes_cz.csv");
        var list = new List<string>();
        foreach (var r in rows)
        {
            if (r.Length < 2) continue;
            var prefix = r[0];
            var isMobile = r[1] == "1";
            if (mobileOnly && !isMobile) continue;
            list.Add(prefix);
        }
        return list.Count == 0 ? new[] { "602" } : list;
    }

    public string GetCountryCode(string? region)
    {
        var rows = ReadCsv("country_codes.csv");
        foreach (var r in rows)
        {
            if (r.Length < 2) continue;
            if (string.Equals(r[0], (region ?? "CZ"), StringComparison.OrdinalIgnoreCase))
                return r[1].TrimStart('+');
        }
        return "+420".TrimStart('+');
    }

    private static IEnumerable<string[]> ReadCsv(string fileName)
    {
        var asm = Assembly.GetExecutingAssembly();
        var res = asm.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith($"PhoneNumbers.Data.{fileName}", StringComparison.OrdinalIgnoreCase));
        if (res is null) yield break;
        using var stream = asm.GetManifestResourceStream(res);
        if (stream is null) yield break;
        using var sr = new StreamReader(stream, Encoding.UTF8, true);
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            line = line.Trim();
            if (line.Length == 0 || line.StartsWith("#")) continue;
            yield return line.Split(',');
        }
    }
}

