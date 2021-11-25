using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace KDConfig
{
  public class OptionInstance
  {
    public Type FieldType;
    public ConfigValueAttribute Attribute;
    public ConfigPathAttribute? PathAttribute;
    public FieldInfo Field;

    public OptionInstance(Type fieldType, ConfigValueAttribute attribute, ConfigPathAttribute? pathAttribute, FieldInfo field)
    {
      FieldType = fieldType;
      Attribute = attribute;
      PathAttribute = pathAttribute;
      Field = field;
    }

    public bool IsRequired => Attribute.Required == RequiredEnum.Required;
    public bool IsOptional => Attribute.Required == RequiredEnum.Optional;
  }

  public class Error
  {
    public string Path;
    public int Line, Column;
    public string Message;

    public Error(string path, int line, int column, string message)
    {
      Path = path;
      Line = line;
      Column = column;
      Message = message;
    }

    public override string ToString()
    {
      if (Line == -1)
        return $"[{Path}] {Message}";
      else if (Column == -1)
        return $"[{Path}] [line {Line}] {Message}";
      else
        return $"[{Path}] [line {Line} column {Column}] {Message}";
    }
  }

  public class KDConfig
  {
    private static List<OptionInstance> GetClassOptions(Type type)
    {
      var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

      var options = new List<OptionInstance>();

      foreach (var fieldInfo in fields) {
        var attr = fieldInfo.GetCustomAttribute<ConfigValueAttribute>();
        if (attr == null)
          continue;

        var pathAttr = fieldInfo.GetCustomAttribute<ConfigPathAttribute>();

        options.Add(new OptionInstance(fieldInfo.FieldType, attr, pathAttr, fieldInfo));
      }

      foreach (var propertyInfo in properties) {
        var attr = propertyInfo.GetCustomAttribute<ConfigValueAttribute>();
        if (attr == null)
          continue;

        var pathAttr = propertyInfo.GetCustomAttribute<ConfigPathAttribute>();

        var backingField = type.GetField($"<{propertyInfo.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
        if (backingField == null) {
          throw new Exception("No backing field"); // not autogenerated property
        }

        options.Add(new OptionInstance(backingField.FieldType, attr, pathAttr, backingField));
      }

      return options;
    }

    public static T CreateFrom<T>(IConfigDataProvider provider) where T : new()
    {
      var errors = new List<Error>();
      var instance = CreateClassFromProvider("", typeof(T), provider, errors);
      return (T)instance;
    }

    private static object CreateClassFromProvider(string basePath, Type type, IConfigDataProvider provider, List<Error> errors)
    {
      var instance = Activator.CreateInstance(type)!;
      var options = GetClassOptions(type);

      foreach (var option in options) {
        var path = option.Attribute.Path;
        var fieldType = option.FieldType;

        if (basePath != "")
          path = basePath + "." + path;

        NodeValue? node = null;
        try {
          if (ConversionUtils.IsScalarType(fieldType)) {
            var scalarNodeValue = provider.GetScalar(path);
            node = scalarNodeValue;

            ProcessSimpleOption(option, instance, scalarNodeValue, provider, errors);
          }
          else if (fieldType.IsArray && ConversionUtils.IsScalarType(fieldType.GetElementType())) {
            var arrayNodeValue = provider.GetScalar(path);
            node = arrayNodeValue;

            ProcessSimpleArrayOption(option, instance, arrayNodeValue, provider, errors);
          }
          else {
            var value = CreateClassFromProvider(path, fieldType, provider, errors);
            option.Field.SetValue(instance, value);
          }
        }
        catch (InternalConfigException e) {
          errors.Add(new Error(path, node?.Line ?? -1, node?.Column ?? -1, e.Message));
        }
      }

      if (basePath == "" && errors.Count > 0) {
        throw new ConfigException(errors);
      }

      return instance;
    }

    private static void ProcessSimpleOption(OptionInstance option, object instance, NodeValue? scalarNode, IConfigDataProvider provider, List<Error> errors)
    {
      var scalarValue = scalarNode?.Value;

      if (scalarValue is "") {
        switch (option.Attribute.EmptyHandling) {
          case EmptyHandling.NotAllowed:
            throw new InternalConfigException("empty value not allowed");
          case EmptyHandling.AsIs:
            break;
          case EmptyHandling.AsNull:
            if (option.IsRequired) {
              throw new InternalConfigException("required option is not present");
            }
            else {
              if (option.FieldType.IsNullable())
                option.Field.SetValue(instance, null);
              else
                throw new InternalConfigException("unable to assign null to not nullable value");
            }

            return;
          case EmptyHandling.UseDefaultValue:
            return;
          default:
            throw new InternalConfigException("invalid EmptyHandling value");
        }
      }

      if (scalarValue is null) {
        if (option.IsRequired)
          throw new InternalConfigException("required option is not present");
      }
      else {
        if (provider.IsFixedType) {
          throw new NotImplementedException();
        }
        else {
          if (scalarValue is string scalarValueString) {
            var value = ConversionUtils.ParseStringToType(scalarValueString, option.FieldType);
            PostprocessValue(provider, option, ref value);
            option.Field.SetValue(instance, value);
          }
          else {
            throw new InternalConfigException("invalid conversion");
          }
        }
      }
    }

    private static void ProcessSimpleArrayOption(OptionInstance option, object instance, NodeValue? scalarNode, IConfigDataProvider provider, List<Error> errors)
    {
      var scalarValue = scalarNode?.Value;

      if (scalarValue is "") {
        switch (option.Attribute.EmptyHandling) {
          case EmptyHandling.NotAllowed:
            throw new InternalConfigException("empty value not allowed");
          case EmptyHandling.AsIs:
            scalarValue = Array.Empty<string>();
            break;
          case EmptyHandling.AsNull:
            if (option.IsRequired) {
              throw new InternalConfigException("required option is not present");
            }
            else {
              option.Field.SetValue(instance, null);
            }

            return;
          case EmptyHandling.UseDefaultValue:
            return;
          default:
            throw new InternalConfigException("invalid EmptyHandling value");
        }
      }

      if (scalarValue is null) {
        if (option.IsRequired)
          throw new InternalConfigException("required option is not present");
      }
      else {
        if (provider.IsFixedType) {
          throw new NotImplementedException();
        }
        else {
          if (scalarValue is string[] scalarValueStringArray) {
            var itemType = option.FieldType.GetElementType();

            IList array = Array.CreateInstance(itemType, scalarValueStringArray.Length);
            for (var i = 0; i < scalarValueStringArray.Length; i++) {
              array[i] = ConversionUtils.ParseStringToType(scalarValueStringArray[i], itemType);
            }

            option.Field.SetValue(instance, array);
          }
          else {
            throw new InternalConfigException("invalid conversion");
          }
        }
      }
    }

    private static void PostprocessValue(IConfigDataProvider provider, OptionInstance option, ref object value)
    {
      if (value is string valueStr && option.PathAttribute is not null) {
        switch (option.PathAttribute.PathRelativeTo) {
          case PathRelativeTo.ConfigFile:
            if (provider.Directory is null)
              throw new InternalConfigException("can't use paths relative to config file with ephemeral config file");
            valueStr = Path.Combine(provider.Directory, valueStr);
            break;
          case PathRelativeTo.WorkingDirectory:
            valueStr = Path.Combine(Environment.CurrentDirectory, valueStr);
            break;
          default: throw new ArgumentOutOfRangeException();
        }

        if (option.PathAttribute.MustExists && !(File.Exists(valueStr) || Directory.Exists(valueStr)))
          throw new InternalConfigException("path doesn't exist");

        if (option.PathAttribute.PathType != PathType.FileOrDirectory) {
          if (Directory.Exists(valueStr) && option.PathAttribute.PathType == PathType.File)
            throw new InternalConfigException("file required, but directory passed");
          if (File.Exists(valueStr) && option.PathAttribute.PathType == PathType.Directory)
            throw new InternalConfigException("directory required, but file passed");
        }

        value = valueStr;
      }
    }
  }

  public class InternalConfigException : Exception
  {
    public InternalConfigException(string message) : base(message) { }
  }

  public class ConfigException : Exception
  {
    public List<Error> Errors { get; }

    public ConfigException(List<Error> errors)
    {
      Errors = errors;
    }

    public override string ToString() => string.Join("\n", Errors.Select(x => x.ToString()));
  }
}