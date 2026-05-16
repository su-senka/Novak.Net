namespace Novak.Net.Core.Abstractions;

public interface IGenerator<out T>
{
    T Generate();
}