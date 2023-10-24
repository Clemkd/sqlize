using System.Reflection;
using System.Runtime.CompilerServices;

namespace sqlize.test;

public class TestCases
{
    public static IEnumerable<object[]> Data()
    {
        yield return new object[] 
        { 
            "Get all posts by blog ordered by sender and creation date",
            (Sqlize sqlize) => {
                sqlize.Declare<Blog>(out var b);
                sqlize.Declare<Post>(out var p);
                return (RawSqlInterpolatedStringHandler)$@"
                    SELECT {b.Name}, {p.Sender}, {p.CreatedAt}, {p.Content}
                    FROM {b}
                    LEFT JOIN {p} ON {p.BlogId} = {b.Id}
                    ORDER BY {b.Name}, {p.Sender}, {p.CreatedAt};
                ";
            },
            SqlizeFlags.DoubleQuotedName,
            @"
                    SELECT b.""Name"", p.""Sender"", p.""CreatedAt"", p.""Content""
                    FROM ""Blog"" AS b
                    LEFT JOIN ""Post"" AS p ON p.""BlogId"" = b.""Id""
                    ORDER BY b.""Name"", p.""Sender"", p.""CreatedAt"";
            "
        };
        yield return new object[]
{
            "Delete posts from blog named 'A' and created before the january 1th, 2023",
            (Sqlize sqlize) => {
                sqlize.Declare<Blog>(out var blog);
                sqlize.Declare<Post>(out var post);
                return (RawSqlInterpolatedStringHandler)$@"
                    DELETE {sqlize.Alias(post)}
                    FROM {post}
                    INNER JOIN {blog} ON {blog.Id} = {post.BlogId}
                    WHERE {blog.Name} = 'A' AND {post.CreatedAt} < '2023-01-01';
                ";
            },
            SqlizeFlags.DoubleQuotedName,
            @"
                    DELETE post
                    FROM ""Post"" AS post
                    INNER JOIN ""Blog"" AS blog ON blog.""Id"" = post.""BlogId""
                    WHERE blog.""Name"" = 'A' AND post.""CreatedAt"" < '2023-01-01';
            "
        };
        yield return new object[]
{
            "Alias using another variable",
            (Sqlize sqlize) => {
                sqlize.Declare<Post>(out var post);
                var abc = post;
                return (RawSqlInterpolatedStringHandler)$@"{sqlize.Alias(abc)}";
            },
            SqlizeFlags.DoubleQuotedName,
            @"post"
        };
        yield return new object[]
        {
            "Get all posts where sender name is in a specified array",
            (Sqlize sqlize) => {
                sqlize.Declare<Post>(out var p);
                return (RawSqlInterpolatedStringHandler)$@"
                    SELECT {sqlize.Alias(p)}.*
                    FROM {p}
                    WHERE {p.Sender} IN {new string[] { "Marc", "Ben" }};";
            },
            SqlizeFlags.DoubleQuotedName,
            FormattableStringFactory.Create(@"
                    SELECT {0}.*
                    FROM {1}
                    WHERE {2} IN {3};", 
            "p", @"""Post"" AS p", @"p.""Sender""", new string[] { "Marc", "Ben" })
        };
    }

    public static string GetTestDisplayNames(MethodInfo methodInfo, object[] values) => (string)values[0];
}

public class Blog
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<Post> Posts { get; set; }
}

public class Post
{
    public Guid Id { get; set; }
    public Guid BlogId { get; set; }
    public string Sender { get; set; }
    public string Content { get; set; }
    public DateTimeOffset CreatedAt {  get; set; }
}