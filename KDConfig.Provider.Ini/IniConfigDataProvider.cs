using System;
using System.IO;
using IniParser;

namespace KDConfig.Provider.Ini
{
  public class IniConfigDataProvider : IConfigDataProvider
  {
    public bool IsFixedType => false;

    private readonly IniData _data;
    private readonly string _configDirectory;

    private IniConfigDataProvider(IniData data, string configDirectory)
    {
      _data = data;
      _configDirectory = configDirectory;
    }

    public object GetScalar(string dotPath)
    {
      var parts = dotPath.Split('.');

      switch (parts.Length) {
        case 1: return _data.Global.FindByKey(parts[0])?.Value;
        case 2: return _data.Sections.FindByName(parts[0])?.Properties.FindByKey(parts[1])?.Value;
        default: throw new Exception("invalid path");
      }
    }

    public static IniConfigDataProvider FromString(string iniString)
    {
      var parser = new IniDataParser();
      IniData data = parser.Parse(iniString);
      return new IniConfigDataProvider(data, null);
    }

    public static IConfigDataProvider FromFile(string path)
    {
      var parser = new IniDataParser();
      IniData data = parser.Parse(new StreamReader(path));
      return new IniConfigDataProvider(data, Path.GetDirectoryName(path));
    }
  }
}