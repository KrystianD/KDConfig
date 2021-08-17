using System;

namespace KDConfig
{
  public static class ConversionUtils
  {
    public static bool IsNullable(this Type type)
    {
      return type == typeof(string) || IsGenericNullable(type);
    }

    public static bool IsGenericNullable(this Type type)
    {
      return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    public static Type GetNullableInnerType(this Type type)
    {
      if (!IsNullable(type))
        throw new Exception("Type is not nullable");

      return type.GenericTypeArguments[0];
    }

    public static bool IsScalarType(Type type)
    {
      return type == typeof(bool) ||
             type == typeof(short) ||
             type == typeof(ushort) ||
             type == typeof(int) ||
             type == typeof(uint) ||
             type == typeof(long) ||
             type == typeof(ulong) ||
             type == typeof(decimal) ||
             type == typeof(string) ||
             (IsGenericNullable(type) && IsScalarType(GetNullableInnerType(type)));
    }

    public static object ParseStringToType(string value, Type targetType)
    {
      try {
        if (targetType == typeof(bool)) {
          switch (value.ToLower()) {
            case "y":
            case "yes":
            case "on":
            case "true":
              return true;
            case "n":
            case "no":
            case "off":
            case "false":
              return false;
            default:
              throw new FormatException("invalid boolean value");
          }
        }

        if (targetType == typeof(short)) return short.Parse(value);
        if (targetType == typeof(ushort)) return ushort.Parse(value);
        if (targetType == typeof(int)) return int.Parse(value);
        if (targetType == typeof(uint)) return uint.Parse(value);
        if (targetType == typeof(long)) return long.Parse(value);
        if (targetType == typeof(ulong)) return ulong.Parse(value);
        if (targetType == typeof(decimal)) return decimal.Parse(value);

        if (targetType == typeof(string)) return value;

        throw new FormatException("unsupported type");
      }
      catch (FormatException) {
        throw new InternalConfigException($"invalid value, got: /{value}/, expected {targetType}");
      }
    }
  }
}