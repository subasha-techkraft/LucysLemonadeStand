using WireMock.Server;

namespace LucysLemonadeStand.Infrastructure.IntegrationTests;
public class MomMockAPI : IDisposable
{
    public readonly WireMockServer WireMockServer;

    public MomMockAPI()
    {
        WireMockServer = WireMockServer.Start();
    }

    public void Dispose()
    {
        WireMockServer?.Dispose();
        GC.SuppressFinalize(this);
    }
}
