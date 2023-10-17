# sqlize
An easy-to-use .NET 6+ library for generating formatted SQL code from an interpolated string.

## Usage
```cs
var id = 123;

var sqlize = new Sqlize();

// Declare entities for use
sqlize.Declare<Test>(out var t);

// Generate formated SQL
var query = sqlize.ToFormattedSql($@"
    SELECT {t.Title}
    FROM {t}
    WHERE {t.Id} = {id};
");

Console.WriteLine(query);

//  SELECT t.Title
//  FROM Test
//  WHERE t.Id = 123; 
```

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
      sqlize.ToFormattedSql($"SELECT {t.Title}", SqlizeFlags.DoubleQuotedName);
      // SELECT t."Title"
      </pre>
    </td>
  </tr>
</table>
