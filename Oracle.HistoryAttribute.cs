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
  /// Clase para conservar la fecha y los cambios realizados en las clases del ensamblado
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class HistoryAttribute : Attribute
  {
    #region Private Field's
    private string comment;
    private DateTime date;
    #endregion

    #region Constructors

    /// <summary>
    /// Constructor del atributo
    /// </summary>
    /// <param name="date">Fecha del cambio</param>
    /// <param name="comment">Comentario del cambio</param>
    public HistoryAttribute(string date, string comment)
    {
      this.comment = comment;
      this.date = DateTime.Parse(date);
    }
    #endregion

    #region Properties
    /// <summary>
    /// Comentario del Historial, cambios realizados
    /// </summary>
    public string Comment
    {
      get { return comment; }
      set { comment = value; }
    }

    /// <summary>
    /// Fecha en que se realizaron los cambios
    /// </summary>
    public DateTime Date
    {
      get { return date; }
      set { date = value; }
    }
    #endregion
  }
}
