namespace Novak.Net.Common.Abstractions;

public interface IDataSource
{
    string ReadText(string resourcePath);
    byte[] ReadBytes(string resourcePath);
    bool Exists(string resourcePath);
}