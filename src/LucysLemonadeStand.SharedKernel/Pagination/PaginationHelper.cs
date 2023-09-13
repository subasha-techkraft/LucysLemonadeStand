using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LucysLemonadeStand.SharedKernel.Pagination;
public class PaginationHelper
{
  public static IComparer<T>? CreateComparer<T>(PaginationSettings paginationSettings)
  {
    if (paginationSettings.OrderBy is null)
    {
      return null;
    }
    IComparer<T> comparer = Comparer<T>.Create((o1, o2) =>
    {
      foreach (FieldOrder fieldOrder in paginationSettings.OrderBy)
      {
        PropertyInfo? prop = typeof(T).GetProperty(fieldOrder.FieldName) ?? throw new ArgumentException("Unknown property {property} of type " + typeof(T).Name + ".", fieldOrder.FieldName);
            object? v1 = prop.GetValue(o1, null);
        object? v2 = prop.GetValue(o2, null);
        if (v1 is null && v2 is null)
          continue;
        if (v1 is null && v2 is not null)
          return 1;
        if (v1 is not null && v2 is null)
          return -1;
        int comp = Comparer.Default.Compare(v1, v2);
        if (comp == 0)
          continue;
        return comp * (fieldOrder.Ascending? 1 : -1);
      }
      return 0;
    });
    return comparer;
  }
}
