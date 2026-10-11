using Novak.Net.Names.Models;
using Novak.Net.Names.Options;

namespace Novak.Net.Names;

/// <summary>Generates realistic Czech names.</summary>
public interface INameGenerator
{
    /// <summary>First name and gender-consistent surname (a woman gets the feminine surname form).</summary>
    PersonName GenerateName(NameGeneratorOptions? options = null);

    string GenerateFirstName(NameGeneratorOptions? options = null);

    /// <summary>Surname; feminine form for <see cref="Gender.Female"/>, random gender for <see cref="Gender.Any"/>.</summary>
    string GenerateSurname(NameGeneratorOptions? options = null);

    /// <summary>Same as <see cref="GenerateName"/> formatted as "FirstName LastName".</summary>
    string GenerateFullName(NameGeneratorOptions? options = null);

    /// <summary>
    /// Generates <paramref name="count"/> full names. When the options carry a seed, one random sequence is shared
    /// across the batch, so the result is reproducible but the names differ from each other.
    /// </summary>
    IEnumerable<string> GenerateMany(Func<NameGeneratorOptions> optionsFactory, int count);
}
