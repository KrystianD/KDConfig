using System;
using System.IO;
using YamlDotNet.RepresentationModel;

namespace KDConfig.Provider.Yaml
{
  public class YamlConfigDataProvider : IConfigDataProvider
  {
    public bool IsFixedType => false;

    private readonly YamlMappingNode _node;
    private readonly string _configDirectory;

    public YamlConfigDataProvider(YamlMappingNode node, string configDirectory)
    {
      _node = node;
      _configDirectory = configDirectory;
    }

    public object GetScalar(string dotPath)
    {
      var node = GetConfigNode(dotPath);

      return node switch {
          YamlScalarNode scalarNode => scalarNode.Value,
          null => null,
          _ => throw new Exception("scalar expected"),
      };
    }

    private YamlNode GetConfigNode(string dotPath)
    {
      if (_node == null)
        return null;

      var curNode = (YamlNode)_node;
      foreach (var p in dotPath.Split('.')) {
        if (curNode is YamlMappingNode n) {
          if (!n.Children.TryGetValue(p, out curNode))
            return null;
        }
        else {
          throw new Exception($"Unknown node path /{dotPath}/");
        }
      }

      return curNode;
    }

    public static YamlConfigDataProvider FromYamlString(string yamlString)
    {
      var node = CreateYamlMappingNodeFromString(yamlString);
      return new YamlConfigDataProvider(node, null);
    }

    private static YamlMappingNode CreateYamlMappingNodeFromString(string yamlStr)
    {
      var yaml = new YamlStream();
      using var reader = new StringReader(yamlStr);
      yaml.Load(reader);
      if (yaml.Documents.Count == 0)
        return null;
      return (YamlMappingNode)yaml.Documents[0].RootNode;
    }
  }
}