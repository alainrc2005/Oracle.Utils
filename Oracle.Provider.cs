/*-----------------------------------------------*
 * Programado por Alain Ramírez Cabrejas         *
 * Camagüey, Ciudad de los Tinajones.            *
 * Julio 2006                                    *
 *-----------------------------------------------*
 */
using System;
using System.Data;
using Oracle.DataAccess.Client;
using System.Xml;

namespace Oracle.Utils
{
  /// <summary>
  /// Define el tipo de consulta que se ejecutará
  /// </summary>
  public enum QueryType
  {
    /// <remarks/>
    Select,
    /// <remarks/>
    Update,
    /// <remarks/>
    Insert,
    /// <remarks/>
    Delete
  }

  /// <summary>
  /// Define la clase de respuesta para el comando ExecuteQuery
  /// </summary>
  public class QueryResponse
  {
    /// <summary>
    /// Cantidad de registros afectados con ExecuteQuery
    /// </summary>
    public int CantidadRegistros;
    /// <summary>
    /// Registros obtenidos con SELECT
    /// </summary>
    public DataSet Registros;
  }

  /// <summary>
  /// Clase proveedora de los métodos que interactúan con Oracle
  /// </summary>
  [History("12/07/2007", "Publico el método UpdateFromDataSet2")]
  [History("07/11/2006", "Creado el ensamblado Oracle.Utils")]
  public sealed class OracleProvider : IDisposable
  {
    #region Fields
    private OracleConnection oConnection;
    private OracleTransaction oTransaction;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructor de la clase
    /// </summary>
    /// <param name="connectionString">Cadena de Conexión</param>
    public OracleProvider(string connectionString)
    {
      oConnection = new OracleConnection(connectionString);
      oConnection.Open();
    }
    #endregion

    #region IDisposable Members
    /// <summary>
    /// Método que libera los objetos y la conexión con Oracle
    /// en caso de estar abierta
    /// </summary>
    public void Dispose()
    {
      if (oConnection != null)
      {
        if (oConnection.State == ConnectionState.Open) oConnection.Close();
        oConnection.Dispose();
      }
    }

    #endregion

    #region Private Members

    /// <summary>
    /// Este método es usado para adjuntar el arreglo de OracleParameters al OracleCommand.
    /// 
    /// Asigna DbNull a cualquier parámetro con dirección
    /// InputOutput y un valor de null.  
    /// </summary>
    /// <param name="command">Comando al que se adjuntarán los OracleParameters</param>
    /// <param name="commandParameters">Arreglo de OracleParameters a adicionar al OracleCommand</param>
    private static void AttachParameters(OracleCommand command, OracleParameter[] commandParameters)
    {
      foreach (OracleParameter p in commandParameters)
      {
        if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
          p.Value = DBNull.Value;
        command.Parameters.Add(p);
      }
    }

    #endregion

    #region ExecuteNonQuery
    private int ExecuteNonQuery(string commandText, CommandType commandType, OracleParameter[] parameters)
    {
      OracleCommand cmd = new OracleCommand(commandText, oConnection);
      cmd.CommandType = commandType;
      AttachParameters(cmd, parameters);
      return cmd.ExecuteNonQuery();
    }
    
    /// <summary>
    /// Ejecuta una instrucción SQL y devuelve el número de filas afectadas. 
    /// </summary>
    /// <param name="commandText">Instrucción SQL.</param>
    /// <param name="parameters">Colección de parámetros OracleParameter</param>
    /// <returns>El valor devuelto corresponde al número de filas afectadas por el comando.</returns>
    public int ExecuteNonQuery(string commandText, params OracleParameter[] parameters)
    {
      return ExecuteNonQuery(commandText, CommandType.Text, parameters);
    }
    
