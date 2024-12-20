using System;

namespace Tooller.InterfaceGenerator.Attributes;

public class Namespace : Attribute
{
    public Namespace(string targetNamespace) => TargetNamespace = targetNamespace;
    public string TargetNamespace { get; }
}