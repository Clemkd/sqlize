
namespace sqlize;

public class SqlizeOptions
{
    public Func<string, string> PropertyNameFormat { get; set; } = (string name) => name;
    public Func<string, string> TableNameFormat { get; set; } = (string name) => name;
    public bool IsAliasEnabled { get; set; } = true;
}
