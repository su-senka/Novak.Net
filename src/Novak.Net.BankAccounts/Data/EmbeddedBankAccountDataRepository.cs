using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Novak.Net.BankAccounts.Data;

public sealed class EmbeddedBankAccountDataRepository : IBankAccountDataRepository
{
    public IReadOnlyList<string> GetBankCodes()
    {
        var codes = ReadCsv("bank_codes_cz.csv")
            .Where(r => r.Length >= 1)
            .Select(r => r[0].Trim())
            .ToList();
        return codes.Count == 0 ? new[] { "0800" } : codes;
    }

    private static IEnumerable<string[]> ReadCsv(string fileName)
    {
        var asm = Assembly.GetExecutingAssembly();
        var res = asm.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith($"BankAccounts.Data.{fileName}", StringComparison.OrdinalIgnoreCase));
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
