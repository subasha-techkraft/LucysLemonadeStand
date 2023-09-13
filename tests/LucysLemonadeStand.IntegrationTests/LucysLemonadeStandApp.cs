using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using static Microsoft.Extensions.DependencyInjection.ServiceDescriptor;
using LucysLemonadeStand.Core.Interfaces;
using LucysLemonadeStand.Core.Models;
using NSubstitute;
using LucysLemonadeStand.Infrastructure.Repositories;
using LucysLemonadeStand.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace LucysLemonadeStand.IntegrationTests;
public class LucysLemonadeStandApp : WebApplicationFactory<Program>
{    
    private readonly string _connectionString;
    private readonly ServiceDescriptor[] _replacements;

    public LucysLemonadeStandApp(string connectionString, params ServiceDescriptor[] replacements)
    {
        _connectionString = connectionString;
        _replacements = replacements;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(new KeyValuePair<string, string?>[] //modify the Configuration parsed from appsettings.json
            {
                new("ConnectionStrings:Default", _connectionString)
            });
            
        })
        .ConfigureServices(services =>
        {
            foreach(ServiceDescriptor replacement in _replacements ?? Array.Empty<ServiceDescriptor>())
            {
                services.Replace(replacement);
            }
        });
    }
}
