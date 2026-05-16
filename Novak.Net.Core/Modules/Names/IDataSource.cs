using System.Collections.Generic;
using Novak.Net.Core.Modules.Names.Models;
using Novak.Net.Core.Modules.Names.Options;

namespace Novak.Net.Core.Modules.Names;

/// <summary>
/// Provides filtered pools of names. Implementations may prefilter efficiently.
/// </summary>
public interface IDataSource
{
    /// <summary>
    /// Get a filtered pool of first names honoring options such as Gender and Region.
    /// </summary>
    /// <param name="options">The options to filter the first names.</param>
    /// <returns>A filtered list of first names.</returns>
    IReadOnlyList<NameRecord> GetFirstNames(NameGeneratorOptions options);

    /// <summary>
    /// Get a filtered pool of surnames honoring options such as Region.
    /// </summary>
    /// <param name="options">The options to filter the surnames.</param>
    /// <returns>A filtered list of surnames.</returns>
    IReadOnlyList<NameRecord> GetSurnames(NameGeneratorOptions options);
}
