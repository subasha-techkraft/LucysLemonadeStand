namespace LucysLemonadeStand.Endpoints.V1.StandEndpoints;

public class GetPricesResponse
{
    public string Item { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
