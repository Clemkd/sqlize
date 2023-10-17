# sqlize
![workflow](https://github.com/clemkd/sqlize/actions/workflows/dotnet.yml/badge.svg)

An easy-to-use .NET 6+ library for generating formatted SQL code from an interpolated string.

## Usage
```cs
class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```
```cs
var id = 123;

var sqlize = new Sqlize();

// Declare entities for use
sqlize.Declare<Blog>(out var b);

// Generate formated SQL
var query = sqlize.ToFormattedSql($@"
    SELECT {b.Name}
    FROM {b}
    WHERE {b.Id} = {id};
");

Console.WriteLine(query);

//  SELECT b.Name
//  FROM Blog AS b
//  WHERE b.Id = 123; 
```

## SQL Injection
You can use the `ToFormattableString` function instead of `ToFormattedSql` to obtain a `FormattableString` and thus use the SQL injection-proof methods of your favorite ORMs.

```cs
FormattableString query = sqlize.ToFormattableString($@"SELECT {b.Name} FROM {b} WHERE {b.Id} = {id};");
```

## Options
This first version offers few options for managing the specific features of dbms and users.

<table>
  <tr>
    <td>Option</td>
    <td>Description</td>
    <td>Example</td>
  </tr>
  <tr>
    <td><pre lang="C#">SqlizeFlags.DoubleQuotedName<pre></td>
    <td>Encloses column and table names in double quotes</td>
    <td>
      <pre lang="c#"> 
      sqlize.ToFormattedSql($"SELECT {b.Name}", SqlizeFlags.DoubleQuotedName);
      // SELECT b."Name"
      </pre>
    </td>
  </tr>
</table>