    /// <summary>
    /// Ejecuta una instrucción SQL y devuelve el número de filas afectadas. 
    /// </summary>
    /// <param name="spName">Nombre del procedimiento almacenado</param>
    /// <param name="parameters">Colección de parámetros OracleParameter</param>
    /// <returns>-1</returns>
    public int ExecuteNonQueryStoreProc(string spName, params OracleParameter[] parameters)
    {
      return ExecuteNonQuery(spName, CommandType.StoredProcedure, parameters);
    }
    #endregion

    #region ExecuteDataSet
    private DataSet ExecuteDataSet(string commandText, CommandType commandType, OracleParameter[] parameters)
    {
      DataSet result = new DataSet("REGISTROS");
      OracleCommand cmd = new OracleCommand(commandText, oConnection);
      cmd.CommandType = commandType;
      AttachParameters(cmd, parameters);
      OracleDataAdapter oda = new OracleDataAdapter(cmd);
      oda.MissingSchemaAction = MissingSchemaAction.AddWithKey;
      oda.Fill(result);
      return result;
    }

    /// <summary>
    /// Ejecuta una instrucción SQL y devuelve el conjunto de filas. 
    /// </summary>
    /// <param name="commandText">Instrucción SQL. (SELECT)</param>
    /// <param name="parameters">Colección de parámetros OracleParameter</param>
    /// <returns>El valor devuelto corresponde al conjunto de datos seleccionados (SELECT).</returns>
    public DataSet ExecuteDataSet(string commandText, params OracleParameter[] parameters)
    {
      return ExecuteDataSet(commandText, CommandType.Text, parameters);
    }

    /// <summary>
    /// Ejecuta una instrucción SQL y devuelve el conjunto de filas. 
    /// </summary>
    /// <param name="spName">Nombre del procedimiento almacenado</param>
    /// <param name="parameters">Colección de parámetros OracleParameter</param>
    /// <returns>El valor devuelto corresponde al conjunto de datos seleccionados (CURSORES).</returns>
    public DataSet ExecuteDataSetStoreProc(string spName, params OracleParameter[] parameters)
    {
      return ExecuteDataSet(spName, CommandType.StoredProcedure, parameters);
    } 
    #endregion

    #region ExecuteReader
    private OracleDataReader ExecuteReader(string commandText, CommandType commandType, OracleParameter[] parameters)
    {
      OracleCommand cmd = new OracleCommand(commandText, oConnection);
      cmd.CommandType = commandType;
      AttachParameters(cmd, parameters);
      OracleDataReader dr = cmd.ExecuteReader();
      return dr;
    }

    /// <summary>
    /// Ejecuta commandText y genera un objeto OracleDataReader
    /// </summary>
    /// <param name="commandText">Instrucción SQL. (SELECT)</param>
    /// <param name="parameters">Colección de parámetros OracleParameter</param>
    /// <returns>Objeto OracleDataReader</returns>
    public OracleDataReader ExecuteReader(string commandText, params OracleParameter[] parameters)
    {
      return ExecuteReader(commandText, CommandType.Text, parameters);
    }

    /// <summary>
    /// Ejecuta un procedimiento almacenado y genera un objeto OracleDataReader
    /// </summary>
    /// <param name="spName">Nombre del procedimiento almacenado</param>
    /// <param name="parameters">Colección de parámetros OracleParameter</param>
    /// <returns>Objeto OracleDataReader</returns>
    public OracleDataReader ExecuteReaderStoreProc(string spName, params OracleParameter[] parameters)
    {
      return ExecuteReader(spName, CommandType.StoredProcedure, parameters);
    } 
    #endregion

    #region ExecuteScalar
    private object ExecuteScalar(string commandText, CommandType commandType, OracleParameter[] parameters)
    {
      OracleCommand cmd = new OracleCommand(commandText, oConnection);
      cmd.CommandType = commandType;
      AttachParameters(cmd, parameters);
      return cmd.ExecuteScalar();
    }

