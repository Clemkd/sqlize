using System.Runtime.CompilerServices;

namespace sqlize;

public class Sqlize
{
    private readonly Dictionary<string, object> _entities;
    private readonly Dictionary<int, string> _alias;

    public Sqlize() 
    {
        _entities = new Dictionary<string, object>();
        _alias = new Dictionary<int, string>();
    }

    public void Declare<T>(out T entity, [CallerArgumentExpression("entity")] string name = "") where T : class
    {
        var instanceName = GetEntityInstanceName(name);
        entity = Activator.CreateInstance<T>();
        _entities.Add(instanceName, entity);
        _alias.Add(entity.GetHashCode(), instanceName);
    }

    public string ToFormattedSql(RawSqlInterpolatedStringHandler sql, SqlizeFlags flags = SqlizeFlags.None)
    {
        return sql.ToString(_entities, flags);
    }

    public FormattableString ToFormattableString(RawSqlInterpolatedStringHandler sql, SqlizeFlags flags = SqlizeFlags.None)
    {
        return sql.ToFormattableString(_entities, flags);
    }

    /// <summary>
    /// Get the unique alias of the declared entity
    /// </summary>
    /// <typeparam name="T">The type of the declared entity</typeparam>
    /// <param name="declaredEntity">The declared entity</param>
    /// <returns>The alias of the declared entity</returns>
    public string Alias<T>(T declaredEntity)
    {
        if (declaredEntity == null) throw new ArgumentNullException();
        return _alias[declaredEntity.GetHashCode()];
    }

    private string GetEntityInstanceName(string name) => name.Split(' ').Last().Trim();
}
