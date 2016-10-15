/*-----------------------------------------------*
 * Programado por Alain Ram�rez Cabrejas         *
 * Camag�ey, Ciudad de los Tinajones.            *
 * Julio 2006                                    *
 *-----------------------------------------------*
 */
using System;
using System.Collections;
using System.Data;
using System.Text;
using Oracle.DataAccess.Client;

namespace Oracle.Utils
{
  /// <summary>
  /// Clase encargada de m�todos generales de la aplicaci�n
  /// </summary>
  [History("11/07/2007","Arreglada la creaci�n de par�metros a partir de OracleNative")]
  [History("01/06/2007","Correcci�n de m�todo CreateOracleColumn")]
  public sealed class OracleUtils
  {
    #region Create Oracle Parameters
    /// <summary>
    /// Crea un parametro Oracle de tipo <c>VARCHAR2</c>
    /// </summary>
    /// <param name="name">Nombre del par�metro</param>
    /// <param name="value">Valor del par�metro</param>
    /// <returns>Retorna un valor de tipo OracleParameter</returns>
    public static OracleParameter CreateVarchar2(string name, string value)
    {
      return OracleNative.CreateParams(name,OracleDbType.Varchar2,value,ParameterDirection.Input);
    }

    /// <summary>
    /// Crea un parametro Oracle de tipo <c>DATE</c>.
    /// </summary>
    /// <param name="name">Nombre del par�metro</param>
    /// <param name="value">Valor del par�metro</param>
    /// <returns>Retorna un valor de tipo OracleParameter</returns>
    public static OracleParameter CreateDateTime(string name, DateTime value)
    {
      return OracleNative.CreateParams(name, OracleDbType.Date, value, ParameterDirection.Input);
    }

    /// <summary>
    /// Crea un parametro Oracle de tipo <c>Cursor Ref</c>.
    /// </summary>
    /// <param name="name">Nombre del par�metro</param>
    /// <returns>Retorna un valor de tipo OracleParameter</returns>
    public static OracleParameter CreateCursorRefOut(string name)
    {
      return OracleNative.CreateParams(name, OracleDbType.RefCursor, ParameterDirection.Output);
    }

    /// <summary>
    /// Crea un par�metro Oracle de tipo <c>BLOB IN</c>
    /// </summary>
    /// <param name="name">Nombre del par�metro</param>
    /// <param name="value">Contenido del BLOB</param>
    /// <returns>Retorna un valor de tipo OracleParameter</returns>
    public static OracleParameter CreateBlob(string name, object value)
    {
      return OracleNative.CreateParams(name, OracleDbType.Blob, value, ParameterDirection.Input);
    }

    /// <summary>
    /// Crea un parametro Oracle de tipo <c>NUMBER(n) OUT</c>.
    /// </summary>
    /// <param name="name">Nombre del par�metro</param>
    /// <returns>Retorna un valor de tipo OracleParameter</returns>
    public static OracleParameter CreateInt32Out(string name)
    {
      return OracleNative.CreateParams(name, OracleDbType.Int32, ParameterDirection.Output);
    }

    /// <summary>
    /// Crea un parametro Oracle de tipo <c>NUMBER(n)</c>.
    /// </summary>
    /// <param name="name">Nombre del par�metro</param>
    /// <param name="value">Valor del par�metro</param>
    /// <returns>Retorna un valor de tipo OracleParameter</returns>
    public static OracleParameter CreateInt32(string name, int value)
    {
      return OracleNative.CreateParams(name, OracleDbType.Int32, value, ParameterDirection.Input);
    }

    /// <summary>
    /// Crea una columna con las propiedades extendidas para Oracle
    /// </summary>
    /// <param name="columnName">Nombre de la columna</param>
    /// <param name="columnType"></param>
    /// <returns>DataColum</returns>
    public static DataColumn CreateOracleColumn(string columnName, OracleDbType columnType)
    {
      DataColumn dc = new DataColumn(columnName, OracleNative.OracleToFramework(columnType));
      dc.ExtendedProperties["OraDbType"] = (byte)columnType;
      return dc;
    }
    
    #endregion
  }

