using KDConfig;
using KDConfig.Attributes;
using KDConfig.Provider.Yaml;

// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable ConvertToConstant.Local

namespace Example1;

internal static class Program
{
  private class DBConfig
  {
    [ConfigValue("host", RequiredEnum.Required)]
    public readonly string Hostname = null!;

    /*
     * UseDefaultValue - allows option without value to be present in the configuration (for configuration file clarity)
     */
    [ConfigValue("port", RequiredEnum.Optional, EmptyHandling.UseDefaultValue)]
    public readonly int Port = 5432;

    [ConfigValue("user", RequiredEnum.Required)]
    public readonly string Username = null!;

    [ConfigValue("pass", RequiredEnum.Required)]
    public readonly string Password = null!;

    [ConfigValue("name", RequiredEnum.Optional)]
    public readonly string Name = "postgres";
  }

  private class Config
  {
    [ConfigValue("db")]
    public readonly DBConfig DB = null!;
  }

  public static void Main()
  {
    var configFile = @"
db:
  host: localhost
  port: 
  user: user
  pass: pass
".Trim();

    try {
      var provider = YamlConfigDataProvider.FromYamlString(configFile);
      var cfg = ConfigParser.CreateFrom<Config>(provider);
      Console.WriteLine($"Hostname = {cfg.DB.Hostname}");
      Console.WriteLine($"Port     = {cfg.DB.Port}");
      Console.WriteLine($"Username = {cfg.DB.Username}");
      Console.WriteLine($"Password = {cfg.DB.Password}");
      Console.WriteLine($"Name     = {cfg.DB.Name}");
    }
    catch (ConfigException e) {
      Console.WriteLine(e);
    }
  }
}