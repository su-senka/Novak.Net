using System;
using System.Text;
using Novak.Net.PhoneNumbers.Models;
using Novak.Net.PhoneNumbers.Options;
using Novak.Net.Common.Randomization;

namespace Novak.Net.PhoneNumbers;

public class PhoneNumberGenerator : IPhoneNumberGenerator
{
    private readonly IPhoneNumberDataRepository _repo;

    public PhoneNumberGenerator(IPhoneNumberDataRepository repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public PhoneNumber GeneratePhoneNumber(PhoneNumberGeneratorOptions? options = null)
    {
        options ??= new PhoneNumberGeneratorOptions();
        var rng = DefaultRandom.Create(options.Seed);
        var country = options.CountryCode ?? _repo.GetCountryCode(options.Region);
        if (string.IsNullOrWhiteSpace(country))
            throw new InvalidOperationException("No country code available.");
        var prefixes = _repo.GetPhonePrefixes(options.Region, options.MobileOnly);
        if (prefixes is null || prefixes.Count == 0)
            throw new InvalidOperationException("No phone prefixes available.");
        var prefix = prefixes[rng.Next(prefixes.Count)];

        int remaining = Math.Max(0, 9 - prefix.Length);
        var sb = new StringBuilder(prefix);
        for (int i = 0; i < remaining; i++) sb.Append(rng.Next(10));
        var nsn = sb.ToString();

        return options.Format switch
        {
            PhoneNumberFormat.E164   => new PhoneNumber($"+{country}{nsn}"),
            PhoneNumberFormat.Plain  => new PhoneNumber($"{country}{nsn}"),
            PhoneNumberFormat.Spaced => new PhoneNumber($"+{country} {nsn[..3]} {nsn[3..6]} {nsn[6..]}"),
            _                        => new PhoneNumber($"+{country}{nsn}")
        };
    }
}
