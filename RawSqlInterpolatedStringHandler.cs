using System.Runtime.CompilerServices;
using System.Text;

namespace sqlize;

[InterpolatedStringHandler]
public class RawSqlInterpolatedStringHandler
{
    private StringBuilder _stringBuilder;
    private List<SqlizeArgument> _arguments;

    public RawSqlInterpolatedStringHandler(int literalLength, int formattedCount)
    {
        _stringBuilder = new StringBuilder(literalLength);
        _arguments = new List<SqlizeArgument>();
    }

    public void AppendLiteral(string s)
    {
        _stringBuilder.Append(s);
    }

    public void AppendFormatted<T>(T t, [CallerArgumentExpression("t")] string tName = "")
    {
        var prop = tName.Split('.', 2);
        _stringBuilder.Append($"{{{_arguments.Count}}}");
        _arguments.Add(new SqlizeArgument 
        { 
            Name = prop.First(), 
            PropertyPath = prop.Skip(1).FirstOrDefault(), 
            Type = typeof(T), 
            Value = t! 
        });
    }

    public string ToString(Dictionary<string, object> entities, SqlizeFlags flags = SqlizeFlags.None)
    {
        var format = _stringBuilder.ToString();
        var args = GetArgs(entities, flags);
        return string.Format(format, args);
    }

    public FormattableString ToFormattableString(Dictionary<string, object> entities, SqlizeFlags flags = SqlizeFlags.None)
    {
        var format = _stringBuilder.ToString();
        var args = GetArgs(entities, flags);
        return FormattableStringFactory.Create(format, args);
    }

    private object[] GetArgs(Dictionary<string, object> entities, SqlizeFlags flags = SqlizeFlags.None)
    {
        var args = new List<object>();
        foreach (var p in _arguments)
        {
            if (entities.TryGetValue(p.Name, out var entity))
            {
                // If it is a declared entity's property
                if (!string.IsNullOrEmpty(p.PropertyPath))
                {
                    var propertyPath = p.PropertyPath;
                    if (flags.HasFlag(SqlizeFlags.DoubleQuotedName))
                        propertyPath = $"\"{propertyPath}\"";
                    args.Add(string.Join('.', p.Name, propertyPath));
                }
                // If it is a declared entity (table name expected here)
                else
                {
                    var entityName = entity.GetType().Name;
                    if (flags.HasFlag(SqlizeFlags.DoubleQuotedName))
                        entityName = $"\"{entityName}\"";

                    var alias = p.Name;
                    args.Add($"{entityName} AS {alias}");
                }
            }
            else
            {
                // If it is an undeclared argument
                args.Add(p.Value);
            }
        }

        return args.ToArray();
    }
}