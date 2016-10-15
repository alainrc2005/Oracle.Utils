/*-----------------------------------------------*
 * Convertido de Oracle Project Raptor (Java)    *
 * por Alain Ramírez Cabrejas            *
 * Camagüey, Ciudad de los Tinajones.            *
 * Septiembre 2006                               *
 *-----------------------------------------------*
 */
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using Oracle.Utils.TNSParser;

namespace Oracle.Utils
{
  /// <summary>
  /// Clase auxiliar para el trabajo con el TNSNAMES.ORA y el Home de Oracle
  /// </summary>
  public sealed class TNSandHome
  {
    /// <summary>
    /// Captura los nodos de Oracle Home definidos en el registro de Windows
    /// </summary>
    /// <returns>Lista genérica de OracleHomeNode</returns>
    public static List<OracleHomeNode> GetHomes()
    {
      List<OracleHomeNode> oracleList = new List<OracleHomeNode>();
      using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\ORACLE"))
      {
        if (key != null)
        {
          OracleHomeNode oh = processRegistry(key, true);
          if (oh != null) oracleList.Add(oh);
          string upperKey;
          foreach (string subkey in key.GetSubKeyNames())
          {
            upperKey = subkey.ToUpper();
            if (upperKey.StartsWith("HOME") || upperKey.StartsWith("KEY_"))
            {
              using (RegistryKey sub = key.OpenSubKey(upperKey))
              {
                oh = processRegistry(sub, false);
                if (oh != null) oracleList.Add(oh);
              }
            }
          }
        }
      }
      return oracleList;
    }

    /// <summary>
    /// Captura todos los Alias de un TNSNAMES.ORA a partir de OracleHomeNode
    /// </summary>
    /// <param name="oracleHomeNode"></param>
    /// <returns></returns>
    public static string[] GetAlias(OracleHomeNode oracleHomeNode)
    {
      return GetAlias(oracleHomeNode.OracleHome);
    }
    
    /// <summary>
    /// Captura todos los Alias de un TNSNAMES.ORA a partir de un PATH determinado
    /// </summary>
    /// <param name="oracleHomePath"></param>
    /// <returns></returns>
    public static string[] GetAlias(string oracleHomePath)
    {
      NLParamParser tnsParser = new NLParamParser(Path.Combine(oracleHomePath,"NETWORK\\ADMIN\\TNSNAMES.ORA"));
      return tnsParser.NLPAllNames;
    }

    /// <summary>
    /// Captura todos los Alias de un TNSNAMES.ORA a partir de un PATH determinado
    /// </summary>
    /// <param name="oracleHomePath"></param>
    /// <returns></returns>
    public static NLParamParser GetAliasComplete(string oracleHomePath)
    {
      NLParamParser tnsParser = new NLParamParser(Path.Combine(oracleHomePath, "NETWORK\\ADMIN\\TNSNAMES.ORA"));
      return tnsParser;
    }
    
    #region Private Methods
    private static OracleHomeNode processRegistry(RegistryKey key, bool root)
    {
      if (key.GetValue("ORACLE_HOME") == null) return null;
      return new OracleHomeNode((string) key.GetValue("ORACLE_HOME"),
                                (string) key.GetValue("ORACLE_HOME_NAME"),
                                (string) key.GetValue("ORACLE_SID"),
                                (string) key.GetValue("NLS_LANG"), root);
    }
    #endregion
  }
  
  /// <summary>
  /// Clase que representa un Home de Oracle
  /// </summary>
  public sealed class OracleHomeNode
  {
    #region Private Fields
    private string oracleHomeName;
    private string oracleHome;
    private string oracleSID;
    private string nlsLang;
    private bool valid;
    private bool root;
    private string clientversion;
    #endregion

    #region Constructor
    internal OracleHomeNode(string oracleHome, string oracleHomeName, string oracleSID, string nlsLang, bool root)
    {
      this.oracleHome = oracleHome;
      this.oracleHomeName = oracleHomeName;
      this.oracleSID = oracleSID;
      this.nlsLang = nlsLang;
      this.root = root;
      valid = Directory.Exists(oracleHome);
      clientversion = "";
      if (valid)
      {
        try
        {
          FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(oracleHome + "\\Bin\\OCI.DLL");
          clientversion = fvi.FileVersion;
        }
        catch { }
      }
    }
    #endregion
    
    #region Properties
    /// <summary>
    /// Propiedad que retorna el nombre del home oracle
    /// </summary>
    public string OracleHomeName
    {
      get { return oracleHomeName; }
    }

    /// <summary>
    /// Propiedad que retorna el PATH del home del oracle
    /// </summary>
    public string OracleHome
    {
      get { return oracleHome; }
    }

    /// <summary>
    /// Propiedad que retorna el SID de la base que representa el home
    /// </summary>
    public string OracleSID
    {
      get { return oracleSID; }
    }

    /// <summary>
    /// Propiedad que retorna el lenguaje asociado al home oracle
    /// </summary>
    public string NLSLang
    {
      get { return nlsLang; }
    }

    /// <summary>
    /// Propiedad que retorna la versión del cliente OCI.DLL
    /// </summary>
    public string ClientVersion
    {
      get { return clientversion; }
    }
    
    /// <summary>
    /// Propiedad que retorna si es válido del OracleHome
    /// </summary>
    public bool isValid
    {
      get { return valid; }
    }

    /// <summary>
    /// Propiedad que retorna si es el root home oracle
    /// </summary>
    public bool isRoot
    {
      get { return root; }
    }
    #endregion
  }

}