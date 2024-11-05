<h2>About</h2>
This package helps to generate interfaces with properties, methods, and properties' signatures based on your class.

<h2>Installing</h2>
Install the package:

```bash
dotnet add package Tooller.InterfaceGenerator --version 0.1.1
```

Add to your `.csproj` file this properties:

```yaml
<PropertyGroup>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild
    <IncludeBuildOutput>false</IncludeBuildOutput>
</PropertyGroup>
<ItemGroup>
    <None Include="$(OutputPath)\$(AssemblyName).dll" Pack="true" PackagePath="analyzers/dotnet/cs" Visible="false" />
</ItemGroup>
```

<h2>Usage</h2>

Create the class and add the Contract attribute:

namespace ToollerDemo;

```C#
[Contract]
public class UserService : IUserService
{
    public string GetUserId(Guid id)
    {
        return id.ToString();
    }
}
```
You'll get the generated interface:

```C#
namespace ToollerDemo;
public interface IUserService
{
    string GetUserId(System.Guid id);
}
```

If you want the custom namespace, use the Namespace attribute with the desired namespace value:

```C#
[Namespace("Domain")]
[Contract]
public class UserService : IUserService
{
    public string GetUserId(Guid id)
    {
        return id.ToString();
    }
} 
```

You'll get the generated interface with the desired namespace:

```C#
namespace Domain;
public interface IUserService
{
    string GetUserId(System.Guid id);
} 
```
