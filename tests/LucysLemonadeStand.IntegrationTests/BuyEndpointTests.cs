using FluentAssertions;
using static Microsoft.Extensions.DependencyInjection.ServiceDescriptor;
//using NSubstitute;
//using LucysLemonadeStand.Core.Interfaces;
//using LucysLemonadeStand.Core.Models;
using LucysLemonadeStand.SharedKernel;
using System.Net;
using System.Text.Json;
using LucysLemonadeStand.Endpoints.V1.StandEndpoints;
using System.Text;

namespace LucysLemonadeStand.IntegrationTests;

/// <summary>
/// This is a class that contains integration tests (one for demo purposes) for the "Buy" endpoint.
/// The class will start up the API and will also launch a Docker container with the database the API will use.
/// The test(s) will create a client to this API and call it so it will be just like if you called the API from a totally external program.
/// </summary>
public class BuyEndpointTests : IClassFixture<DatabaseTestContainerSetup> //class fixtures are automatically created and injected into the constructor and if they implement ILifetime or IAsyncLifetime they can specify what happens on creation and on disposal.
{
    public readonly decimal CupCost = 0.5m;
    public readonly DateTimeOffset OrderTime = new((long)(DateTimeOffset.MaxValue.Ticks / 4.9445126924624905), TimeSpan.FromHours(-4)); //just for fun: 3/30/2023 11:30:30 AM -04:00. Don't actually instantiate dates this way.

    /// <summary>
    /// This type extends WebApplicationFactory<Program> so it really starts the API, though part of the same executable as this one. 
    /// However, it will run through all its normal configuration first.
    /// Anything you want to change must overwrite the existing configuration.
    /// In the case of the constructor, I overwrite the connection string to point to the Docker Testcontainer DB, and can also change the service container registrations to use my mock substitutions.
    /// </summary>
    public LucysLemonadeStandApp App { get; }

    //These repository mocks are not needed when the DB is completely controlled.
    //private readonly IPitcherRepository _pitcherRepository = Substitute.For<IPitcherRepository>();
    //private readonly IPricesRepository _priceRepository = Substitute.For<IPricesRepository>();

    /// <summary>
    /// The constructor sets up the API to be run and overwrites the configuration to point to the Docker database that was spun up when the test was kicked off.
    /// </summary>
    /// <param name="dbSetup">The script to launch a Docker database and make its connection string available.</param>
    public BuyEndpointTests(DatabaseTestContainerSetup dbSetup)
    {
        App = new(dbSetup.ConnectionString,
            //Transient(_ => _pitcherRepository),
            //Transient(_ => _priceRepository),
            Transient(_ => new DateTimeProvider(OrderTime)) //This sets the time that will be used for when an order is created to a fixed time in case we want to test that.
        );
    }

    [Fact]
    public async Task BuyEndpoint_SavesAndReturnsCupsAndChange_WhenOrderIsValid()
    {
        //_pitcherRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Pitcher>)new Pitcher[] { new() { Cups = 8 } }));
        //_priceRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<PriceEntry>)new PriceEntry[] { new() { Item = "Cup", Price = CupCost } }));
        HttpClient appClient = App.CreateClient(); //automatically configured to point to the fully running API defined in LucysLemonadeStand.
        appClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        int orderCountStart = await GetOrderCount(appClient);

        HttpRequestMessage buyMsg = new(HttpMethod.Post, "/v1/buy")
        {
            Content = new StringContent("""
            {
                "cups": 3,
                "cash": 2
            }
            """, Encoding.UTF8, "application/json")
        };
        var response = await appClient.SendAsync(buyMsg);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        BuyResponse? buyResponse = JsonSerializer.Deserialize<BuyResponse>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions() 
        { 
            PropertyNameCaseInsensitive = true //C# property names are PascalCase but get serialized into JSON as camelCase and it's important to allow for that difference when deserializing, or the properties won't get set
        });
        buyResponse.Should().NotBeNull();
        buyResponse!.Cups.Should().Be(3);
        buyResponse!.Change.Should().Be(0.5m);

        //check creation
        int orderCountFinish = await GetOrderCount(appClient);
        orderCountFinish.Should().Be(orderCountStart + 1);
    }

    private static async Task<int> GetOrderCount(HttpClient appClient)
    {
        HttpResponseMessage ordersResponse = await appClient.GetAsync("/v1/orders?PageSize=" + 500); //this is not an ideal way to write this test but I am not interested in implementing pagination perfectly for this demo. Page sizes this big should be rejected by the API normally.
        ordersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        GetOrderHistoryResponse? orders = JsonSerializer.Deserialize<GetOrderHistoryResponse>(await ordersResponse.Content.ReadAsStreamAsync(), new JsonSerializerOptions() 
        {
            PropertyNameCaseInsensitive = true
        });
        return orders?.Orders?.Count() ?? 0;
    }
}