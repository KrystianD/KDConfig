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
    private class SubModel
    {
      [ConfigValue("value1")]
      public int Value1;

      protected bool Equals(SubModel other)
      {
        return Value1 == other.Value1;
      }

      public override bool Equals(object obj)
      {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SubModel)obj);
      }

      public override int GetHashCode()
      {
        return Value1;
      }
    }

    private class TestComplexArrayModel
    {
      [ConfigValue("array_objs")]
      public SubModel[] Objs;
    }

    [Test]
    public void TestComplexArray()
    {
      var cfg = ParseYaml<TestComplexArrayModel>(@"
array_objs:
  - value1: 999
  - value1: 888
");
      CollectionAssert.AreEqual(
          new[] {
              new SubModel() { Value1 = 999 },
              new SubModel() { Value1 = 888 },
          },
          cfg.Objs);
    }
  }
}