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