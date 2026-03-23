using Dapper;
using FastEndpoints;
using Models;
using System.Data;
using System.Runtime.InteropServices.Swift;

public class AddCarEndpoint : Endpoint<AddCarRequest>
{
    private readonly DbConnectionFactory _dbConnectionFactory;

    public AddCarEndpoint(DbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override void Configure()
    {
        Post("/cars/add");
        AllowAnonymous(); // TODO: add JWT
        Description(x => x.WithName("Add Car")
            .WithSummary("Adds a new car to the inventory")
            .WithTags("Cars"));
    }

    public override async Task HandleAsync(AddCarRequest req, CancellationToken ct)
    {
        using IDbConnection connection = _dbConnectionFactory.CreateConnection();

        string sql = @"
            INSERT INTO Cars (DealerId, Make, Model, Year, Stock)
            VALUES (@DealerId, @Make, @Model, @Year, @Stock);
        ";

        await connection.ExecuteAsync(sql, req);
        await Send.OkAsync(new { message = "Car added successfully!" }, ct);
    }
}