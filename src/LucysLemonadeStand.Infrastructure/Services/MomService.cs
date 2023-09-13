using LucysLemonadeStand.Core.Interfaces;
using Mom.Client;

namespace LucysLemonadeStand.Infrastructure.Services
{
    public class MomService : IMomService
    {
        private readonly IMomClient _momClient;

        public MomService(IMomClient momClient) 
        {
            _momClient = momClient;
        }

        public async Task<int> AskForAPitcher(float cash)
        {
            //if failure, throw exception
            Mom.Client.Pitcher result = await _momClient.MakeLemonadeAsync(cash);
            return result.Cups;
        }
    }
}