    /// <summary>
    /// Ejecuta la consulta y devuelve la primera columna de la primera fila del conjunto 
    /// de resultados devuelto por la consulta como un tipo de datos de .NET Framework. 
    /// Las demás columnas o filas no se tienen en cuenta. 
    /// </summary>
    /// <param name="commandText">Instrucción SQL. (SELECT)</param>
    /// <param name="parameters">Colección de parámetros OracleParameter</param>
    /// <returns>Un objeto.</returns>
    public object ExecuteScalar(string commandText, params OracleParameter[] parameters)
    {
      return ExecuteScalar(commandText, CommandType.Text, parameters);
    }

    /// <summary>
    /// Ejecuta un procedimiento almacenado y devuelve la primera columna de la primera fila del conjunto 
    /// de resultados devuelto por la consulta como un tipo de datos de .NET Framework. 
    /// Las demás columnas o filas no se tienen en cuenta. 
    /// </summary>
    /// <param name="spName">Nombre del procedimiento almacenado</param>
    /// <param name="parameters">Colección de parámetros OracleParameter</param>
    /// <returns>Un objeto.</returns>
    public object ExecuteScalarStoreProc(string spName, params OracleParameter[] parameters)
    {
      return ExecuteScalar(spName, CommandType.StoredProcedure, parameters);
    } 
    #endregion

    #region ExecuteXmlReader
    private XmlReader ExecuteXmlReader(string commandText, CommandType commandType, OracleParameter[] parameters)
    {
      OracleCommand cmd = new OracleCommand(commandText, oConnection);
      cmd.CommandType = commandType;
      AttachParameters(cmd, parameters);
      return cmd.ExecuteXmlReader();
    }

    /// <summary>
    /// Ejecuta commandText y genera un objeto XmlReader
    /// </summary>
    /// <param name="commandText">Instrucción SQL. (SELECT)</param>
    /// <param name="parameters">Colección de parámetros OracleParameter</param>
    /// <returns>Objeto XmlReader</returns>
    public XmlReader ExecuteXmlReader(string commandText, params OracleParameter[] parameters)
    {
      return ExecuteXmlReader(commandText, CommandType.Text, parameters);
    }

    /// <summary>
    /// Ejecuta el procedimiento almacenado y genera un objeto XmlReader
    /// </summary>
    /// <param name="spName">Nombre del procedimiento almacenado</param>
    /// <param name="parameters">Colección de parámetros OracleParameter</param>
    /// <returns>Objeto XmlReader</returns>
    public XmlReader ExecuteXmlReaderStoreProc(string spName, params OracleParameter[] parameters)
    {
      return ExecuteXmlReader(spName, CommandType.StoredProcedure, parameters);
    } 
    #endregion

    #region ExecuteDataTable
    private DataTable ExecuteDataTable(string commandText, CommandType commandType, OracleParameter[] parameters)
    {
      DataTable result = new DataTable();
      OracleCommand cmd = new OracleCommand(commandText, oConnection);
      cmd.CommandType = commandType;
      AttachParameters(cmd, parameters);
      OracleDataAdapter oda = new OracleDataAdapter(cmd);
      oda.MissingMappingAction = MissingMappingAction.Passthrough;
      oda.MissingSchemaAction = MissingSchemaAction.AddWithKey;
      oda.Fill(result);
      return result;
    }

    /// <summary>
    /// Ejecuta commandText y genera un objeto DataTable
    /// </summary>
    /// <param name="commandText">Instrucción SQL. (SELECT)</param>
    /// <param name="parameters">Colección de parámetros OracleParameter</param>
    /// <returns>Objeto DataTable</returns>
    public DataTable ExecuteDataTable(string commandText, params OracleParameter[] parameters)
    {
      return ExecuteDataTable(commandText, CommandType.Text, parameters);
    }

    /// <summary>
    /// Ejecuta el procedimiento almacenado y genera un objeto DataTable
    /// </summary>
    /// <param name="spName">Nombre del procedimiento almacenado</param>
    /// <param name="parameters">Colección de parámetros OracleParameter</param>
    /// <returns>Objeto DataTable</returns>
    public DataTable ExecuteDataTableStoreProc(string spName, params OracleParameter[] parameters)
    {
      return ExecuteDataTable(spName, CommandType.StoredProcedure, parameters);
    }
    #endregion

