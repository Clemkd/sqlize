using System.Runtime.CompilerServices;
using System.Text;

namespace sqlize;

public class Sqlize
{
    protected readonly Dictionary<string, object> _entities;
    protected readonly Dictionary<int, string> _alias;

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

    public virtual FormattableString ToFormattableString(RawSqlInterpolatedStringHandler sql, Action<SqlizeOptions>? options)
    {
        var query = sql.ToString();
        var queryArgs = sql.Args;
        var opts = new SqlizeOptions();
        options?.Invoke(opts);
        var formatArgs = GetArgs(_entities, queryArgs, opts);

        return FormattableStringFactory.Create(query, formatArgs.Values.ToArray());
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

    protected string GetEntityInstanceName(string name) => name.Split(' ').Last().Trim();

    protected virtual Dictionary<string, string> GetArgs(Dictionary<string, object> entities, 
        IReadOnlyList<SqlizeArgument> arguments, SqlizeOptions options)
    {
        var args = new Dictionary<string, string>();
        foreach (var p in arguments)
        {
            if (entities.TryGetValue(p.Name, out var entity))
            {
                // If it is a declared entity's property
                if (!string.IsNullOrEmpty(p.PropertyPath))
                {
                    args.Add(p.Key, GetPropertyName(p, options));
                }
                // If it is a declared entity (table name expected here)
                else
                {
                    args.Add(p.Key, GetTableName(p, entity, options));
                }
            }
            else
            {
                // If it is an undeclared argument
                args.Add(p.Key, p.Value.ToString());
            }
        }

        return args;
    }

    protected virtual string GetTableName(SqlizeArgument arg, object obj, SqlizeOptions options)
    {
        var entityName = options.TableNameFormat(obj.GetType().Name);
        return $"{entityName}{(options.IsAliasEnabled ? $" AS {arg.Name}" : string.Empty)}";
    }

    protected virtual string GetPropertyName(SqlizeArgument arg, SqlizeOptions options)
    {
        var propertyPath = options.PropertyNameFormat(arg.PropertyPath);
        return string.Join('.', arg.Name, propertyPath);
    }
}
