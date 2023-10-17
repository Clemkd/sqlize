using System.Runtime.CompilerServices;

namespace sqlize;

public class Sqlize
{
    private readonly Dictionary<string, object> _entities;

    public Sqlize() 
    {
        _entities = new Dictionary<string, object>();
    }

    public void Declare<T>(out T entity, [CallerArgumentExpression("entity")] string name = "") where T : class
    {
        var instanceName = name.Split(' ').Last().Trim();
        entity = Activator.CreateInstance<T>();
        _entities.Add(instanceName, entity);
    }

    public string ToFormattedSql(RawSqlInterpolatedStringHandler sql, SqlizeFlags flags = SqlizeFlags.None)
    {
        return sql.ToString(_entities, flags);
    }

    internal FormattableString ToFormattableString(RawSqlInterpolatedStringHandler sql, SqlizeFlags flags = SqlizeFlags.None)
    {
        return sql.ToFormattableString(_entities, flags);
    }
}
