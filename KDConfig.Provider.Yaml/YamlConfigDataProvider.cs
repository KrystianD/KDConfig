using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using YamlDotNet.RepresentationModel;

namespace KDConfig.Provider.Yaml
{
  public class YamlConfigDataProvider : IConfigDataProvider
  {
    public bool IsFixedType => false;

    private readonly YamlMappingNode? _node;

    public string? Directory { get; }

    public YamlConfigDataProvider(YamlMappingNode? node, string? configDirectory)
    {
      _node = node;
      Directory = configDirectory;
    }

    public NodeValue? GetScalar(string dotPath)
    {
      var node = GetConfigNode(dotPath);

      switch (node) {
        case YamlScalarNode scalarNode:
          return new NodeValue(scalarNode.Value!, scalarNode.Start.Line, scalarNode.Start.Column);
        case YamlSequenceNode sequenceNode:
          return new NodeValue(sequenceNode.Children.Select(x => ((YamlScalarNode)x).Value!).ToArray(),
                               sequenceNode.Start.Line,
                               sequenceNode.Start.Column);
        case null:
          return null;
        default:
          throw new Exception("scalar expected");
      }
    }

    public bool TryGetScalar(string dotPath, out NodeValue? value)
    {
      var node = GetConfigNode(dotPath);

      switch (node) {
        case YamlScalarNode scalarNode:
          value = new NodeValue(scalarNode.Value!, scalarNode.Start.Line, scalarNode.Start.Column);
          return true;
        case YamlSequenceNode sequenceNode:
          value = new NodeValue(sequenceNode.Children.Select(x => ((YamlScalarNode)x).Value!).ToArray(),
                                sequenceNode.Start.Line,
                                sequenceNode.Start.Column);
          return true;
        default:
          value = null;
          return false;
      }
    }

    public int GetArrayLength(string dotPath)
    {
      var node = GetConfigNode(dotPath);

      switch (node) {
        case YamlSequenceNode sequenceNode:
          return sequenceNode.Children.Count;
        default: throw new Exception("sequence node expected");
      }
    }

    public YamlNode? GetConfigNode(string dotPath)
    {
      if (_node == null)
        return null;

      var curNode = (YamlNode)_node;
      foreach (var p in dotPath.Split('.')) {
        if (curNode is YamlMappingNode n) {
          if (!n.Children.TryGetValue(p, out curNode))
            return null;
        }
        else if (curNode is YamlSequenceNode s) {
          var m = Regex.Match(p, @"\[(\d+)\]");
          if (m.Success) {
            int idx = int.Parse(m.Groups[1].Value);
            curNode = s.Children[idx];
          }
          else {
            throw new Exception($"Invalid array index /{dotPath}/, part /{p}/");
          }
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

    public static YamlConfigDataProvider FromYamlFile(string path)
    {
      var content = File.ReadAllText(path);
      var node = CreateYamlMappingNodeFromString(content);
      return new YamlConfigDataProvider(node, Path.GetDirectoryName(Path.GetFullPath(path)));
    }

    private static YamlMappingNode? CreateYamlMappingNodeFromString(string yamlStr)
    {
      var yaml = new YamlStream();
      using (var reader = new StringReader(yamlStr)) {
        yaml.Load(reader);
        if (yaml.Documents.Count == 0)
          return null;
        return (YamlMappingNode)yaml.Documents[0].RootNode;
      }
    }
  }
}