using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Novak.Net.Core.Modules.Emails.Data;

public sealed class EmbeddedEmailDataRepository : IEmailDataRepository
{
    public IReadOnlyList<string> GetEmailDomains(string? region)
    {
        var list = ReadCsv("email_domains.csv").Select(r => r[0]).ToList();
        if (list.Count == 0) return new[] { "example.com" };
        return list;
    }

    private static IEnumerable<string[]> ReadCsv(string fileName)
    {
        var asm = Assembly.GetExecutingAssembly();
        var res = asm.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith($"Emails.Data.{fileName}", StringComparison.OrdinalIgnoreCase));
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

