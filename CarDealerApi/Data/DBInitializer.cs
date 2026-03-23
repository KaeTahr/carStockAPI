using System.Data.Common;

namespace Data;
using Dapper;

public class DbInitializer
{
    private readonly DbConnectionFactory _factory;

    public DbInitializer(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        using var connection = _factory.CreateConnection();
        var sql = await File.ReadAllTextAsync("Data/init.sql");
        await connection.ExecuteAsync(sql);
    }
}