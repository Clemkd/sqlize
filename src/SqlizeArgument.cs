namespace sqlize;
public struct SqlizeArgument
{
    public string Key { get; internal set; }
    public string Name { get; internal set; }
    public string? PropertyPath { get; internal set; }
    public Type Type { get; internal set; }
    public object Value { get; internal set; }
    public bool IsTable { get; internal set; }
}