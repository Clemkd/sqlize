# sqlize
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
var query = sqlize.ToFormattedString($@"
    SELECT {b.Name}
    FROM {b}
    WHERE {b.Id} = {id};
");

Console.WriteLine(query);

//  SELECT b.Name
//  FROM Blog AS b
//  WHERE b.Id = 123; 
```