using System;
using System.IO;
using System.Reflection;
using Novak.Net.Core.Abstractions;

namespace Novak.Net.Core.Data;

public sealed class EmbeddedDataSource : IDataSource
{
    private readonly Assembly _assembly;
    private readonly string _rootNamespace;

    public EmbeddedDataSource(Assembly assembly)
    {
        _assembly = assembly;
        _rootNamespace = assembly.GetName().Name ?? "CzechMind";
    }

    public string ReadText(string resourcePath)
    {
        using var stream = GetStream(resourcePath) ?? throw new InvalidOperationException($"Resource not found: {resourcePath}");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public byte[] ReadBytes(string resourcePath)
    {
        using var stream = GetStream(resourcePath) ?? throw new InvalidOperationException($"Resource not found: {resourcePath}");
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    public bool Exists(string resourcePath) => GetStream(resourcePath) is not null;

    private Stream? GetStream(string resourcePath)
    {
        var manifestName = $"{_rootNamespace}.Data.Resources.{resourcePath.Replace('/', '.')}";
        return _assembly.GetManifestResourceStream(manifestName);
    }
}