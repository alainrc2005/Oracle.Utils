/*-----------------------------------------------*
 * Convertido de Oracle Project Raptor (Java)    *
 * por Alain Ramírez Cabrejas            *
 * Camagüey, Ciudad de los Tinajones.            *
 * Septiembre 2006                               *
 *-----------------------------------------------*
 */
using System;

namespace Oracle.Utils.TNSParser
{
  /// <remarks/>
  public struct SchemaObject_Fields
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
  public interface SchemaObject
  {
    /// <remarks/>
    int isA();
    /// <remarks/>
    String isA_String();
    /// <remarks/>
    void initFromString(String s);
    /// <remarks/>
    void initFromNVPair(NVPair nvpair);
    /// <remarks/>
    String ToString();
  }
}