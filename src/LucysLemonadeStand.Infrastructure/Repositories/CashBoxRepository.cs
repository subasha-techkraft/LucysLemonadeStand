using LucysLemonadeStand.Core.Interfaces;
using LucysLemonadeStand.Core.Models;
using LucysLemonadeStand.Infrastructure.DataAccess;

namespace LucysLemonadeStand.Infrastructure.Repositories;

public class CashBoxRepository : ICashBoxRepository
{
    private readonly ISqlDataAccess _db;

    public CashBoxRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public Task<IEnumerable<CashBox>> GetAllAsync()
    {
        return _db.LoadData<CashBox>("[dbo].[stp_ReadCashBox]");
    }

    public Task<CashBox?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<int> InsertAsync(CashBox record)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<int>> InsertAsync(IEnumerable<CashBox> records)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(CashBox record)
    {
        int returnVal = await _db.SaveDataAndGetReturnInt("[dbo].[stp_UpdateCashBox]", new { AmountToAdd = record.CashOnHand });
        return returnVal == 0;
    }

    public Task<int> UpdateAsync(IEnumerable<CashBox> records)
    {
        throw new NotImplementedException();
    }
}
