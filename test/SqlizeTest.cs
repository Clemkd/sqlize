using FluentAssertions;

namespace sqlize.test;

[TestClass]
public class SqlizeTest
{
    [TestMethod]
    [DynamicData(
        nameof(TestCases.Data),
        typeof(TestCases),
        DynamicDataSourceType.Method,
        DynamicDataDisplayName = nameof(TestCases.GetTestDisplayNames),
        DynamicDataDisplayNameDeclaringType = typeof(TestCases)
    )]
    public void ToFormattableString(string _, Func<Sqlize, RawSqlInterpolatedStringHandler> calculateRawSql, Action<SqlizeOptions>? options, object expectedSql)
    {
        var sqlize = new Sqlize();
        var rawSqlInput = calculateRawSql(sqlize);
        var result = sqlize.ToFormattableString(rawSqlInput, options);

        if (expectedSql is string)
        {
            result.ToString().Trim().Should().BeEquivalentTo(((string)expectedSql).Trim());
        } 
        else
        {
            result.Should().BeEquivalentTo(expectedSql);
        }
    }
}
