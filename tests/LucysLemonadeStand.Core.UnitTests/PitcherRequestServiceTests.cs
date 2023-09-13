using FluentAssertions;
using LucysLemonadeStand.Core.Interfaces;
using LucysLemonadeStand.Core.Models;
using LucysLemonadeStand.Core.Services;
using NSubstitute;

namespace LucysLemonadeStand.Core.UnitTests;
public class PitcherRequestServiceTests
{
    private readonly IPricesRepository _pricesRepository = Substitute.For<IPricesRepository>();
    private readonly IPitcherRepository _pitcherRepository = Substitute.For<IPitcherRepository>();
    private readonly ICashBoxRepository _cashBoxRepository = Substitute.For<ICashBoxRepository>();
    private readonly IMomService _momService = Substitute.For<IMomService>();
    public const decimal REFILL_COST = 3.2m; 

    public static readonly IEnumerable<object[]> _numbersGreaterThanRefillCost = Enumerable.Range(1, 100).Select(x => new object[] { Math.Max((double)REFILL_COST, Random.Shared.NextDouble() * int.MaxValue) });

    [Fact]
    public void RequestPitcher_Throws_WhenCashboxDoesNotHaveEnoughForRefill()
    {
        _pricesRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<PriceEntry>)new List<PriceEntry>() { new PriceEntry() { Item = "Refill of 8 cups", Price = REFILL_COST } }));
        _cashBoxRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<CashBox>)new List<CashBox>() { new CashBox() { CashOnHand = (decimal)Random.Shared.NextDouble() * REFILL_COST } }));
        PitcherRequestService sut = new(_pricesRepository, _pitcherRepository, _cashBoxRepository, _momService);
        Func<Task<int>> call = (async () => await sut.RequestPitcher());
        call.Should().ThrowAsync<InvalidOperationException>();
    }

    [Theory]
    [InlineData(3.2)]
    [MemberData(nameof(_numbersGreaterThanRefillCost))]
    public async Task RequestPitcher_ReturnsCups_WhenSuccessful(decimal cash)
    {
        _pricesRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<PriceEntry>)new List<PriceEntry>() { new PriceEntry() { Item = "Refill of 8 cups", Price = REFILL_COST } }));
        _cashBoxRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<CashBox>)new List<CashBox>() { new CashBox() { CashOnHand = cash } }));
        //_momService.AskForAPitcher((float)cash).Returns(8);
        _momService.AskForAPitcher((float)REFILL_COST).Returns(8);
        _cashBoxRepository.UpdateAsync(new Models.CashBox { CashOnHand = REFILL_COST * -1 }).Returns(Task.FromResult(true));
        _pitcherRepository.UpdateAsync(new Models.Pitcher() { Cups = 8 }).Returns(Task.FromResult(true));
        PitcherRequestService sut = new(_pricesRepository, _pitcherRepository, _cashBoxRepository, _momService);
        int cups = await sut.RequestPitcher();
        cups.Should().Be(8);
    }
}
