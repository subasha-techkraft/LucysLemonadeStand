using LucysLemonadeStand.Core.Interfaces;
using LucysLemonadeStand.Infrastructure.DataAccess;
using LucysLemonadeStand.Infrastructure.Repositories;
using LucysLemonadeStand.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mom.Client;
using System.Data;

namespace LucysLemonadeStand.Infrastructure;
public static class InfrastructureDependencyInjectionSetup
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IDataTypeMapping<DbType>, DefaultDataTypeMapping>();
        services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
        return services;
    }

    public static IServiceCollection AddRepositoriesWithInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ICashBoxRepository, CashBoxRepository>();
        services.AddSingleton<IOrderRepository, OrderRepository>();
        services.AddSingleton<IPitcherRepository, PitcherRepository>();
        services.AddSingleton<IPricesRepository, PricesRepository>();
        return services;
    }

    /// <summary>
    /// Uses the AddHttpClient extension to register IMomClient. Also registers IMomService.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddMomClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IMomClient, MomClient>(configureClient => //typed clients are normal clients that are tied to the class they will be injected into
        {
            configureClient.BaseAddress = new Uri(configuration["Mom:BaseURL"]!);
        });
        //services.AddTransient<IMomClient, MomClient>(); //do not include this. The above statement does this on top of registering a typed client.
        services.AddTransient<IMomService, MomService>();
        return services;
    }
}