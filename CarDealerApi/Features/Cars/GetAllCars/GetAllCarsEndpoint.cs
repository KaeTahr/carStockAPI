using FastEndpoints;
using Dapper;

using Data;

public class GetAllCarsEndpoint : EndpointWithoutRequest<List<CarResponse>>
{
    private readonly DbConnectionFactory _factory;

    public override void Configure()
    {
        Verbs(Http.GET);
        Routes("/cars");
        AllowAnonymous();
        Description(x => x.WithName("Get All Cars")
            .WithSummary("Retrieves a list of all cars in the inventory")
            .WithTags("Cars"));
    }
    public GetAllCarsEndpoint(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using var connection = _factory.CreateConnection();
        var cars = await connection.QueryAsync<CarResponse>("SELECT * FROM Cars");
        await Send.OkAsync(cars.ToList(), ct);
    }
}