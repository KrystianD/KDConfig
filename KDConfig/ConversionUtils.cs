using System;

namespace KDConfig
{
  public static class ConversionUtils
  {
    public static bool IsNullable(this Type type)
    {
      return 
          type == typeof(string) ||
          (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
    }

    public static Type GetNullableInnerType(this Type type)
    {
      if (!IsNullable(type))
        throw new Exception("Type is not nullable");

      return type.GenericTypeArguments[0];
    }

    public static bool IsScalarType(Type type)
    {
      return type == typeof(int) ||
             type == typeof(string) ||
             IsScalarType(GetNullableInnerType(type));
    }

    public static object ParseStringToType(string value, Type targetType)
    {
      if (targetType == typeof(int)) {
        return int.Parse(value);
      }

      if (targetType == typeof(string)) {
        return value;
      }

      throw new Exception("invalid type");
    }
  }
}