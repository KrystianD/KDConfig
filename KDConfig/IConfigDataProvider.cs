namespace KDConfig
{
  public class NodeValue
  {
    public object Value;
    public int Line, Column;

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
    NodeValue GetScalar(string dotPath);
  }
}