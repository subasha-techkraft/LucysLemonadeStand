using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace LucysLemonadeStand.Infrastructure.DataAccess;
public class SqlDataAccess : ISqlDataAccess
{
    private readonly IConfiguration _configuration;
    private readonly IDataTypeMapping<DbType> _dataTypeMapping;

    public SqlDataAccess(IConfiguration configuration, IDataTypeMapping<DbType> dataTypeMapping)
    {
        _configuration = configuration;
        _dataTypeMapping = dataTypeMapping;
    }

    public async Task<TModel?> LoadSingle<TModel>(string storedProcedure, string connectionID = "Default")
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionID));
        return await connection.QueryFirstOrDefaultAsync<TModel>(storedProcedure, commandType: CommandType.StoredProcedure);
    }

    public async Task<TModel?> LoadSingle<TModel, TParams>(string storedProcedure, TParams parameters, string connectionID = "Default") where TParams : class
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionID));
        return await connection.QueryFirstOrDefaultAsync<TModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<T> LoadValue<T>(string storedProcedure, Func<dynamic, T> valueSelector, string connectionID = "Default")
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionID));
        dynamic record = await connection.QueryFirstAsync(storedProcedure, commandType: CommandType.StoredProcedure);
        return valueSelector(record);
    }

    public async Task<T> LoadValue<TModel, T>(string storedProcedure, Func<TModel, T> valueSelector, string connectionID = "Default")
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionID));
        dynamic? record = await connection.QueryFirstAsync<TModel>(storedProcedure, commandType: CommandType.StoredProcedure);
        return valueSelector(record);
    }

    public async Task<IEnumerable<TModel>> LoadData<TModel>(string storedProcedure, string connectionID = "Default")
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionID));
        return await connection.QueryAsync<TModel>(storedProcedure, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<TModel>> LoadData<TModel, TParams>(string storedProcedure, TParams parameters, string connectionID = "Default") where TParams : class
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionID));
        return await connection.QueryAsync<TModel>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task SaveData<TParams>(string storedProcedure, TParams parameters, string connectionID = "Default") where TParams : class
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionID));
        await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> SaveDataAndGetReturnInt<TParams>(string storedProcedure, TParams parameters, string connectionID = "Default") where TParams : class
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionID));
        DynamicParameters ps = new();
        ps.AddDynamicParams(parameters);
        ps.Add("@return", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await connection.ExecuteAsync(storedProcedure, ps, commandType: CommandType.StoredProcedure);
        return ps.Get<int>("@return");
    }

    public async Task<TId> SaveRecordAndGetID<TParams, TId>(string storedProcedure, TParams parameters, string idParamName, string connectionID = "Default") where TParams : class
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionID));
        DynamicParameters ps = new();
        ps.AddDynamicParams(parameters);
        ps.Add(idParamName, dbType: _dataTypeMapping.GetMappedType(typeof(TId)), direction: ParameterDirection.Output);
        await connection.ExecuteAsync(storedProcedure, ps, commandType: CommandType.StoredProcedure);
        return ps.Get<TId>(idParamName);
    }

    public async Task<int> SaveRecordsAndGetCount<TParams>(string storedProcedure, TParams parameters, string countParamName, string connectionID = "Default") where TParams : class
    {
        using IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionID));
        DynamicParameters ps = new();
        ps.AddDynamicParams(parameters);
        ps.Add(countParamName, dbType: DbType.Int32, direction: ParameterDirection.Output);
        await connection.ExecuteAsync(storedProcedure, ps, commandType: CommandType.StoredProcedure);
        return ps.Get<int>(countParamName);
    }

    public async Task<IMultipleResultSets> LoadMultiple<TParams>(string storedProcedure, TParams parameters, string connectionID = "Default") where TParams : class
    {
        IDbConnection connection = new SqlConnection(_configuration.GetConnectionString(connectionID));
        return new MultipleResultSets(await connection.QueryMultipleAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure), connection);
    }

    public class MultipleResultSets : IMultipleResultSets, IDisposable
    {
        private readonly SqlMapper.GridReader _multi;
        private readonly IDbConnection _dbConnection;

        public MultipleResultSets(SqlMapper.GridReader multi, IDbConnection dbConnection)
        {
            _multi = multi;
            _dbConnection = dbConnection;
        }

        public IMultipleResultSets Read<TModel>(out IEnumerable<TModel> data)
        {
            data = _multi.Read<TModel>();
            return this;
        }

        public IMultipleResultSets ReadSingle<TModel>(out TModel? data)
        {
            data = _multi.Read<TModel?>().FirstOrDefault();
            return this;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _multi.Dispose();
            _dbConnection.Dispose();
        }
    }
}
