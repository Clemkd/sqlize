using System;
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
        var path = prop.Skip(1).FirstOrDefault();
        var key = $"{{{_arguments.Count}}}";
        _stringBuilder.Append(key);
        _arguments.Add(new SqlizeArgument 
        { 
            Key = key,
            Name = prop.First(), 
            PropertyPath = path, 
            Type = typeof(T), 
            Value = t!,
            IsTable = string.IsNullOrEmpty(path)
        });
    }

    public override string ToString()
    {
        return _stringBuilder.ToString();
    }

    public IReadOnlyList<SqlizeArgument> Args => _arguments;
}