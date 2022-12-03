namespace KDConfig
{
  public interface IConfigDataProvider
  {
    bool IsFixedType { get; }
    object GetScalar(string dotPath);
  }
}