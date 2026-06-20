using Novak.Net.RodneCislo.Options;

namespace Novak.Net.RodneCislo;

public interface IRodneCisloGenerator
{
    Models.RodneCislo Generate(RodneCisloGeneratorOptions? options = null);
}
