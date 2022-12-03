using System;
using KDConfig.Attributes;
using KDConfig.Provider.Yaml;
using NUnit.Framework;

#pragma warning disable 649
#pragma warning disable 414
// ReSharper disable StringLiteralTypo
// ReSharper disable ConvertToConstant.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace KDConfig.Test
{
  public partial class Tests
  {
    private T ParseYaml<T>(string yaml) where T : new() => ConfigParser.CreateFrom<T>(YamlConfigDataProvider.FromYamlString(yaml));

    #region TestSimple
    private class TestSimpleModel
    {
      [ConfigValue("section.value_int")]
      public int ValueInt;

      [ConfigValue("section.value_str")]
      public string ValueStr { get; }

      [ConfigValue("section.value_strint")]
      public string ValueStrInt { get; }
    }

    [Test]
    public void TestSimple()
    {
      var cfg = ParseYaml<TestSimpleModel>(@"
section:
  value_int: 123
  value_str: abc
  value_strint: 456
");

      Assert.AreEqual(123, cfg.ValueInt);
      Assert.AreEqual("abc", cfg.ValueStr);
      Assert.AreEqual("456", cfg.ValueStrInt);
    }
    #endregion
  }
}