  /// <summary>
  /// Utilitarios generales
  /// </summary>
  public static class Utils
  {
    /// <summary>
    /// <para>Libera los objetos pasados como par�metros utilizando la interface IDisposable</para>
    /// <para>si la tienen y luego asigna null a estos</para>
    /// </summary>
    /// <param name="objs">Arreglo de objetos a liberar</param>
    public static void DisposeAndNull(params IDisposable[] objs)
    {
      if (objs != null && objs.Length != 0)
        for (int i = 0; i < objs.Length; i++)
          if (objs[i] != null)
          {
            objs[i].Dispose();
            objs[i] = null;
          }
    }

    /// <summary>
    /// Libera los objetos que son de la interfase IList pasados como par�metros
    /// y luego asigna null a estos (Ej. Array)
    /// </summary>
    /// <param name="objs"></param>
    public static void ClearAndNull(params IList[] objs)
    {
      if (objs != null && objs.Length != 0)
      {
        for (int i = 0; i < objs.Length; i++)
        {
          if (objs[i] != null)
          {
            objs[i].Clear();
            objs[i] = null;
          }
        }
      }
    }
  }

  /// <summary>
  /// Clase encargada de la conversi�n num�rica de una base a otra
  /// </summary>
  public static class BaseConverter
  {
    /// <summary>
    ///Convierte un n�mero de cualquier base entre 2 y 36 a cualquier base entre 2 y 36
    /// </summary>
    /// <param name="numberAsString">N�mero a convertir</param>
    /// <param name="fromBase">La base desde la cual quiere convertir (Entre 2 y 36).</param>
    /// <param name="toBase">La base a la cual se convertir� (Entre 2 y 36).</param>
    /// <returns>The number converted from fromBase to toBase.</returns>
    public static string ToBase(string numberAsString, ushort fromBase, ushort toBase)
    {
      ulong base10 = ToBase10(numberAsString, fromBase);
      return FromBase10(base10, toBase);
    }

    /// <summary>
    ///Convierte un n�mero desde cualquier base entre 2 y 36 a base 10.
    /// </summary>
    /// <param name="encodedNumber">N�mero a convertir.</param>
    /// <param name="fromBase">La base desde la cual se va a convertir (Entre 2 y 36).</param>
    /// <returns>El n�mero convertido a base 10.</returns>
    public static ulong ToBase10(string encodedNumber, ushort fromBase)
    {

      // If the from base is 10, simply parse the string and return it
      if (fromBase == 10)
      {
        return UInt64.Parse(encodedNumber);
      }

      // Ensure that the string only contains upper case characters
      encodedNumber = encodedNumber.ToUpper();

      // Go through each character and decode its value.
      int length = encodedNumber.Length;
      ulong runningTotal = 0;

      for (int index = 0; index < length; index++)
      {
        char currentChar = encodedNumber[index];

        // Anything above base 10 uses letters as well as numbers, so A will be 10, B will be 11, etc.
        uint currentValue = Char.IsDigit(currentChar) ? (uint)(currentChar - '0') :
        (uint)(currentChar - 'A' + 10);

        // The value which of the character represents depends on its position and it is calculated
        // by multiplying its value with the power of the base to the position of the character, from
        // right to left.
        runningTotal += currentValue * (ulong)Math.Pow(fromBase, length - index - 1);
      }

      return runningTotal;
    }

    /// <summary>
    ///Convierte un n�mero de base 10 a cualquier base entre 2 y 36.
    /// </summary>
    /// <param name="number">El n�mero a convertir.</param>
    /// <param name="toBase">La base a la cual se desea convertir (Entre 2 y 36).</param>
    /// <returns>El n�mero convertido desde la base 10.</returns>
    public static string FromBase10(ulong number, ushort toBase)
    {
      //If the to base is 10, simply return the number as a string
      if (toBase == 10)
      {
        return number.ToString();
      }

      // The number has to be divided by the base it needs to be converted to
      // until the result of the division is 0. The modulus of the division 
      // is used to calculate the character that represents it
      StringBuilder runningResult = new StringBuilder();

      while (number > 0)
      {
        ulong modulus = number % toBase;

        if (modulus < 10)
        {
          runningResult.Insert(0, modulus);
        }
        else
        {
          runningResult.Insert(0, (char)('A' + modulus - 10));
        }

        number = (number - modulus) / toBase;
      }
      return runningResult.ToString();
    }
  }
}
