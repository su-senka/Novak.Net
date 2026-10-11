namespace Novak.Net.People;

/// <summary>Generates complete, internally consistent fictional people.</summary>
public interface IPersonGenerator
{
    Person GeneratePerson(PersonGeneratorOptions? options = null);
}
