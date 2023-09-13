using FluentAssertions;
using RichardSzalay.MockHttp;
using Mom.Client;
using System.Net;
using LucysLemonadeStand.Infrastructure.Services;

namespace LucysLemonadeStand.Infrastructure.UnitTests;

/// <summary>
/// This is an example of what to do when the code you're unit testing has HttpClient as an injected dependency. 
/// HttpClient is a notoriously tricky case since it has no direct interfaces that it implements and the one class up the inheritance chain only implements IDispoable.
/// NSubstitute and similar libraries cannot be used to mock it as a result.
/// The good news is that it takes in a HttpMessageHandler as a constructor parameter, and that can be mocked, although with a bit of hacking.
/// Luckily, the RichardSzalay.MockHttp NuGet package does this hacking for us and makes it easy to create a mock HttpMessageHandler with custom responses.
/// 
/// This is a slightly contrived example since IMomClient exists and we can use NSubstitute to mock that instead.
/// </summary>
public class MomServiceTests
{
    [Fact]
    public async Task AskForAPitcher_Returns8Cups_WhenCashIsSufficient()
    {
        //Arrange
        MockHttpMessageHandler mockHttp = new();
        mockHttp.When("http://localhost:5216/makelemonade?cash=3.2")
            .Respond("application/json", """
            {
                "cups": 8
            }
            """);
        HttpClient client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost:5216");
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
        MockHttpMessageHandler mockHttp = new();
        mockHttp.When("http://localhost:5216/makelemonade?cash=3.3")
            .Respond(() =>
            {
                HttpResponseMessage response = new();
                response.Headers.Add("retry-after", "5");
                response.StatusCode = HttpStatusCode.TooManyRequests;
                return Task.FromResult(response);
            });
        HttpClient client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost:5216");
        MomClient momClient = new(client);
        MomService sut = new(momClient);
        //Act
        Func<Task<int>> call = async () => await sut.AskForAPitcher(3.3f);
        //Assert
        call.Should().ThrowAsync<HttpRequestException>();
    }
}
