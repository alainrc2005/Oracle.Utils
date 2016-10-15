/*-----------------------------------------------*
 * Programado por Alain Ramírez Cabrejas         *
 * Camagüey, Ciudad de los Tinajones.            *
 * Julio 2006                                    *
 *-----------------------------------------------*
 */
using System;
using System.Data;
using Oracle.DataAccess.Client;

namespace Oracle.Utils
{
  /// <summary>
  /// Clase para algunos métodos Nativos entre el Oracle y el .NET Framework
  /// </summary>
  [History("11/06/2007", "Adicionado el método ConvertNumberToOraDbType y CreateParams")]
  [History("07/06/2007", "Ampliación del método OracleToFramework")]
  public static class OracleNative
  {
    /// <summary>
    /// Convierte un tipo de datos de oracle a un tipo de datos del .NET Framework
    /// </summary>
    /// <param name="typeOracle">Tipo de datos oracle</param>
    /// <returns>Tipo de datos del .NET Framework</returns>
    public static Type OracleToFramework(OracleDbType typeOracle)
    {
      switch (typeOracle)
      {
        case OracleDbType.Int16:
          return typeof(short);
        case OracleDbType.Int32:
          return typeof(int);
        case OracleDbType.Int64:
          return typeof(long);
        case OracleDbType.Single:
          return typeof (float);
        case OracleDbType.Double:
          return typeof (double);
        case OracleDbType.Decimal:
          return typeof(decimal);
        case OracleDbType.BFile:
        case OracleDbType.Blob:
        case OracleDbType.LongRaw:
        case OracleDbType.Raw:
          return typeof(Byte[]);
        case OracleDbType.Char:
        case OracleDbType.Clob:
        case OracleDbType.Long:
        case OracleDbType.NClob:
        case OracleDbType.NChar:
        case OracleDbType.NVarchar2:
        case OracleDbType.Varchar2:
          return typeof(String);
        case OracleDbType.Date:
        case OracleDbType.TimeStamp:
        case OracleDbType.TimeStampLTZ:
        case OracleDbType.TimeStampTZ:
          return typeof(DateTime);
        case OracleDbType.IntervalDS:
          return typeof(TimeSpan);
        case OracleDbType.IntervalYM:
          return typeof(Int64);
        case OracleDbType.RefCursor:
        default:
          throw new DataBaseException("No Aplicable");
      }
    }
    
    /// <summary>
    /// Retorna el tipo de dato Oracle que le corresponde a una columna de una tabla
    /// </summary>
    /// <param name="dc">DataColumn</param>
    /// <returns>OracleDbType</returns>
    public static OracleDbType FrameworkToOracle(DataColumn dc)
    {
      return (OracleDbType)byte.Parse(dc.ExtendedProperties["OraDbType"].ToString());
    }

    /// <summary>
    /// Retorna el tipo de datos numérico de Oracle a partir de la Precisión y la Escala
    /// </summary>
    /// <param name="precision">Precisión</param>
    /// <param name="scale">Escala</param>
    /// <returns></returns>
    public static OracleDbType ConvertNumberToOraDbType(int precision, int scale)
    {
      OracleDbType @decimal = OracleDbType.Decimal;
      if ((scale <= 0) && ((precision - scale) < 5))
      {
        return OracleDbType.Int16;
      }
      if ((scale <= 0) && ((precision - scale) < 10))
      {
        return OracleDbType.Int32;
      }
      if ((scale <= 0) && ((precision - scale) < 0x13))
      {
        return OracleDbType.Int64;
      }
      if ((precision < 8) && (((scale <= 0) && ((precision - scale) <= 0x26)) || ((scale > 0) && (scale <= 0x2c))))
      {
        return OracleDbType.Single;
      }
      if (precision < 0x10)
      {
        @decimal = OracleDbType.Double;
      }
      return @decimal;
    }

    /// <summary>
    /// Crea un parámetro Oracle a partir de los datos de entrada
    /// </summary>
    /// <param name="paramName">Nombre del Parámetro</param>
    /// <param name="colType">Tipo de datos Oracle</param>
    /// <param name="value">Valor del parámetro</param>
    /// <param name="direction">Dirección del parámetro</param>
    /// <returns>OracleParameter</returns>
    public static OracleParameter CreateParams(string paramName, OracleDbType colType, object value, ParameterDirection direction)
    {
      int size = 0; // OracleParameter.InvalidSize == -1;
      if (value != null)
      {
        if (value is char)
        {
          size = 1;
        }
        else if (value is string)
        {
          size = ((string)value).Length;
        }
        else if (value is char[])
        {
          size = ((char[])value).Length;
        }
        else if (value is byte[])
        {
          size = ((byte[])value).Length;
        }
      }
      if (colType == OracleDbType.Clob)
      {
        colType = OracleDbType.Varchar2;
      }
      else if (colType == OracleDbType.NClob)
      {
        colType = OracleDbType.NVarchar2;
      }
      else if (colType == OracleDbType.Blob)
      {
        colType = OracleDbType.Raw;
      }
      return new OracleParameter(paramName, colType, size, value, direction);
    }

    /// <summary>
    /// Crea un parámetro Oracle a partir de los datos de entrada
    /// </summary>
    /// <param name="paramName">Nombre del Parámetro</param>
    /// <param name="colType">Tipo de datos Oracle</param>
    /// <param name="direction">Dirección del parámetro</param>
    /// <returns>OracleParameter</returns>
    public static OracleParameter CreateParams(string paramName, OracleDbType colType, ParameterDirection direction)
    {
      return CreateParams(paramName, colType, null, direction);
    }

  }

}
