using System;

namespace KDConfig
{
  public enum RequiredEnum
  {
    Required,
    Optional,
  }

  public enum EmptyHandling
  {
    NotAllowed,
    AsIs,
    AsNull,
    UseDefaultValue,
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class ConfigValueAttribute : Attribute
  {
    public string Path { get; }
    public RequiredEnum Required { get; }
    public EmptyHandling EmptyHandling { get; }

    public ConfigValueAttribute(string path, RequiredEnum required = RequiredEnum.Required, EmptyHandling emptyHandling = EmptyHandling.NotAllowed)
    {
      Path = path;
      Required = required;
      EmptyHandling = emptyHandling;
    }
  }

  public enum PathRelativeTo
  {
    ConfigFile,
    WorkingDirectory,
  }

  public enum PathType
  {
    FileOrDirectory,
    File,
    Directory,
  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class ConfigPathAttribute : Attribute
  {
    public PathType PathType { get; }
    public PathRelativeTo PathRelativeTo { get; }
    public bool MustExists { get; }

    public ConfigPathAttribute(PathType pathType, PathRelativeTo pathRelativeTo = PathRelativeTo.ConfigFile, bool mustExists = true)
    {
      PathType = pathType;
      PathRelativeTo = pathRelativeTo;
      MustExists = mustExists;
    }
  }
}