namespace Novak.Net.Names.Models;

/// <summary>
/// Popularity band of a name. Names are ranked by frequency and split by cumulative share of bearers:
/// the top 20 % of bearers are <see cref="Common"/>, the bottom 20 % <see cref="Rare"/>.
/// </summary>
public enum NameFrequency
{
    Common,
    Normal,
    Rare
}
