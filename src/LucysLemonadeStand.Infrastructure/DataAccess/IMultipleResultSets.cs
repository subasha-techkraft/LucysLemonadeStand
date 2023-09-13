namespace LucysLemonadeStand.Infrastructure.DataAccess;

public interface IMultipleResultSets
{
    void Dispose();
    IMultipleResultSets Read<TModel>(out IEnumerable<TModel> data);

    IMultipleResultSets ReadSingle<TModel>(out TModel? data);
}