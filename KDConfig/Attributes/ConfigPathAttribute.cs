using System;
using JetBrains.Annotations;

namespace KDConfig.Attributes
{
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

  [PublicAPI]
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