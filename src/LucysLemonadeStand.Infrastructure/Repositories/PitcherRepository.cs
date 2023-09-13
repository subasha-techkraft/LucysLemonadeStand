using LucysLemonadeStand.Core.Interfaces;
using LucysLemonadeStand.Core.Models;
using LucysLemonadeStand.Infrastructure.DataAccess;

namespace LucysLemonadeStand.Infrastructure.Repositories;
public class PitcherRepository : IPitcherRepository
{
    private readonly ISqlDataAccess _db;

    public PitcherRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public Task<IEnumerable<Pitcher>> GetAllAsync()
    {
        return _db.LoadData<Pitcher>("[dbo].[stp_ReadPitcher]");
    }

    public Task<Pitcher?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<int> InsertAsync(Pitcher record)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<int>> InsertAsync(IEnumerable<Pitcher> records)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateAsync(Pitcher record)
    {
        await _db.SaveData<dynamic>("[dbo].[stp_UpdatePitcher]", new
        {
            record.Cups
        });
        return true;
    }

    public Task<int> UpdateAsync(IEnumerable<Pitcher> records)
    {
        throw new NotImplementedException();
    }
}