using System;

namespace Tooller.Interface.Attributes;

public class Namespace : Attribute
{
    public Namespace(string targetNamespace) => TargetNamespace = targetNamespace;
    public string TargetNamespace { get; }
}