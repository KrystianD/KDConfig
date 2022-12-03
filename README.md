KDConfig
======

Yet another, highly configurable config files reader and parser with detailed configuration errors reporting.
Supports YAML and INI config providers.

## Features

* Support for nested keys,
* Support for C# objects as nested config items,
* Support for path config items (checking for file/directory existence and type),
* Configurable empty values handling,
* Using class member initial values as default values,
* Automatic type coercion where it seems natural (eg. `"yes"` as `true` for boolean config items).

## Documentation

#### ConfigValue attribute

```ConfigValue(string path, RequiredEnum required, EmptyHandling emptyHandling)```

`required` -

* `RequiredEnum.Required` - **default**, value is required not to be null (either present in the config file or with the default
  value as set by `EmptyHandling` value),
* `RequiredEnum.Optional` - value is optional,

`emptyHandling` -

* `EmptyHandling.NotAllowed` - **default**, empty value is not allowed
* `EmptyHandling.AsIs` - empty value is passed as is, ie. `name: ` will be `name = ""`,
* `EmptyHandling.AsNull` - treat empty value as if null was passed, ie. `name: ` will be `name = null`,
* `EmptyHandling.UseDefaultValue` - use default value (from the C# object) if value is empty.
  If member is nullable, there is no default value and config item is `Required`, error will be raised.

#### ConfigPath attribute

```ConfigPath(PathType pathType, PathRelativeTo pathRelativeTo, bool mustExists)```

`pathType` -

* `PathType.FileOrDirectory` - path must be a file or directory,
* `PathType.File` - path must be a file, throw an error if a directory is passed,
* `PathType.Directory` - path must be a directory, throw an error if a file is passed,

`pathRelativeTo` -

* `PathRelativeTo.ConfigFile` - **default**, resulting path will be relative to config file path,
* `PathRelativeTo.WorkingDirectory` - resulting path will be relative to the current working directory,

`mustExists` - **default = true**, throw an error, if file doesn't exist.

## Examples

* [Database configuration](Examples/Example1/Program.cs)
* [Address book](Examples/Example2/Program.cs)

```csharp
using KDConfig;
using KDConfig.Attributes;
using KDConfig.Provider.Yaml;

namespace ExampleREADME;

internal static class Program
{
  private class Config
  {
    [ConfigValue("db.host", RequiredEnum.Required)]
    public readonly string Hostname = null!;

    [ConfigValue("db.port", RequiredEnum.Optional, EmptyHandling.UseDefaultValue)]
    public readonly int Port = 5432;
  }

  public static void Main()
  {
    var configFile = @"
db:
  host: localhost
  port: 15432
".Trim();

    try {
      var provider = YamlConfigDataProvider.FromYamlString(configFile);
      var cfg = ConfigParser.CreateFrom<Config>(provider);
      Console.WriteLine($"Hostname = {cfg.Hostname}");
      Console.WriteLine($"Port     = {cfg.Port}");
    }
    catch (ConfigException e) {
      Console.WriteLine(e);
    }
  }
}
```

## See also

* [Config.NET](https://github.com/aloneguid/config) - great project that I used to use before I wrote my own library.