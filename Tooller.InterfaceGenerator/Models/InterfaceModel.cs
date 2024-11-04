using System.Collections.Generic;

namespace Tooller.InterfaceGenerator.Models;

public class InterfaceModel
{
    public string Namespace { get; set; }
    public string ClassName { get; set; }
    public IEnumerable<string> Methods { get; set; }
};