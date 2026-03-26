using System.Data;
using System.Data.Common;
using Dapper;

namespace Data;

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
        await SeedAsync(connection);
    }

    private async Task SeedAsync(IDbConnection connection)
    {
        var dealersCount = await connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM Dealers");
        if (dealersCount > 0) return; // do not work over existing data

        // seed dealers
        var dealers = new[]
        {
            new { Name = "Melbourne Auto Group", Username="melbourne", Password="dealer123" },
            new { Name = "Sydney Car World", Username="sydney", Password="dealer123"}
        };

        foreach (var dealer in dealers)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(dealer.Password);
            var dealerId = await connection.QuerySingleAsync<int>(@"
                INSERT INTO Dealers (Name, Username, PasswordHash)
                VALUES (@Name, @Username, @PasswordHash);
                SELECT last_insert_rowid();",
                new { dealer.Name, dealer.Username, PasswordHash = hash });
            
            // seed cars for each dealer
            if (dealer.Username == "melbourne")
            {
                await connection.ExecuteAsync(@"
                    INSERT INTO Cars (Make, Model, Year, Price, Stock, DealerId)
                    VALUES 
                    ('Toyota', 'Corolla', 2021, 24990, 5, @Id),
                    ('Toyota', 'Rav4', 2022, 38990, 3, @Id),
                    ('Honda', 'CX-5', 2021, 35990, 4, @Id),
                    ('Hyundai', 'Civic', 2020, 33990, 2, @Id);",
                    new { Id = dealerId });
            }
            else if (dealer.Username == "sydney")
            {
                await connection.ExecuteAsync(@"
                    INSERT INTO Cars (Make, Model, Year, Price, Stock, DealerId)
                    VALUES 
                    ('BMW', '3 Series', 2021, 65990, 2, @Id),
                    ('Audi', 'A4', 2022, 72990, 3, @Id),
                    ('Mercedes', 'C-Class', 2021, 78990, 1, @Id),
                    ('Volkswagen', 'Golf', 2020, 32990, 7, @Id),
                    ('Porsche', 'Macan', 2022, 95990, 2, @Id);",
                    new { Id = dealerId });
            }
        }
    }
}