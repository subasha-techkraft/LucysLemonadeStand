using FluentAssertions;
using LucysLemonadeStand.Infrastructure.Services;
using Mom.Client;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace LucysLemonadeStand.Infrastructure.IntegrationTests;
public class MomServiceTestsWithWireMock : IClassFixture<MomMockAPI>
{
    private readonly MomMockAPI _momMockAPI;

    public MomServiceTestsWithWireMock(MomMockAPI momMockAPI)
    {
        _momMockAPI = momMockAPI;
    }

    [Fact]
    public async Task AskForAPitcher_Returns8Cups_WhenCashIsSufficient()
    {
        //Arrange
        _momMockAPI.WireMockServer
            .Given(Request.Create()
                .WithPath("/makelemonade")
                .WithParam("cash", "3.2")
            )
            .RespondWith(Response.Create()
                .WithBodyAsJson(new { cups = 8 })
                .WithStatusCode(HttpStatusCode.OK)
            );

        HttpClient client = _momMockAPI.WireMockServer.CreateClient();
        MomClient momClient = new(client);
        MomService sut = new(momClient);
        //Act
        int pitcher = await sut.AskForAPitcher(3.2f);
        //Assert
        pitcher.Should().Be(8);
    }

    [Fact]
    public void AskForAPitcher_Throws429Exception_WhenMomIsTired()
    {
        //Arrange
        _momMockAPI.WireMockServer
            .Given(Request.Create()
                .WithPath("/makelemonade")
                .WithParam("cash", "3.3")
            )
            .RespondWith(Response.Create()
                .WithHeader("retry-after", "5")
                .WithStatusCode(HttpStatusCode.TooManyRequests)
            );

        HttpClient client = _momMockAPI.WireMockServer.CreateClient();
        MomClient momClient = new(client);
        MomService sut = new(momClient);
        //Act
        Func<Task<int>> call = async () => await sut.AskForAPitcher(3.3f);
        //Assert
        call.Should().ThrowAsync<HttpRequestException>();
    }
}
