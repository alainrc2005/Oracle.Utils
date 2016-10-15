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
  [Serializable]
  public class TnsException : Exception
  {
    /// <remarks/>
    public TnsException() : base() { }

    /// <remarks/>
    public TnsException(String s) : base(s) { }
  }
}