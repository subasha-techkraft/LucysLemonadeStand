using FastEndpoints;

namespace LucysLemonadeStand.Endpoints.V1.StandEndpoints;

public class BuyRequest
{
    public const string Route = "/buy";

    public int Cups { get; set; }

    public decimal Cash { get; set; }
}

//public class BuyRequestValidator : Validator<BuyRequest>
//{ }
