namespace LucysLemonadeStand.Infrastructure.DataAccess;

public interface ISqlDataAccess
{
    Task<IEnumerable<TModel>> LoadData<TModel, TParams>(string storedProcedure, TParams parameters, string connectionID = "Default") where TParams : class;
    Task<IEnumerable<TModel>> LoadData<TModel>(string storedProcedure, string connectionID = "Default");
    Task<IMultipleResultSets> LoadMultiple<TParams>(string storedProcedure, TParams parameters, string connectionID = "Default") where TParams : class;
    Task<TModel?> LoadSingle<TModel>(string storedProcedure, string connectionID = "Default");
    Task<TModel?> LoadSingle<TModel, TParams>(string storedProcedure, TParams parameters, string connectionID = "Default") where TParams : class;
    Task<T> LoadValue<T>(string storedProcedure, Func<dynamic, T> valueSelector, string connectionID = "Default");
    Task<T> LoadValue<TModel, T>(string storedProcedure, Func<TModel, T> valueSelector, string connectionID = "Default");
    Task SaveData<TParams>(string storedProcedure, TParams parameters, string connectionID = "Default") where TParams : class;
    Task<int> SaveDataAndGetReturnInt<TParams>(string storedProcedure, TParams parameters, string connectionID = "Default") where TParams : class;
    Task<TId> SaveRecordAndGetID<TParams, TId>(string storedProcedure, TParams parameters, string idParamName, string connectionID = "Default") where TParams : class;
    Task<int> SaveRecordsAndGetCount<TParams>(string storedProcedure, TParams parameters, string countParamName, string connectionID = "Default") where TParams : class;
}