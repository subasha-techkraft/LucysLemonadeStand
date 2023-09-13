using NJsonSchema.Generation;

namespace LucysLemonadeStand;

public class CustomSchemaNameGenerator : ISchemaNameGenerator
{
    private string MainNamespace { get; } = typeof(CustomSchemaNameGenerator).Namespace!;

  public string Generate(Type type)
  {
    if (!type.Namespace!.StartsWith(MainNamespace))
      return type.FullName!.Replace(".", "");
    if (type.Namespace!.StartsWith(MainNamespace + ".Endpoints."))
    {
      string ns = type.Namespace!.Replace(MainNamespace + ".Endpoints.", "");
      string name = type.Name;
      if (ns.StartsWith("SharedModels."))
        ns = ns.Substring("SharedModels.".Length);
      else //skip category folder
      {
        int firstDot = ns.IndexOf('.');
        if (firstDot > 0)
        {
          int secondDot = ns.IndexOf('.', firstDot + 1);
          name += ns.Substring(0, firstDot); //append version to the end
          if (secondDot >= 0)
            ns = ns.Substring(secondDot + 1);
          else
            ns = "";
        }
        //else
        //{
        //  throw new ArgumentException("A class used for endpoints and is inside the DependencyInjectionWorkshop.Level2.API.Web.Endpoints namespace must also be inside a namespace with a version number, e.g. DependencyInjectionWorkshop.Level2.API.Web.Endpoints.V1");
        //}
      }
      string schemaName = ns.Replace(".", "") + name;
      return schemaName;
    }
    else
    {
      string ns = type.Namespace!.Replace(MainNamespace + ".", "");
      ns = ns.Substring(ns.IndexOf("."));
      string schemaName = ns.Replace(".", "") + type.Name;
      return schemaName;
    }
  }
}
