using System;

namespace LucysLemonadeStand.Infrastructure.DataAccess;
public interface IDataTypeMapping<TTypeEnum> where TTypeEnum : Enum
{
    TTypeEnum GetMappedType(Type type);
}