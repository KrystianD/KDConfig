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
    private class TestTypesModel
    {
      [ConfigValue("section.value_bool")]
      public bool ValueBool;

      [ConfigValue("section.value_short")]
      public short ValueShort;

      [ConfigValue("section.value_ushort")]
      public ushort ValueUShort;

      [ConfigValue("section.value_int")]
      public int ValueInt;

      [ConfigValue("section.value_uint")]
      public uint ValueUInt;

      [ConfigValue("section.value_long")]
      public long ValueLong { get; }

      [ConfigValue("section.value_ulong")]
      public ulong ValueULong { get; }

      [ConfigValue("section.value_dec")]
      public decimal ValueDecimal { get; }

      [ConfigValue("section.value_str")]
      public string ValueStr { get; }
    }

    [Test]
    public void TestTypes()
    {
      var cfg = ParseYaml<TestTypesModel>(@"
section:
  value_bool: true
  value_short: -123
  value_ushort: 123
  value_int: -1234567
  value_uint: 1234567
  value_long: -123456789123456789
  value_ulong: 123456789123456789
  value_dec: 12345678.1234
  value_str: abc
");
      Assert.AreEqual(true, cfg.ValueBool);
      Assert.AreEqual(-123, cfg.ValueShort);
      Assert.AreEqual(123, cfg.ValueUShort);
      Assert.AreEqual(-1234567, cfg.ValueInt);
      Assert.AreEqual(1234567, cfg.ValueUInt);
      Assert.AreEqual(-123456789123456789L, cfg.ValueLong);
      Assert.AreEqual(123456789123456789L, cfg.ValueULong);
      Assert.AreEqual(12345678.1234M, cfg.ValueDecimal);
      Assert.AreEqual("abc", cfg.ValueStr);
    }
  }
}