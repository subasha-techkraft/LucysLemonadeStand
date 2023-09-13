using FluentAssertions;
using LucysLemonadeStand.Core.Models;
using LucysLemonadeStand.Infrastructure.DataAccess;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace LucysLemonadeStand.Infrastructure.IntegrationTests;

public class SQLDataAccessTests : IClassFixture<DatabaseTestContainerSetup>
{
    private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();

    //I've noticed a few things that may cause this to fail.
    //If you get some issues with this, make sure any containers that use external port 1433 are turned off. 
    public SQLDataAccessTests(DatabaseTestContainerSetup dbSetup)
    {
        _configuration["ConnectionStrings:Default"].Returns(dbSetup.ConnectionString);
        _configuration.GetConnectionString("Default").Returns(dbSetup.ConnectionString);
    }

    [Fact]
    public async Task LoadSingle_GetsCashbox_WhenReadCashBoxIsCalled()
    {
        SqlDataAccess sut = new(_configuration, new DefaultDataTypeMapping());
        CashBox? box = await sut.LoadSingle<CashBox>("[dbo].[stp_ReadCashBox]");
        box.Should().NotBeNull();
        box!.CashOnHand.Should().Be(20m);
    }
}