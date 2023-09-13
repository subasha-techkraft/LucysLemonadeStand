using LucysLemonadeStand.Core.Interfaces;
using LucysLemonadeStand.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LucysLemonadeStand.Core;
public static class BLDISetup
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<ICostCalculationService, CostCalculationService>();
        services.AddTransient<IPitcherRequestService, PitcherRequestService>();
        services.AddTransient<IPurchaseService, PurchaseService>();
        return services;
    }
}
