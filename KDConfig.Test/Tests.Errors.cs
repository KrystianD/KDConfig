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
    private class TestErrorModel
    {
      [ConfigValue("section.value_int")]
      public int ValueInt;
    }

    [Test]
    public void TestError()
    {
      Assert.Throws<ConfigException>(() => {
        ParseYaml<TestErrorModel>(@"
section:
  value_int: a
");
      });
    }
  }
}