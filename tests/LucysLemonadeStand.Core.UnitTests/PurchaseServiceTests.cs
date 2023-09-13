using AutoFixture.Xunit2;
using FluentAssertions;
using LucysLemonadeStand.Core.Interfaces;
using LucysLemonadeStand.Core.Models;
using LucysLemonadeStand.Core.Services;
using LucysLemonadeStand.SharedKernel;
using NSubstitute;

namespace LucysLemonadeStand.Core.UnitTests;

public class PurchaseServiceTests
{
    private readonly IPitcherRepository _pitcherRepository = Substitute.For<IPitcherRepository>();
    private readonly IPricesRepository _pricesRepository = Substitute.For<IPricesRepository>();
    private readonly ICostCalculationService _costCalculationService = new CostCalculationService();
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    public static readonly DateTimeOffset TEST_TIME = DateTimeOffset.Now.AddHours(-1);
    private readonly DateTimeProvider _dateTimeProvider = new(TEST_TIME);//arbitrary time

    public static readonly IEnumerable<object[]> _negativeIntegers = Enumerable.Range(1, 100).Select(x => new object[] { Random.Shared.Next(int.MaxValue) * -1 });

    private PurchaseService SetupPurchaseServiceSUT()
    {
        return new(_pitcherRepository, _pricesRepository, _costCalculationService, _orderRepository, _dateTimeProvider);
    }

    [Theory] //it's important to test more than one assumption about validators
    [InlineData(0)] //tests when cups is 0
    [InlineData(-1)] //tests when cups is -1
    [InlineData(int.MinValue)]
    [MemberData(nameof(_negativeIntegers))]
    public async Task VerifyPurchase_ReturnsError_WhenOrderHasNonPositiveCups(int cups)
    {

        PurchaseService sut = SetupPurchaseServiceSUT();
        Order order = new()
        {
            Cups = cups,
            CashGiven = 100m
        };
        var errors = await sut.VerifyPurchase(order);
        errors.Should().NotBeEmpty();
        errors.Should().HaveCount(1);
        errors!.First().Should().Be("Requested cups must be positive.");
    }

    [Theory] //it's important to test more than one assumption about validators
    [InlineData(0)] //tests when cups is 0
    [InlineData(-1)] //tests when cups is -1
    [InlineData(int.MinValue)]
    [MemberData(nameof(_negativeIntegers))]
    public async Task VerifyPurchase_ReturnsError_WhenOrderHasNonPositiveCashGiven(decimal cash)
    {
        PurchaseService sut = SetupPurchaseServiceSUT();
        Order order = new()
        {
            Cups = 1,
            CashGiven = cash
        };
        var errors = await sut.VerifyPurchase(order);
        errors.Should().NotBeEmpty();
        errors.Should().HaveCount(1);
        errors!.First().Should().Be("Cash given must be positive.");
    }

    public static readonly IEnumerable<object[]> _badCupsRequests = Enumerable.Range(1, 100).Select(x =>
    {
        int order = Random.Shared.Next(1, int.MaxValue);
        return new object[] { order, Random.Shared.Next(0, order) };
    });

    [Theory] //it's important to test more than one assumption about validators    
    [MemberData(nameof(_badCupsRequests))]
    public async Task VerifyPurchase_ReturnsError_WhenOrderRequestsMoreCupsThanPitcher(int orderCups, int pitcherCups)
    {
        SetPitcherCupQuantity(pitcherCups);
        PurchaseService sut = SetupPurchaseServiceSUT();
        Order order = new()
        {
            Cups = orderCups,
            CashGiven = decimal.MaxValue
        };
        var errors = await sut.VerifyPurchase(order);
        errors.Should().NotBeEmpty();
        errors.Should().HaveCount(1);
        errors!.First().Should().Be($"The order of {orderCups} cups cannot be fulfilled because the pitcher only has {pitcherCups} cups.");
    }

    public static readonly IEnumerable<object[]> _badCashOrders = Enumerable.Range(1, 100).Select(x =>
    {
        int cups = Random.Shared.Next(1, int.MaxValue);
        decimal cost = cups * COST_PER_CUP;
        decimal cash = (decimal)Random.Shared.NextDouble() * cost;
        return new object[] { cups, cost, cash };
    });

    public const decimal COST_PER_CUP = 0.5m;

    [Theory] //it's important to test more than one assumption about validators    
    [InlineData(int.MaxValue, 1073741823.5, 1e-28)]
    [MemberData(nameof(_badCashOrders))]
    public async Task VerifyPurchase_ReturnsError_WhenOrderProvidesInsufficientCash(int orderCups, decimal cost, decimal cash)
    {
        SetPitcherCupQuantity(int.MaxValue); //make it so that it's impossible to buy more cups than there are
        SetCupPrice();
        PurchaseService sut = SetupPurchaseServiceSUT();
        Order order = new()
        {
            Cups = orderCups,
            CashGiven = cash
        };
        var errors = await sut.VerifyPurchase(order);
        errors.Should().NotBeEmpty();
        errors.Should().HaveCount(1);
        errors!.First().Should().Be($"The cash given (${cash:N2}) does not cover the cost of the cups requested (${cost:N2}).");
    }

    private void SetPitcherCupQuantity(int cups)
    {
        _pitcherRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Pitcher>)new List<Pitcher>() { new Pitcher() { Cups = cups } }));
    }

    private void SetCupPrice()
    {
        _pricesRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<PriceEntry>)new List<PriceEntry>() { new PriceEntry() { Item = "Cup", Price = COST_PER_CUP } }));
    }

    [Theory, AutoData]
    public async Task VerifyPurchase_ReturnsNoErrors_WhenOrderIsValid(Order order)
    {
        order.CashGiven = order.Cups * COST_PER_CUP + (decimal)(Random.Shared.NextSingle() * int.MaxValue);
        _pitcherRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<Pitcher>)new List<Pitcher>() { new Pitcher() { Cups = int.MaxValue } }));
        _pricesRepository.GetAllAsync().Returns(Task.FromResult((IEnumerable<PriceEntry>)new List<PriceEntry>() { new PriceEntry() { Item = "Cup", Price = COST_PER_CUP } }));
        PurchaseService sut = SetupPurchaseServiceSUT();
        var errors = await sut.VerifyPurchase(order);
        errors.Should().BeEmpty();
    }

    [Theory, AutoData]
    public async Task CompletePurchase_ReturnsOrderEntry_WhenSuccessful(Order order)
    {
        order.CashGiven = order.Cups * COST_PER_CUP + 1;
        SetCupPrice();
        _orderRepository.InsertAsync(Arg.Any<OrderEntry>()).Returns(Task.FromResult(5)); //ID = 5
        PurchaseService sut = SetupPurchaseServiceSUT();
        OrderEntry entry = await sut.CompletePurchase(order);
        entry.Should().NotBeNull();
        entry.Type.Should().Be(0);
        entry.Cups.Should().Be(order.Cups);
        entry.CashGiven.Should().Be(order.CashGiven);
        entry.Change.Should().Be(1m);
        entry.Occurred.Should().Be(TEST_TIME);
        entry.ID.Should().Be(5);

    }    
}