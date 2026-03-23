using FastEndpoints;
using Dapper;
using Data;

public class UpdateCar : Endpoint<UpdateCarRequest, CarResponse>
{
    private readonly DbConnectionFactory _factory;

    public UpdateCar(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public override void Configure()
    {
        Verbs(Http.PUT);
        Routes("/cars/{id}");
        AllowAnonymous();
        Description(x => x.WithName("Update Car")
            .WithSummary("Updates details of an existing car in the inventory")
            .WithTags("Cars"));
    }

    public override async Task HandleAsync(UpdateCarRequest req, CancellationToken ct)
    {
        using var connection = _factory.CreateConnection();
        var existingCar = await connection.QueryFirstOrDefaultAsync<CarResponse>("SELECT * FROM Cars WHERE Id = @Id", new { Id = req.Id });
        if (existingCar == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var sql = "UPDATE Cars SET DealerId = @DealerId, Make = @Make, Model = @Model, Year = @Year, Stock = @Stock WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { req.DealerId, req.Make, req.Model, req.Year, req.Stock, req.Id });

        var updatedCar = await connection.QueryFirstOrDefaultAsync<CarResponse>("SELECT * FROM Cars WHERE Id = @Id", new { Id = req.Id });
        if (updatedCar == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        await Send.OkAsync(updatedCar, ct);
    }
}