    #region ExecuteQuery
    /// <summary>
    /// Ejecuta una consulta
    /// </summary>
    /// <param name="cmdText">Instrucción SQL. (SELECT, INSERT, UPDATE, DELETE)</param>
    /// <param name="qType">Tipo de Consulta (SELECT, INSERT, UPDATE, DELETE)</param>
    /// <param name="parameters">Colección de parámetros OracleParameter</param>
    /// <returns>Objeto QueryResponse</returns>
    public QueryResponse ExecuteQuery(string cmdText, QueryType qType, params OracleParameter[] parameters)
    {
      QueryResponse Result = new QueryResponse();
      switch (qType)
      {
        case QueryType.Select: //Select
          Result.Registros = ExecuteDataSet(cmdText, parameters);
          Result.CantidadRegistros = Result.Registros.Tables[0].Rows.Count;
          break;
        default: //Insert, Update and Delete
          Result.CantidadRegistros = ExecuteNonQuery(cmdText, parameters);
          break;
      } 
      return Result;
    }
    
    /// <summary>
    /// Ejecuta una consulta, preferiblemente SELECT
    /// </summary>
    /// <param name="cmdText">Instrucción SQL. (SELECT)</param>
    /// <param name="parameters">Colección de parámetros OracleParameter</param>
    /// <returns>Objeto DataSet con el resultado de la consulta</returns>
    public DataSet ExecuteQuery(string cmdText, params OracleParameter[] parameters)
    {
      return ExecuteDataSet(cmdText, parameters);
    }
    #endregion
    
    #region Control Transaction
    private void ReleaseTransaction()
    {
      oTransaction.Dispose();
      oTransaction = null;
    }
    
    /// <summary>
    /// Inicia una transacción en la base de datos. 
    /// </summary>
    public void BeginTransaction()
    {
      if (oTransaction != null) throw new Exception("Transacción ya iniciada.");
      oTransaction = oConnection.BeginTransaction();
    }

    /// <summary>
    /// Confirma la transacción en la base de datos SQL. 
    /// </summary>
    public void CommitTransaction()
    {
      if (oTransaction != null)
      {
        oTransaction.Commit();
        ReleaseTransaction();
      }
    }

    /// <summary>
    /// Deshace una transacción desde un estado pendiente. 
    /// </summary>
    public void RollBackTransaction()
    {
      if (oTransaction != null)
      {
        oTransaction.Rollback();
        ReleaseTransaction();
      }
    } 
    #endregion

    #region Update Member's
    /// <summary>
    /// <para>Actualiza datos en el Servidor Oracle a partir</para>
    /// <para>de un DataSet en el orden de la colección Tables.</para>
    /// </summary>
    /// <param name="ds">Fuente de Datos</param>
    public void UpdateFromDataSet2(DataSet ds)
    {
      try
      {
        BeginTransaction();
        foreach (DataTable dt in ds.Tables)
        {
          OracleDataAdapter da = new OracleDataAdapter();
          using (new CommandBuilder(oConnection, da, dt))
            da.Update(ds, dt.TableName);
        }
      }
      catch (Exception ex)
      {
        RollBackTransaction();
        throw new DataBaseException(ex.Message);
      }
      finally
      {
        CommitTransaction();
      }
    }

    /// <summary>
    /// <para>Actualiza datos en el Servidor Oracle a partir</para>
    /// <para>de un DataTable.</para>
    /// </summary>
    /// <param name="dt">Tabla fuente de Datos</param>
    /// <param name="keyString">Campos WHERE de actualización</param>
    public void UpdateFromDataTable2(DataTable dt, string[] keyString)
    {
      try
      {
        BeginTransaction();
        OracleDataAdapter da = new OracleDataAdapter();
        using (new CommandBuilder(oConnection, da, dt, keyString))
          da.Update(dt);
      }
      catch (Exception ex)
      {
        RollBackTransaction();
        throw new DataBaseException(ex.Message);
      }
      finally
      {
        CommitTransaction();
      }
    }

