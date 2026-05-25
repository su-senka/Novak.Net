using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Novak.Net.Names.Models;
using Novak.Net.Names.Options;

namespace Novak.Net.Names;

public sealed class DataSource : IDataSource
{
    private static readonly Lazy<IReadOnlyList<NameRecord>> FirstNames = new(LoadFirstNames);
    private static readonly Lazy<IReadOnlyList<NameRecord>> Surnames = new(LoadSurnames);

    public IReadOnlyList<NameRecord> GetFirstNames(NameGeneratorOptions options)
    {
        var q = FirstNames.Value.AsEnumerable();
        if (options.Gender == Gender.Male) q = q.Where(r => r.Gender == Gender.Male);
        else if (options.Gender == Gender.Female) q = q.Where(r => r.Gender == Gender.Female);
        if (!string.IsNullOrWhiteSpace(options.Region))
            q = q.Where(r => string.Equals(r.Region, options.Region, StringComparison.OrdinalIgnoreCase));
        return q.ToArray();
    }

    public IReadOnlyList<NameRecord> GetSurnames(NameGeneratorOptions options)
    {
        var q = Surnames.Value.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(options.Region))
            q = q.Where(r => string.Equals(r.Region, options.Region, StringComparison.OrdinalIgnoreCase));
        return q.ToArray();
    }

    private static IReadOnlyList<NameRecord> LoadFirstNames()
    {
        var list = new List<NameRecord>(64);
        using var reader = OpenResourceStream("first_names.demo.csv");
        using var sr = new StreamReader(reader, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        sr.ReadLine(); // header
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = SplitCsv(line);
            if (parts.Length < 5) continue;
            list.Add(new NameRecord
            {
                Text = parts[0],
                Gender = ParseGender(parts[1]),
                Frequency = double.Parse(parts[2], CultureInfo.InvariantCulture),
                Region = string.IsNullOrWhiteSpace(parts[3]) ? null : parts[3],
                Archaic = parts[4] == "1",
                Genderable = false,
                Foreign = false,
                Indeclinable = false
            });
        }
        return list;
    }

    private static IReadOnlyList<NameRecord> LoadSurnames()
    {
        var list = new List<NameRecord>(64);
        using var reader = OpenResourceStream("surnames.demo.csv");
        using var sr = new StreamReader(reader, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        sr.ReadLine(); // header
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = SplitCsv(line);
            if (parts.Length < 6) continue;
            list.Add(new NameRecord
            {
                Text = parts[0],
                Gender = Gender.Any,
                Frequency = double.Parse(parts[2], CultureInfo.InvariantCulture),
                Region = string.IsNullOrWhiteSpace(parts[3]) ? null : parts[3],
                Genderable = parts[1] == "1",
                Foreign = parts[4] == "1",
                Indeclinable = parts[5] == "1",
                Archaic = false
            });
        }
        return list;
    }

    private static Stream OpenResourceStream(string fileName)
    {
        var asm = Assembly.GetExecutingAssembly();
        var resName = asm.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith($"Data.{fileName}", StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Embedded resource not found: {fileName}");
        return asm.GetManifestResourceStream(resName)
            ?? throw new InvalidOperationException($"Cannot open embedded resource: {resName}");
    }

    private static Gender ParseGender(string value)
        => value.Equals("Male", StringComparison.OrdinalIgnoreCase) ? Gender.Male
         : value.Equals("Female", StringComparison.OrdinalIgnoreCase) ? Gender.Female
         : Gender.Any;

    private static string[] SplitCsv(string line) => line.Split(',');
}
