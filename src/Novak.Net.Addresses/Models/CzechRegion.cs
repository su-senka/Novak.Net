namespace Novak.Net.Addresses.Models;

/// <summary>The 14 Czech regions (kraje). Values are the RÚIAN VÚSC codes.</summary>
public enum CzechRegion
{
    Praha = 19,
    Stredocesky = 27,
    Jihocesky = 35,
    Plzensky = 43,
    Karlovarsky = 51,
    Ustecky = 60,
    Liberecky = 78,
    Kralovehradecky = 86,
    Pardubicky = 94,
    Vysocina = 108,
    Jihomoravsky = 116,
    Olomoucky = 124,
    Moravskoslezsky = 132,
    Zlinsky = 141
}

/// <summary>Helpers for <see cref="CzechRegion"/>.</summary>
public static class CzechRegionExtensions
{
    /// <summary>Official Czech name, e.g. "Jihomoravský kraj".</summary>
    public static string ToCzechName(this CzechRegion region) => region switch
    {
        CzechRegion.Praha => "Hlavní město Praha",
        CzechRegion.Stredocesky => "Středočeský kraj",
        CzechRegion.Jihocesky => "Jihočeský kraj",
        CzechRegion.Plzensky => "Plzeňský kraj",
        CzechRegion.Karlovarsky => "Karlovarský kraj",
        CzechRegion.Ustecky => "Ústecký kraj",
        CzechRegion.Liberecky => "Liberecký kraj",
        CzechRegion.Kralovehradecky => "Královéhradecký kraj",
        CzechRegion.Pardubicky => "Pardubický kraj",
        CzechRegion.Vysocina => "Kraj Vysočina",
        CzechRegion.Jihomoravsky => "Jihomoravský kraj",
        CzechRegion.Olomoucky => "Olomoucký kraj",
        CzechRegion.Moravskoslezsky => "Moravskoslezský kraj",
        CzechRegion.Zlinsky => "Zlínský kraj",
        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
    };
}
