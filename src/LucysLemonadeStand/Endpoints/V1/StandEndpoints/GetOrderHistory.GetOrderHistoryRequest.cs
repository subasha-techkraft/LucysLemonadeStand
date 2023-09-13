using LucysLemonadeStand.SharedKernel.Pagination;

namespace LucysLemonadeStand.Endpoints.V1.StandEndpoints;

public record GetOrderHistoryRequest : SimplePaginationSettings
{
    public const string Route = "/orders";
}
