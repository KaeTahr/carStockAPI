public class UpdateCarRequest
{
    public int Id { get; set; }
    public int DealerId { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Stock { get; set; }
}