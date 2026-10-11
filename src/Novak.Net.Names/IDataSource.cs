using Novak.Net.Names.Models;
using Novak.Net.Names.Options;

namespace Novak.Net.Names;

/// <summary>
/// Provides pools of names to sample from. Implement it to use your own name lists.
/// </summary>
public interface IDataSource
{
    /// <summary>First names, already filtered by <see cref="NameGeneratorOptions.Gender"/> when it is not Any.</summary>
    IReadOnlyList<NameRecord> GetFirstNames(NameGeneratorOptions options);

    /// <summary>Surnames in their masculine form, ideally with <see cref="NameRecord.FeminineForm"/> filled in.</summary>
    IReadOnlyList<NameRecord> GetSurnames(NameGeneratorOptions options);
}
