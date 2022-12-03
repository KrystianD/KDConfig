using System;
using KDConfig.Attributes;
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
    #region TestRequiredPresent
    private class TestRequiredPresentModel
    {
      [ConfigValue("value", RequiredEnum.Required)]
      public int Value = 999;
    }

    [Test]
    public void TestRequiredPresent()
    {
      var cfg = ParseYaml<TestRequiredPresentModel>(@"value: 1");

      Assert.AreEqual(1, cfg.Value);
    }
    #endregion

    #region TestRequiredMissing
    private class TestRequiredMissingModel
    {
      [ConfigValue("value", RequiredEnum.Required)]
      public int Value = 999;
    }

    [Test]
    public void TestRequiredMissing()
    {
      Assert.Throws<ConfigException>(() => {
        ParseYaml<TestRequiredMissingModel>(@"");
      });
    }
    #endregion

    #region TestOptionalPresent
    private class TestOptionalPresentModel
    {
      [ConfigValue("value", RequiredEnum.Optional)]
      public int Value = 999;
    }

    [Test]
    public void TestOptionalPresent()
    {
      var cfg = ParseYaml<TestOptionalPresentModel>(@"value: 1");

      Assert.AreEqual(1, cfg.Value);
    }
    #endregion

    #region TestOptionalMissing
    private class TestOptionalMissingModel
    {
      [ConfigValue("value", RequiredEnum.Optional)]
      public int Value = 999;
    }

    [Test]
    public void TestOptionalMissing()
    {
      var cfg = ParseYaml<TestOptionalMissingModel>(@"");

      Assert.AreEqual(999, cfg.Value);
    }
    #endregion
  }
}