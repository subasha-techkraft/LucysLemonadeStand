using System.Data;

namespace LucysLemonadeStand.Infrastructure.DataAccess;
public class DefaultDataTypeMapping : IDataTypeMapping<DbType>
{
    private static Dictionary<Type, DbType> _typeMap;

    static DefaultDataTypeMapping()
    {
        _typeMap = new Dictionary<Type, DbType>();
        _typeMap[typeof(byte)] = DbType.Byte;
        _typeMap[typeof(sbyte)] = DbType.SByte;
        _typeMap[typeof(short)] = DbType.Int16;
        _typeMap[typeof(ushort)] = DbType.UInt16;
        _typeMap[typeof(int)] = DbType.Int32;
        _typeMap[typeof(uint)] = DbType.UInt32;
        _typeMap[typeof(long)] = DbType.Int64;
        _typeMap[typeof(ulong)] = DbType.UInt64;
        _typeMap[typeof(float)] = DbType.Single;
        _typeMap[typeof(double)] = DbType.Double;
        _typeMap[typeof(decimal)] = DbType.Decimal;
        _typeMap[typeof(bool)] = DbType.Boolean;
        _typeMap[typeof(string)] = DbType.String;
        _typeMap[typeof(char)] = DbType.StringFixedLength;
        _typeMap[typeof(Guid)] = DbType.Guid;
        _typeMap[typeof(DateTime)] = DbType.DateTime;
        _typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
        _typeMap[typeof(byte[])] = DbType.Binary;
        _typeMap[typeof(byte?)] = DbType.Byte;
        _typeMap[typeof(sbyte?)] = DbType.SByte;
        _typeMap[typeof(short?)] = DbType.Int16;
        _typeMap[typeof(ushort?)] = DbType.UInt16;
        _typeMap[typeof(int?)] = DbType.Int32;
        _typeMap[typeof(uint?)] = DbType.UInt32;
        _typeMap[typeof(long?)] = DbType.Int64;
        _typeMap[typeof(ulong?)] = DbType.UInt64;
        _typeMap[typeof(float?)] = DbType.Single;
        _typeMap[typeof(double?)] = DbType.Double;
        _typeMap[typeof(decimal?)] = DbType.Decimal;
        _typeMap[typeof(bool?)] = DbType.Boolean;
        _typeMap[typeof(char?)] = DbType.StringFixedLength;
        _typeMap[typeof(Guid?)] = DbType.Guid;
        _typeMap[typeof(DateTime?)] = DbType.DateTime;
        _typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;

    }

    public DbType GetMappedType(Type type)
    {
        if (!_typeMap.TryGetValue(type, out DbType DbType))
        {
            throw new ArgumentException("Unknown SQL DB Type equivalent for \"" + type.Name + "\".");
        }
        return DbType;
    }
}