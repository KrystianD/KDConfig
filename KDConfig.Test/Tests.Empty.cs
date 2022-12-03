using System;
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
    #region Empty handling tests - AsMissing
    private class TestEmptyAsMissingModel
    {
      [ConfigValue("value", RequiredEnum.Required, EmptyHandling.NotAllowed)]
      public int Value = 999;
    }

    [Test]
    public void TestEmptyAsMissing()
    {
      Assert.Throws<ConfigException>(() => {
        ParseYaml<TestEmptyAsMissingModel>(@"value: ");
      });
    }
    #endregion

    #region Empty handling tests - AsIs
    private class TestEmptyAsIsNullableModel
    {
      [ConfigValue("value", RequiredEnum.Optional, EmptyHandling.AsIs)]
      public string Value = "a";
    }

    [Test]
    public void TestEmptyAsIsNullable()
    {
      var cfg = ParseYaml<TestEmptyAsIsNullableModel>(@"value: ");
      Assert.AreEqual("", cfg.Value);
    }

    private class TestEmptyAsIsNotNullableModel
    {
      [ConfigValue("value", RequiredEnum.Optional, EmptyHandling.AsIs)]
      public int Value = 999;
    }

    [Test]
    public void TestEmptyAsIsNotNullable()
    {
      Assert.Throws<ConfigException>(() => {
        ParseYaml<TestEmptyAsIsNotNullableModel>(@"value: ");
      });
    }
    #endregion

    #region Empty handling tests - AsNull
    private class TestEmptyAsNullNotNullableModel
    {
      [ConfigValue("value_nonnullable", RequiredEnum.Optional, EmptyHandling.AsNull)]
      public int ValueNonNullable = 999;
    }

    [Test]
    public void TestEmptyAsNullNotNullable()
    {
      Assert.Throws<ConfigException>(() => {
        ParseYaml<TestEmptyAsNullNotNullableModel>(@"value_nonnullable: ");
      });
    }

    private class TestEmptyAsNullNullableModel
    {
      [ConfigValue("value_nullable", RequiredEnum.Optional, EmptyHandling.AsNull)]
      public int? ValueNullable = 999;

      [ConfigValue("value_nullable_str", RequiredEnum.Optional, EmptyHandling.AsNull)]
      public string ValueNullableStr = "test";
    }

    [Test]
    public void TestEmptyAsNullNullable()
    {
      var cfg = ParseYaml<TestEmptyAsNullNullableModel>(@"
value_nullable: 
value_nullable_str: ");
      Assert.IsNull(cfg.ValueNullable);
      Assert.IsNull(cfg.ValueNullableStr);
    }
    #endregion

    #region Empty handling tests - UseDefaultValue
    private class TestEmptyUseDefaultValueModel
    {
      [ConfigValue("value", RequiredEnum.Optional, EmptyHandling.UseDefaultValue)]
      public int Value = 999;
    }

    [Test]
    public void TestOptionalEmptyUseDefaultValue()
    {
      var cfg = ParseYaml<TestEmptyUseDefaultValueModel>(@"value: ");
      Assert.AreEqual(999, cfg.Value);
    }
    #endregion
  }
}