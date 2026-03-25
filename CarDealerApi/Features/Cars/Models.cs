namespace Features.Cars;

public class AddCarRequest
{
    public string Make {get; set; } = string.Empty;
    public string Model {get; set; } = string.Empty;
    public int Year {get; set; }
    public decimal Price {get; set; }
    public int Stock {get; set; }
}

public class UpdateStockRequest
{
    public int Stock {get; set; }
}

public class CarResponse
{
    public int Id {get; set; }
    public string Make {get; set; } = string.Empty;
    public string Model {get; set; } = string.Empty;
    public int Year {get; set; }
    public decimal Price {get; set; }
    public int Stock {get; set; }
    public int DealerId {get; set; }
}