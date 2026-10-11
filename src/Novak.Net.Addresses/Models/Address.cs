namespace Novak.Net.Addresses.Models;

/// <summary>A Czech postal address.</summary>
public sealed record Address
{
    /// <summary>Street name; null in villages without named streets (the address then uses <see cref="CityPart"/>).</summary>
    public string? Street { get; init; }

    /// <summary>Číslo popisné – the building's number within the city part, e.g. "681".</summary>
    public required string HouseNumber { get; init; }

    /// <summary>Číslo orientační – the number within the street, including letter suffix, e.g. "14" or "96a".</summary>
    public string? OrientationNumber { get; init; }

    /// <summary>Municipality (obec), e.g. "Praha", "Brno", "Žeranovice".</summary>
    public required string Municipality { get; init; }

    /// <summary>Část obce, e.g. "Staré Město", "Veveří".</summary>
    public required string CityPart { get; init; }

    /// <summary>City district (městský obvod/část) in statutory cities, e.g. "Praha 4", "Brno-střed".</summary>
    public string? District { get; init; }

    /// <summary>Town used in the postal line: "Praha 1"–"Praha 10" in Prague, otherwise the municipality.</summary>
    public required string PostTown { get; init; }

    /// <summary>Postal code (PSČ) as 5 digits, e.g. "11000".</summary>
    public required string PostalCode { get; init; }

    /// <summary>Region (kraj).</summary>
    public required CzechRegion Region { get; init; }

    /// <summary>Postal code in the usual "110 00" form.</summary>
    public string PostalCodeFormatted => PostalCode.Length == 5 ? $"{PostalCode[..3]} {PostalCode[3..]}" : PostalCode;

    /// <summary>House number as written on the address: "681/14" when there is an orientation number, else "681".</summary>
    public string Number => OrientationNumber is null ? HouseNumber : $"{HouseNumber}/{OrientationNumber}";

    /// <summary>First address line: "Vodičkova 681/14", or "Lhota 25" where there are no streets.</summary>
    public string StreetLine => $"{Street ?? CityPart} {Number}";

    /// <summary>Postal line: "110 00 Praha 1".</summary>
    public string TownLine => $"{PostalCodeFormatted} {PostTown}";

    /// <summary>Address lines separated by <paramref name="separator"/> (default: new line).</summary>
    public string ToMultiline(string separator = "\n") => StreetLine + separator + TownLine;

    /// <summary>"Vodičkova 681/14, 110 00 Praha 1".</summary>
    public override string ToString() => $"{StreetLine}, {TownLine}";
}
