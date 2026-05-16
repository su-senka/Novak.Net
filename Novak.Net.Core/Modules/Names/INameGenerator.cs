using System;
using System.Collections.Generic;
using Novak.Net.Core.Modules.Names.Options;

namespace Novak.Net.Core.Modules.Names;

public interface INameGenerator
{
    string GenerateFirstName(NameGeneratorOptions? options = null);
    string GenerateSurname(NameGeneratorOptions? options = null);
    string GenerateFullName(NameGeneratorOptions? options = null);
    IEnumerable<string> GenerateMany(Func<NameGeneratorOptions> optionsFactory, int count);
}