/*-----------------------------------------------*
 * Convertido de Oracle Project Raptor (Java)    *
 * por Alain Ramírez Cabrejas            *
 * Camagüey, Ciudad de los Tinajones.            *
 * Septiembre 2006                               *
 *-----------------------------------------------*
 */
namespace Oracle.Utils.TNSParser
{

  /// <remarks/>
  public struct SchemaObjectFactoryInterface_Fields
  {
    /// <remarks/>
    public static readonly int ADDR = 0;
    /// <remarks/>
    public static readonly int ADDR_LIST = 1;
    /// <remarks/>
    public static readonly int DESC = 2;
    /// <remarks/>
    public static readonly int DESC_LIST = 3;
    /// <remarks/>
    public static readonly int ALIAS = 4;
    /// <remarks/>
    public static readonly int SERVICE = 5;
    /// <remarks/>
    public static readonly int DB_SERVICE = 6;
  }

  /// <remarks/>
  public interface SchemaObjectFactoryInterface
  {
    /// <remarks/>
    SchemaObject create(int i);
  }
}