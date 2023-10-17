namespace sqlize;
internal struct SqlizeArgument
{
    public string Name { get; internal set; }
    public string? PropertyPath { get; internal set; }
    public Type Type { get; internal set; }
    public object Value { get; internal set; }
}