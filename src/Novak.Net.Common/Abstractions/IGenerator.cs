namespace Novak.Net.Common.Abstractions;

public interface IGenerator<out T>
{
    T Generate();
}