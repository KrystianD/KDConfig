using KDConfig.Provider.Ini;
using NUnit.Framework;

#pragma warning disable 649
#pragma warning disable 414
// ReSharper disable StringLiteralTypo
// ReSharper disable ConvertToConstant.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace KDConfig.Test
{
  public class TestsIni
  {
    private T ParseIni<T>(string yaml) where T : new() => ConfigParser.CreateFrom<T>(IniConfigDataProvider.FromString(yaml));

    private class Model
    {
      [ConfigValue("section.value_int")]
      public int ValueInt;
    }

    [Test]
    public void TestIni()
    {
      var cfg = ParseIni<Model>(@"
[section]
value_int=1");

      Assert.AreEqual(1, cfg.ValueInt);
    }
  }
}