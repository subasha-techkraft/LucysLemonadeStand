namespace LucysLemonadeStand.Endpoints.V1.StandEndpoints;

public class GetOrderHistoryResponse
{
    public IEnumerable<OrderEntry>? Orders { get; set; }
}
