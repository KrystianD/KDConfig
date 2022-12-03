namespace KDConfig
{
  public class NodeValue
  {
    public readonly object Value;
    public readonly int Line, Column;

    public NodeValue(object value)
    {
      Value = value;
    }

    public NodeValue(object value, int line, int column)
    {
      Value = value;
      Line = line;
      Column = column;
    }
  }

  public interface IConfigDataProvider
  {
    bool IsFixedType { get; }
    NodeValue? GetScalar(string dotPath);
    bool TryGetScalar(string dotPath, out NodeValue? value);

    int GetArrayLength(string dotPath);

    string? Directory { get; }
  }
}