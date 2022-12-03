using KDConfig;
using KDConfig.Attributes;
using KDConfig.Provider.Yaml;

// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable ConvertToConstant.Local

namespace Example2;

internal static class Program
{
  private class BookEntry
  {
    [ConfigValue("name", RequiredEnum.Required)]
    public readonly string Name = null!;

    [ConfigValue("address.street", RequiredEnum.Required)]
    public readonly string AddressStreet = null!;

    [ConfigValue("address.city", RequiredEnum.Required)]
    public readonly string AddressCity = null!;
  }

  private class Config
  {
    [ConfigValue("book")]
    public readonly BookEntry[] Book = null!;
  }

  public static void Main()
  {
    var configFile = @"
book:
  - name: John
    address:
      street: 197 Marvin Rd SE
      city: Lacey
  - name: Bob
    address:
      street: 8150 Florida Blvd
      city: Baton Rouge
".Trim();

    try {
      var provider = YamlConfigDataProvider.FromYamlString(configFile);
      var cfg = ConfigParser.CreateFrom<Config>(provider);
      foreach (var entry in cfg.Book) {
        Console.WriteLine($"Name          = {entry.Name}");
        Console.WriteLine($"AddressStreet = {entry.AddressStreet}");
        Console.WriteLine($"AddressCity   = {entry.AddressCity}");
      }
    }
    catch (ConfigException e) {
      Console.WriteLine(e);
    }
  }
}