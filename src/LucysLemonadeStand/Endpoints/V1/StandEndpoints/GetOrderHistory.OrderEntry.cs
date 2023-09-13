namespace LucysLemonadeStand.Endpoints.V1.StandEndpoints;

public class OrderEntry
{
    public int Type { get; set; }
    public int Cups { get; set; }
    public decimal CashGiven { get; set; }
    public decimal Change { get; set; }

    public DateTimeOffset Occurred { get; set; }
}
