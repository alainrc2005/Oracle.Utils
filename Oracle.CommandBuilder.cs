/*-----------------------------------------------*
 * Programado por Alain Ramírez Cabrejas         *
 * Camagüey, Ciudad de los Tinajones.            *
 * Julio 2006                                    *
 *-----------------------------------------------*
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Oracle.DataAccess.Client;

namespace Oracle.Utils
{
  /// <summary>
  /// Clase para la generación de comandos Update, Insert y Delete a partir de un DataTable
  /// </summary>
  public sealed class CommandBuilder : IDisposable
  {
    private OracleCommand cmdInsert;
    private StringBuilder sqlColumns;
    private StringBuilder sqlValues;
    private StringBuilder sqlUpdateFields;
    private List<OracleParameter> cmdUpdateParameters;
    private readonly List<OracleParameter> parametersDelete;
    private readonly List<OracleParameter> parametersUpdate;
    private string sqlWhere;

    /// <summary>
    /// Constructor en caso de no existir keyString entonces busca las llaves
    /// primarias de forma automatica
    /// </summary>
    /// <param name="oc">Conexión con el oracle</param>
    /// <param name="dataAdapter">Adaptador de datos</param>
    /// <param name="dataTable">Tabla</param>
    /// <param name="keyString">Llave primaria candidata</param>
    public CommandBuilder(OracleConnection oc, OracleDataAdapter dataAdapter, DataTable dataTable, string[] keyString)
    {
      buildParameters(dataTable);

      DataTable changes = dataTable.GetChanges(DataRowState.Added);
      // Insert Command
      if (changes != null)
      {
        changes.Dispose();
        cmdInsert.Connection = oc;
        cmdInsert.CommandText = String.Format("INSERT INTO {0} ({1}) VALUES ({2})", dataTable.TableName, sqlColumns, sqlValues);
        dataAdapter.InsertCommand = cmdInsert;
      }
      parametersDelete = new List<OracleParameter>();
      parametersUpdate = new List<OracleParameter>();
      //if (keyString == null || keyString.Length == 0)
      //{
      //  if (dataTable.Constraints.Count == 0)
      //    throw new DataBaseException("Imposible construir comando Delete y Update, 0 restricciones.");
      //  sqlWhere = FindWhereCondition(dataTable);
      //}
      //else sqlWhere = FindWhereCondition(keyString, dataTable);

      changes = dataTable.GetChanges(DataRowState.Deleted);
      // Delete Command
      if (changes != null)
      {
        changes.Dispose();
        FindWhere(dataTable, keyString);
        OracleCommand cmdDelete = new OracleCommand();
        cmdDelete.Connection = oc;
        cmdDelete.CommandText = string.Format("DELETE FROM {0} WHERE {1}", dataTable.TableName, sqlWhere);
        foreach (OracleParameter op in parametersDelete)
          cmdDelete.Parameters.Add(op);
        dataAdapter.DeleteCommand = cmdDelete;
      }
      changes = dataTable.GetChanges(DataRowState.Modified);
      // Update Command
      if (changes != null)
      {
        changes.Dispose();
        if (String.IsNullOrEmpty(sqlWhere)) FindWhere(dataTable, keyString);
        OracleCommand cmdUpdate = new OracleCommand();
        cmdUpdate.Connection = oc;
        cmdUpdate.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2}", dataTable.TableName, sqlUpdateFields, sqlWhere);
        foreach (OracleParameter op in cmdUpdateParameters)
          cmdUpdate.Parameters.Add(op);
        foreach (OracleParameter op in parametersUpdate)
          cmdUpdate.Parameters.Add(op);
        dataAdapter.UpdateCommand = cmdUpdate;
      }
    }

    /// <summary>
    /// Constructor que llama al constructor anterior pero con keyString null
    /// </summary>
    /// <param name="oc">Conexión con el oracle</param>
    /// <param name="dataAdapter">Adaptador de datos</param>
    /// <param name="dataTable">Tabla</param>
    public CommandBuilder(OracleConnection oc, OracleDataAdapter dataAdapter, DataTable dataTable)
      : this(oc, dataAdapter, dataTable, null)
    {
    }

    #region Private Methods
    private static OracleParameter CreateParameter(DataColumn dc, bool original)
    {
      OracleParameter op = new OracleParameter();
      op.OracleDbType = OracleNative.FrameworkToOracle(dc);
      op.ParameterName = dc.ColumnName;
      op.SourceColumn = dc.ColumnName;
      if (original)
      {
        op.SourceVersion = DataRowVersion.Current;
        op.ParameterName += "_";
      }
      return op;
    }

    private void buildParameters(DataTable dataTable)
    {
      sqlColumns = new StringBuilder();
      sqlValues = new StringBuilder();
      sqlUpdateFields = new StringBuilder();
      cmdInsert = new OracleCommand();
      cmdUpdateParameters = new List<OracleParameter>();
      foreach (DataColumn dc in dataTable.Columns)
      {
        sqlColumns.Append(dc.ColumnName + ",");
        sqlValues.Append(":" + dc.ColumnName + ",");
        sqlUpdateFields.Append(dc.ColumnName + "=:" + dc.ColumnName + ",");
        cmdInsert.Parameters.Add(CreateParameter(dc, false));
        cmdUpdateParameters.Add(CreateParameter(dc, false));
      }
      sqlColumns = sqlColumns.Remove(sqlColumns.Length - 1, 1);
      sqlValues = sqlValues.Remove(sqlValues.Length - 1, 1);
      sqlUpdateFields = sqlUpdateFields.Remove(sqlUpdateFields.Length - 1, 1);
    }

    private string FindWhereCondition(DataTable dataTable)
    {
      StringBuilder Where = new StringBuilder();
      UniqueConstraint constraintKey = FindConstraintKey(dataTable);
      if (constraintKey != null)
      {
        foreach (DataColumn dc in constraintKey.Columns)
        {
          Where.Append(dc.ColumnName + "=:" + dc.ColumnName + "_ AND ");
          parametersDelete.Add(CreateParameter(dc, true));
          parametersUpdate.Add(CreateParameter(dc, true));
        }
      }
      else
      {
        throw new ArgumentException("No existen restricciones en esta tabla");
      }
      return Where.Remove(Where.Length - 5, 5).ToString();
    }

    private string FindWhereCondition(IEnumerable<string> keyString, DataTable dataTable)
    {
      StringBuilder Where = new StringBuilder();
      foreach (string key in keyString)
      {
        int index = dataTable.Columns.IndexOf(key);
        if (index != -1)
        {
          Where.Append(dataTable.Columns[index].ColumnName + "=:" + dataTable.Columns[index].ColumnName + "_ AND ");
          parametersDelete.Add(CreateParameter(dataTable.Columns[index], true));
          parametersUpdate.Add(CreateParameter(dataTable.Columns[index], true));
        }
      }
      if (Where.Length == 0) throw new ArgumentException("No existen los campos llaves seleccionados");
      return Where.Remove(Where.Length - 5, 5).ToString();
    }

    private void FindWhere(DataTable dataTable, string[] keyString)
    {
      if (keyString == null || keyString.Length == 0)
      {
        if (dataTable.Constraints.Count == 0)
          throw new DataBaseException("Imposible construir comando Delete y Update, 0 restricciones.");
        sqlWhere = FindWhereCondition(dataTable);
      }
      else sqlWhere = FindWhereCondition(keyString, dataTable);
    }

    private static UniqueConstraint FindConstraintKey(DataTable table)
    {
      UniqueConstraint Result = null;
      foreach (Constraint cs in table.Constraints)
      {
        UniqueConstraint uk = cs as UniqueConstraint;
        if (uk != null)
        {
          Result = uk;
          if (uk.IsPrimaryKey) break;
        }
      }
      return Result;
    }
    #endregion

    #region IDisposable Members

    /// <summary>
    /// Libera los objetos utilizados internamente.
    /// </summary>
    public void Dispose()
    {
      Utils.DisposeAndNull(cmdInsert);
      Utils.ClearAndNull(cmdUpdateParameters, parametersDelete, parametersUpdate);
    }

    #endregion
  }

}
