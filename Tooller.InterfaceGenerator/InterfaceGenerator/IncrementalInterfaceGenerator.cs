using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Tooller.InterfaceGenerator.Models;

namespace Tooller.InterfaceGenerator.InterfaceGenerator;

#nullable enable
[Generator]
public class IncrementalInterfaceGenerator : IIncrementalGenerator
{

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var pipeline = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (syntaxNode, _) =>
                    syntaxNode is ClassDeclarationSyntax classDecl &&
                    classDecl.AttributeLists
                        .SelectMany(attrList => attrList.Attributes)
                        .Any(attr => attr.Name.ToString().Contains("Contract")),
                transform: (syntaxContext, _) =>
                {
                    var containingClass = (INamedTypeSymbol)syntaxContext.SemanticModel.GetDeclaredSymbol(syntaxContext.Node)!;
                    string? customNamespace = containingClass.GetAttributes()
                        .FirstOrDefault(attr => attr.AttributeClass?.Name == "Namespace")
                        ?.ConstructorArguments.FirstOrDefault().Value?.ToString();

                    var targetNamespace = customNamespace ?? containingClass.ContainingNamespace?.ToDisplayString(
                        SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)
                    );

                    var methodSignatures = containingClass.GetMembers()
                        .OfType<IMethodSymbol>()
                        .Where(method => method.MethodKind != MethodKind.Constructor)
                        .Select(FormatMethodSignature)
                        .ToList();
                    return new InterfaceModel
                    {
                        Namespace = targetNamespace,
                        ClassName = containingClass.Name,
                        Methods = methodSignatures.Any() ? methodSignatures : new List<string>()
                    };
                }
            );

        context.RegisterSourceOutput(pipeline, static (context, model) =>
        {
            if (model == null || !model.Methods.Any()) return;

            var sourceText = SourceText.From($$"""
                                               namespace {{model.Namespace}};
                                               public interface I{{model.ClassName}}
                                               {
                                                   {{string.Join("\n    ", model.Methods)}}
                                               }
                                               """, Encoding.UTF8);

            context.AddSource($"I{model.ClassName}.g.cs", sourceText);
        });
    }

    private string FormatMethodSignature(IMethodSymbol method)
    {
        var returnType = method.ReturnType.ToDisplayString();
        var methodName = method.Name;
        var parameters = string.Join(", ", method.Parameters.Select(param => $"{param.Type} {param.Name}"));
        return $"{returnType} {methodName}({parameters});";
    }
}