    /// <summary>
    /// <para>Actualiza datos en el Servidor Oracle a partir</para>
    /// <para>de un DataSet en el orden especificador en Tables.</para>
    /// </summary>
    /// <param name="ds">Fuente de Datos</param>
    /// <param name="Tables">Tablas del DataSet a actualizar en Oracle</param>
    public void UpdateFromDataSet(DataSet ds, params string[] Tables)
    {
      DataSet dsSend = new DataSet();
      foreach (string tableName in Tables)
      {
        if (ds.Tables.IndexOf(tableName.ToUpper()) != -1)
        {
          DataTable dt = ds.Tables[tableName.ToUpper()].GetChanges();
          if (dt != null && dt.Rows.Count != 0) dsSend.Tables.Add(dt);
        }
      }
      if (dsSend.Tables.Count != 0) UpdateFromDataSet2(dsSend);
    }

    /// <summary>
    /// <para>Actualiza datos en el Servidor Oracle a partir</para>
    /// <para>de un DataTable.</para>
    /// </summary>
    /// <param name="dt">Tabla fuente de Datos</param>
    /// <param name="KeyString">Campos WHERE de actualización</param>
    public void UpdateFromDataTable(DataTable dt, params string[] KeyString)
    {
      DataTable dttmp = dt.GetChanges();
      if (dttmp != null && dttmp.Rows.Count != 0) UpdateFromDataTable2(dttmp, KeyString);
    }

    /// <summary>
    /// <para>Actualiza datos en el Servidor Oracle a partir</para>
    /// <para>de un DataSet en el orden de la colección Tables</para>
    /// <para>y luego ejecuta un procedimiento almacenado.</para>
    /// </summary>
    /// <param name="ds">Fuente de Datos</param>
    /// <param name="spName">Nombre del procedimiento almacenado</param>
    /// <param name="parameters">Colección de parámetros OracleParameter</param>
    public void UpdateFromDataSetStoreProc2(DataSet ds, string spName, params OracleParameter[] parameters)
    {
      try
      {
        BeginTransaction();
        foreach (DataTable dt in ds.Tables)
        {
          OracleDataAdapter da = new OracleDataAdapter();
          using (new CommandBuilder(oConnection, da, dt))
            da.Update(ds, dt.TableName);
        }
        ExecuteNonQueryStoreProc(spName, parameters);
      }
      catch (Exception ex)
      {
        RollBackTransaction();
        throw new DataBaseException(ex.Message);
      }
      finally
      {
        CommitTransaction();
      }
    }

    /// <summary>
    /// <para>Ejecuta un procedimiento almacenado y luego</para>
    /// <para>actualiza datos en el Servidor Oracle a partir</para>
    /// <para>de un DataSet en el orden de la colección Tables</para>
    /// </summary>
    /// <param name="ds">Fuente de Datos</param>
    /// <param name="spName">Nombre del procedimiento almacenado</param>
    /// <param name="parameters">Colección de parámetros OracleParameter</param>
    public void StoreProcUpdateFromDataSet(DataSet ds, string spName, params OracleParameter[] parameters)
    {
      try
      {
        BeginTransaction();
        ExecuteNonQueryStoreProc(spName, parameters);
        foreach (DataTable dt in ds.Tables)
        {
          OracleDataAdapter da = new OracleDataAdapter();
          using (new CommandBuilder(oConnection, da, dt))
            da.Update(ds, dt.TableName);
        }
      }
      catch (Exception ex)
      {
        RollBackTransaction();
        throw new DataBaseException(ex.Message);
      }
      finally
      {
        CommitTransaction();
      }
    }
    #endregion
  }
}
