using DevToys.Api;
using System.ComponentModel.Composition;

namespace DevToys.CSharpToTypescript;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(CSharpToTypescriptAssemblyIdentifier))]
internal sealed class CSharpToTypescriptAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync()
    {
        throw new NotImplementedException();
    }
}