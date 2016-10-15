/*-----------------------------------------------*
 * Programado por Alain Ramírez Cabrejas         *
 * Camagüey, Ciudad de los Tinajones.            *
 * Julio 2006                                    *
 *-----------------------------------------------*
 */
using System;

namespace Oracle.Utils
{
  /// <summary>
  /// Clase para el tratamiento de Excepciones
  /// </summary>
  [History("28/08/2006","Manipulación de exceptiones")]
  public sealed class DataBaseException : Exception
  {
    /// <summary>
    /// Constructor de la clase de excepciones DataBaseException
    /// </summary>
    /// <param name="msg">Mensaje de excepción</param>
    public DataBaseException(string msg) : base(msg) { }
  }
}
