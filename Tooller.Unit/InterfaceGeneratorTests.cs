using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Tooller.SourceGenerator.InterfaceGenerator;

namespace Tooller.Unit;

public class InterfaceGeneratorTests
{
    [Fact]
    public void GenerateInterface_WithoutCustomNamespace()
    {
        var source = @"
        namespace Tooller.Unit;
        [Contract]
        public class UserClass
        {
            public void UserMethod() { }
            public int GetValue(int param) => param * 2;
        }";

        var expected = @"
namespace Tooller.Unit;
public interface IUserClass
{
    void UserMethod();
    int GetValue(int param);
}
        ".Trim();
        var generatedCode = GetGeneratedOutput(source);
        generatedCode.Should().NotBeNull();
        generatedCode.Should().Be(expected);
    }
    
    [Fact]
    public void GenerateInterface_WithCustomNamespace()
    {
        var source = @"
    using System;

    [Namespace(""Tooller.CustomNamespace"")]
    [Contract]
    public class UserClass
    {
        public void UserMethod() { }
        public int GetValue(int param) => param * 2;
    }";

        var expected = @"
namespace Tooller.CustomNamespace;
public interface IUserClass
{
    void UserMethod();
    int GetValue(int param);
}
    ".Trim();

        var generatedCode = GetGeneratedOutput(source);
        generatedCode.Should().NotBeNull();
        generatedCode.Should().Be(expected);
    }

    private static string? GetGeneratedOutput(string sourceCode)
    {
        var namespaceAttributeSource = @"
    using System;
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class Namespace : Attribute
    {
        public Namespace(string targetNamespace) => TargetNamespace = targetNamespace;
        public string TargetNamespace { get; }
    }
    ";

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        var attributeSyntaxTree = CSharpSyntaxTree.ParseText(namespaceAttributeSource);

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
            .Cast<MetadataReference>();

        var compilation = CSharpCompilation.Create("SourceGeneratorTests",
            syntaxTrees: [syntaxTree, attributeSyntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new InterfaceGenerator();
        CSharpGeneratorDriver.Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);

        return outputCompilation.SyntaxTrees.SingleOrDefault(t => t.FilePath.Contains("IUserClass.g.cs"))?.ToString();
    }
}