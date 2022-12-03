using System;
using System.Collections;
using System.Collections.Generic;

namespace KDConfig
{
  public static class ReflectionExtensions
  {
    public static bool IsGenericList(this Type type)
    {
      return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }
  }

  public static class ReflectionUtils
  {
    public static IList CreateListInstance(Type itemType)
    {
      return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType))!;
    }
  }
}