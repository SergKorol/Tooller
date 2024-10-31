namespace Tooller.Metadata;

public class Namespace : Attribute
{
    public Namespace(string targetNamespace) => TargetNamespace = targetNamespace;
    public string TargetNamespace { get; }
}