using System.Globalization;
using Novak.Net.Common.Data;
using Novak.Net.Names.Models;
using Novak.Net.Names.Options;

namespace Novak.Net.Names;

/// <summary>
/// Built-in data source: the most common Czech first names (~175) and surnames (~135),
/// weighted by approximate number of bearers.
/// </summary>
public sealed class DataSource : IDataSource
{
    private static readonly Lazy<NameRecord[]> FirstNames = new(LoadFirstNames);
    private static readonly Lazy<NameRecord[]> MaleFirstNames = new(() => FirstNames.Value.Where(r => r.Gender == Gender.Male).ToArray());
    private static readonly Lazy<NameRecord[]> FemaleFirstNames = new(() => FirstNames.Value.Where(r => r.Gender == Gender.Female).ToArray());
    private static readonly Lazy<NameRecord[]> Surnames = new(LoadSurnames);

    public IReadOnlyList<NameRecord> GetFirstNames(NameGeneratorOptions options) => options.Gender switch
    {
        Gender.Male => MaleFirstNames.Value,
        Gender.Female => FemaleFirstNames.Value,
        _ => FirstNames.Value
    };

    public IReadOnlyList<NameRecord> GetSurnames(NameGeneratorOptions options) => Surnames.Value;

    private static NameRecord[] LoadFirstNames() =>
        EmbeddedData.ReadRows(typeof(DataSource).Assembly, "first_names_cz.csv", hasHeader: true)
            .Select(r => new NameRecord
            {
                Text = r[0],
                Gender = Enum.Parse<Gender>(r[1], ignoreCase: true),
                Frequency = double.Parse(r[2], CultureInfo.InvariantCulture)
            })
            .ToArray();

    private static NameRecord[] LoadSurnames() =>
        EmbeddedData.ReadRows(typeof(DataSource).Assembly, "surnames_cz.csv", hasHeader: true)
            .Select(r => new NameRecord
            {
                Text = r[0],
                FeminineForm = r[1],
                Frequency = double.Parse(r[2], CultureInfo.InvariantCulture)
            })
            .ToArray();
}
