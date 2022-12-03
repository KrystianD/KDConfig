using System;
using JetBrains.Annotations;

namespace KDConfig.Attributes
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

  [PublicAPI]
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
}