using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace Novak.Net.Common.Data;

/// <summary>Reads the delimited data files the modules ship as embedded resources.</summary>
public static class EmbeddedData
{
    /// <summary>
    /// Returns the non-empty, non-comment (<c>#</c>) lines of an embedded resource whose name ends with
    /// <c>.Data.{fileName}</c>, split by <paramref name="separator"/>. Files ending in <c>.gz</c> are decompressed.
    /// Throws when the resource is missing so packaging mistakes surface immediately.
    /// </summary>
    public static IReadOnlyList<string[]> ReadRows(Assembly assembly, string fileName, char separator = ',', bool hasHeader = false)
    {
        var resource = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith(".Data." + fileName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Embedded data file '{fileName}' not found in {assembly.GetName().Name}.");

        using var raw = assembly.GetManifestResourceStream(resource)!;
        using var stream = fileName.EndsWith(".gz", StringComparison.OrdinalIgnoreCase)
            ? new GZipStream(raw, CompressionMode.Decompress)
            : raw;
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

        var rows = new List<string[]>();
        bool skipHeader = hasHeader;
        while (reader.ReadLine() is { } line)
        {
            if (line.Length == 0 || line[0] == '#') continue;
            if (skipHeader) { skipHeader = false; continue; }
            rows.Add(line.Split(separator));
        }
        return rows;
    }
}
