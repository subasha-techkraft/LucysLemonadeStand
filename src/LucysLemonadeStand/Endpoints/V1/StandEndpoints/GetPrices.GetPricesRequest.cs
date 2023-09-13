namespace LucysLemonadeStand.Endpoints.V1.StandEndpoints;

public class GetPricesRequest
{
    public const string Route = "/priceList"; 

    public string? Item { get;set; }
}