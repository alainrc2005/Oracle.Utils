/*-----------------------------------------------*
 * Programado por Alain Ram�rez Cabrejas         *
 * Camag�ey, Ciudad de los Tinajones.            *
 * Julio 2006                                    *
 *-----------------------------------------------*
 */
using System;

namespace Oracle.Utils
{
  /// <summary>
  /// Clase para el tratamiento de Excepciones
  /// </summary>
  [History("28/08/2006","Manipulaci�n de exceptiones")]
  public sealed class DataBaseException : Exception
  {
    /// <summary>
    /// Constructor de la clase de excepciones DataBaseException
    /// </summary>
    /// <param name="msg">Mensaje de excepci�n</param>
    public DataBaseException(string msg) : base(msg) { }
  }
}
