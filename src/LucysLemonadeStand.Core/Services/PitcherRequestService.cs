using LucysLemonadeStand.Core.Interfaces;
using System.Linq;

namespace LucysLemonadeStand.Core.Services;
public class PitcherRequestService : IPitcherRequestService
{
    private readonly IPricesRepository _pricesRepository;
    private readonly IPitcherRepository _pitcherRepository;
    private readonly ICashBoxRepository _cashBoxRepository;
    private readonly IMomService _momService;

    public PitcherRequestService(IPricesRepository pricesRepository, IPitcherRepository pitcherRepository, ICashBoxRepository cashBoxRepository, IMomService momService)
    {
        _pricesRepository = pricesRepository;
        _pitcherRepository = pitcherRepository;
        _cashBoxRepository = cashBoxRepository;
        _momService = momService;
    }
    public async Task<int> RequestPitcher()
    {
        decimal pricePerPitcher = (await _pricesRepository.GetAllAsync()).Single(p => p.Item == "Refill of 8 cups").Price;
        if ((await _cashBoxRepository.GetAllAsync()).Single().CashOnHand < pricePerPitcher)
        {
            throw new InvalidOperationException("Cannot afford a pitcher from mom.");
        }
        int cupsMade = 0;
        try
        {
            cupsMade = await _momService.AskForAPitcher((float)pricePerPitcher);
        } catch (HttpRequestException hrex)
        {
            throw new Exception("Mom did not make a pitcher of lemonade.", hrex);
        } catch (Exception ex)
        {
            throw new Exception("Mom did not make a pitcher of lemonade.", ex);
        }
        if (cupsMade == 0)
            return 0;
        if (await _cashBoxRepository.UpdateAsync(new Models.CashBox { CashOnHand = pricePerPitcher * -1 })) //withdraw $3.20
        {
            if (await _pitcherRepository.UpdateAsync(new Models.Pitcher() { Cups = cupsMade }))
            {
                return cupsMade;
            }
        }
        return 0;
    }
}
