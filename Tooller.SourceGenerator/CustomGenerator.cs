// using System.Text;
// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.Text;
//
// namespace Tooller.SourceGenerator;
//
// public class CustomGenerator : IIncrementalGenerator
// {
//     public void Initialize(IncrementalGeneratorInitializationContext context)
//     {
//         context.RegisterPostInitializationOutput(static postInitializationContext => {
//             postInitializationContext.AddSource("myGeneratedFile.cs", SourceText.From("""
//                 using System;
//
//                 namespace GeneratedNamespace
//                 {
//                     internal sealed class GeneratedAttribute : Attribute
//                     {
//                     }
//                 }
//                 """, Encoding.UTF8));
//         });
//     }
// }