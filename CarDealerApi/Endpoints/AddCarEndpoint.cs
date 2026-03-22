using Dapper;
using FastEndpoints;
using Models;
using System.Data;
using System.Runtime.InteropServices.Swift;

namespace Ednpoints
{
    public class AddCarRequest
    {
        public int DealerId { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Stock { get; set; }
    }

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
}