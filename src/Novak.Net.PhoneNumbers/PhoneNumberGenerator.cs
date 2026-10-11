using System.Text;
using Novak.Net.Common.Randomization;
using Novak.Net.PhoneNumbers.Data;
using Novak.Net.PhoneNumbers.Models;
using Novak.Net.PhoneNumbers.Options;

namespace Novak.Net.PhoneNumbers;

/// <summary>Default <see cref="IPhoneNumberGenerator"/> implementation.</summary>
public class PhoneNumberGenerator : IPhoneNumberGenerator
{
    private const int NationalNumberLength = 9;

    private readonly IPhoneNumberDataRepository _repo;
    private readonly IRandom _random;

    /// <param name="repo">Prefix data; defaults to the built-in Czech numbering plan.</param>
    /// <param name="random">Random source used when options carry no seed.</param>
    public PhoneNumberGenerator(IPhoneNumberDataRepository? repo = null, IRandom? random = null)
    {
        _repo = repo ?? new EmbeddedPhoneNumberDataRepository();
        _random = random ?? DefaultRandom.Shared;
    }

    public PhoneNumber GeneratePhoneNumber(PhoneNumberGeneratorOptions? options = null)
    {
        options ??= new PhoneNumberGeneratorOptions();
        var rng = _random.For(options);

        var type = options.Type ?? (rng.Chance(0.85) ? PhoneNumberType.Mobile : PhoneNumberType.Landline);
        var prefixes = _repo.GetPhonePrefixes().Where(p => p.Type == type).ToList();
        if (prefixes.Count == 0)
            throw new InvalidOperationException($"No {type} phone prefixes available.");
        var prefix = rng.PickWeighted(prefixes, p => p.Weight).Prefix;

        var sb = new StringBuilder(prefix, NationalNumberLength);
        while (sb.Length < NationalNumberLength) sb.Append((char)('0' + rng.Next(10)));

        return new PhoneNumber(sb.ToString(), type, options.Format);
    }
}
