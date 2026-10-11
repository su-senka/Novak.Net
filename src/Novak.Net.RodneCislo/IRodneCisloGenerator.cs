using Novak.Net.RodneCislo.Options;

namespace Novak.Net.RodneCislo;

/// <summary>Generates Czech birth numbers.</summary>
public interface IRodneCisloGenerator
{
    Models.RodneCislo Generate(RodneCisloGeneratorOptions? options = null);
}
