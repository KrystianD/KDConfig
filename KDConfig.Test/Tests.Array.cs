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
    private class TestArrayModel
    {
      [ConfigValue("array_ints")]
      public int[] Ints;
    }

    [Test]
    public void TestArray()
    {
      var cfg = ParseYaml<TestArrayModel>(@"
array_ints:
- 1
- 2
");
      Assert.AreEqual(new[] { 1, 2 }, cfg.Ints);
    }

    private class TestArrayEmptyAsIsModel
    {
      [ConfigValue("array_ints", RequiredEnum.Optional, EmptyHandling.AsIs)]
      public int[] Ints;
    }

    [Test]
    public void TestArrayEmptyAsIs()
    {
      var cfg = ParseYaml<TestArrayEmptyAsIsModel>(@"array_ints: ");
      Assert.AreEqual(new int[] { }, cfg.Ints);
    }

    private class TestArrayEmptyAsNullModel
    {
      [ConfigValue("array_ints", RequiredEnum.Optional, EmptyHandling.AsNull)]
      public int[] Ints;
    }

    [Test]
    public void TestArrayEmptyAsNull()
    {
      var cfg = ParseYaml<TestArrayEmptyAsNullModel>(@"array_ints: ");
      Assert.IsNull(cfg.Ints);
    }

    private class TestArrayEmptyNotAllowedModel
    {
      [ConfigValue("array_ints", RequiredEnum.Optional, EmptyHandling.NotAllowed)]
      public int[] Ints;
    }

    [Test]
    public void TestArrayEmptyNotAllowed()
    {
      Assert.Throws<ConfigException>(() => {
        ParseYaml<TestArrayEmptyNotAllowedModel>(@"array_ints: ");
      });
    }

    private class TestArrayEmptyUseDefaultValueModel
    {
      [ConfigValue("array_ints", RequiredEnum.Optional, EmptyHandling.UseDefaultValue)]
      public int[] Ints = { 1, 2, 3 };
    }

    [Test]
    public void TestArrayEmptyUseDefaultValue()
    {
      var cfg = ParseYaml<TestArrayEmptyUseDefaultValueModel>(@"array_ints: ");
      Assert.AreEqual(new[] { 1, 2, 3 }, cfg.Ints);
    }
  }
}