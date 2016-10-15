/*-----------------------------------------------*
 * Convertido de Oracle Project Raptor (Java)    *
 * por Alain Ramírez Cabrejas            *
 * Camagüey, Ciudad de los Tinajones.            *
 * Septiembre 2006                               *
 *-----------------------------------------------*
 */
using System;
using System.Collections;

namespace Oracle.Utils.TNSParser
{

  /// <remarks/>
  public class Description : SchemaObject
  {
    /// <remarks/>
    public Description(SchemaObjectFactoryInterface schemaobjectfactoryinterface)
    {
      children = ArrayList.Synchronized(new ArrayList(10));
      f = null;
      sourceRoute = false;
      loadBalance = false;
      failover = true;
      keepAlive = false;
      protocolStacks = ArrayList.Synchronized(new ArrayList(10));
      authParams = ArrayList.Synchronized(new ArrayList(10));
      extraConnInfo = ArrayList.Synchronized(new ArrayList(10));
      extraInfo = ArrayList.Synchronized(new ArrayList(10));
      f = schemaobjectfactoryinterface;
    }

    /// <remarks/>
    public virtual int isA()
    {
      return 2;
    }

    /// <remarks/>
    public virtual String isA_String()
    {
      return "DESCRIPTION";
    }

    /// <remarks/>
    public virtual void initFromString(String s)
    {
      NVPair nvpair = (new NVFactory()).createNVPair(s);
      initFromNVPair(nvpair);
    }

    /// <remarks/>
    public virtual void initFromNVPair(NVPair nvpair)
    {
      init();
      int i = nvpair.ListSize;
      if (i == 0) throw new TnsException();
      for (int j = 0; j < i; j++)
      {
        childnv = nvpair.getListElement(j);
        if (childnv.Name.ToUpper().Equals("SOURCE_ROUTE".ToUpper()))
        {
          sourceRoute = childnv.Atom.ToUpper().Equals("yes".ToUpper()) || childnv.Atom.ToUpper().Equals("on".ToUpper()) ||
                        childnv.Atom.ToUpper().Equals("true".ToUpper());
          continue;
        }
        if (childnv.Name.ToUpper().Equals("LOAD_BALANCE".ToUpper()))
        {
          loadBalance = childnv.Atom.ToUpper().Equals("yes".ToUpper()) || childnv.Atom.ToUpper().Equals("on".ToUpper()) ||
                        childnv.Atom.ToUpper().Equals("true".ToUpper());
          continue;
        }
        if (childnv.Name.ToUpper().Equals("FAILOVER".ToUpper()))
        {
          failover = childnv.Atom.ToUpper().Equals("yes".ToUpper()) || childnv.Atom.ToUpper().Equals("on".ToUpper()) ||
                     childnv.Atom.ToUpper().Equals("true".ToUpper());
          continue;
        }
        if (childnv.Name.ToUpper().Equals("ENABLE".ToUpper()))
        {
          keepAlive = childnv.Atom.ToUpper().Equals("broken".ToUpper());
          continue;
        }
        if (childnv.Name.ToUpper().Equals("PROTOCOL_STACK".ToUpper()))
        {
          protocolStacks.Add(childnv.ToString());
          continue;
        }
        if (childnv.Name.ToUpper().Equals("ADDRESS".ToUpper()))
        {
          child = f.create(0);
          child.initFromNVPair(childnv);
          children.Add(child);
          continue;
        }
        if (childnv.Name.ToUpper().Equals("ADDRESS_LIST".ToUpper()))
        {
          child = f.create(1);
          child.initFromNVPair(childnv);
          children.Add(child);
          continue;
        }
        if (childnv.Name.ToUpper().Equals("SDU".ToUpper()))
        {
          SDU = childnv.Atom;
          continue;
        }
        if (childnv.Name.ToUpper().Equals("TDU".ToUpper()))
        {
          TDU = childnv.Atom;
          continue;
        }
        if (childnv.Name.ToUpper().Equals("SEND_BUF_SIZE".ToUpper()))
        {
          sendBufSize = childnv.Atom;
          continue;
        }
        if (childnv.Name.ToUpper().Equals("RECV_BUF_SIZE".ToUpper()))
        {
          receiveBufSize = childnv.Atom;
          continue;
        }
        if (childnv.Name.ToUpper().Equals("CONNECT_DATA".ToUpper()))
        {
          connectData = childnv.valueToString();
          int k = childnv.ListSize;
          if (k == 0) throw new TnsException();
          int i1 = 0;
          do
          {
            if (i1 >= k) continue;
            NVPair nvpair1 = childnv.getListElement(i1);
            if (nvpair1.Name.ToUpper().Equals("SID".ToUpper()))
              SID = nvpair1.Atom;
            else if (nvpair1.Name.ToUpper().Equals("SERVER".ToUpper()))
              server = nvpair1.Atom;
            else if (nvpair1.Name.ToUpper().Equals("SERVICE_NAME".ToUpper()))
              serviceName = nvpair1.Atom;
            else if (nvpair1.Name.ToUpper().Equals("INSTANCE_NAME".ToUpper()))
              instanceName = nvpair1.Atom;
            else if (nvpair1.Name.ToUpper().Equals("HANDLER_NAME".ToUpper()))
              handlerName = nvpair1.Atom;
            else if (nvpair1.Name.ToUpper().Equals("ORACLE_HOME".ToUpper()))
              oracleHome = nvpair1.Atom;
            else if (nvpair1.Name.ToUpper().Equals("FAILOVER_MODE".ToUpper()))
            {
              failoverMode = childnv.getListElement(i1).ToString();
            }
            else if (nvpair1.Name.ToUpper().Equals("INSTANCE_ROLE".ToUpper()))
            {
              instanceRole = nvpair1.Atom;
            }
            else
            {
              String s1 = nvpair1.ToString().Trim();
              s1 = s1.Substring(1, (s1.Length - 1) - (1));
              extraConnInfo.Add(s1);
            }
            i1++;
          } while (true);
        }
        if (childnv.Name.ToUpper().Equals("SECURITY".ToUpper()))
        {
          int l = childnv.ListSize;
          if (l == 0) throw new TnsException();
          int j1 = 0;
          do
          {
            if (j1 >= l)
            {
              continue;
            }
            NVPair nvpair2 = childnv.getListElement(j1);
            if (nvpair2.Name.ToUpper().Equals("AUTHENTICATION".ToUpper()))
            {
              authTypes = nvpair2.ToString();
            }
            if (nvpair2.Name.ToUpper().Equals("ssl_server_cert_dn".ToUpper()))
            {
              sslServerCertDN = nvpair2.Atom;
              if (sslServerCertDN != null && sslServerCertDN.StartsWith("\"") && sslServerCertDN.EndsWith("\""))
                sslServerCertDN = sslServerCertDN.Substring(1, (sslServerCertDN.Length - 1) - (1));
            }
            else
            {
              authParams.Add(nvpair2.ToString());
            }
            j1++;
          } while (true);
        }
        if (childnv.Name.ToUpper().Equals("HS".ToUpper()) && childnv.Atom == null)
          try
          {
            childnv.Atom = "OK";
          }
          catch
          {
          }
        String s = childnv.ToString().Trim();
        s = s.Substring(1, (s.Length - 1) - (1));
        extraInfo.Add(s);
      }
    }

    /// <remarks/>
    public override String ToString()
    {
      String s = "";
      for (int i = 0; i < children.Count; i++)
      {
        String s2 = ((SchemaObject) children[i]).ToString();
        if (!s2.Equals("")) s = s + s2;
      }

      if (!s.Equals("") && sourceRoute)
        s = s + "(SOURCE_ROUTE=yes)";
      if (!s.Equals("") && loadBalance)
        s = s + "(LOAD_BALANCE=yes)";
      if (!s.Equals("") && !failover)
        s = s + "(FAILOVER=false)";
      if (keepAlive)
        s = s + "(ENABLE=broken)";
      if (SDU != null)
        s = s + "(SDU=" + SDU + ")";
      if (TDU != null)
        s = s + "(TDU=" + TDU + ")";
      if (sendBufSize != null)
        s = s + "(SEND_BUF_SIZE=" + sendBufSize + ")";
      if (receiveBufSize != null)
        s = s + "(RECV_BUF_SIZE=" + receiveBufSize + ")";
      if (protocolStacks.Count != 0)
      {
        for (int j = 0; j < protocolStacks.Count; j++)
          s = s + ((String) protocolStacks[j]);
      }
      if (SID != null || server != null || serviceName != null || instanceName != null || handlerName != null ||
          extraConnInfo.Count != 0 || oracleHome != null)
      {
        s = s + "(CONNECT_DATA=";
        if (SID != null)
          s = s + "(SID=" + SID + ")";
        if (server != null)
          s = s + "(SERVER=" + server + ")";
        if (serviceName != null)
          s = s + "(SERVICE_NAME=" + serviceName + ")";
        if (instanceName != null)
          s = s + "(INSTANCE_NAME=" + instanceName + ")";
        if (handlerName != null)
          s = s + "(HANDLER_NAME=" + handlerName + ")";
        if (oracleHome != null)
          s = s + "(ORACLE_HOME=" + oracleHome + ")";
        if (instanceRole != null)
          s = s + "(INSTANCE_ROLE=" + instanceRole + ")";
        if (failoverMode != null)
          s = s + failoverMode;
        for (int k = 0; k < extraConnInfo.Count; k++)
          s = s + "(" + ((String) extraConnInfo[k]) + ")";

        s = s + ")";
      }
      if (authTypes != null || authParams.Count != 0)
      {
        s = s + "(SECURITY=";
        if (authTypes != null)
          s = s + "(AUTHENTICATION=" + authTypes + ")";
        for (int l = 0; l < authParams.Count; l++)
          s = s + ((String) authParams[l]);

        s = s + ")";
      }
      for (int i1 = 0; i1 < extraInfo.Count; i1++)
        s = s + "(" + ((String) extraInfo[i1]) + ")";

      if (!s.Equals(""))
        s = "(DESCRIPTION=" + s + ")";
      return s;
    }

    /// <remarks/>
    protected internal virtual void init()
    {
      children.Clear();
      child = null;
      childnv = null;
      sourceRoute = false;
      loadBalance = false;
      failover = true;
      keepAlive = false;
      protocolStacks.Clear();
      SDU = null;
      TDU = null;
      SID = null;
      server = null;
      serviceName = null;
      instanceName = null;
      handlerName = null;
      oracleHome = null;
      authTypes = null;
      sendBufSize = null;
      receiveBufSize = null;
      failoverMode = null;
      instanceRole = null;
      authParams.Clear();
      extraConnInfo.Clear();
      extraInfo.Clear();
    }

    /// <remarks/>
    public ArrayList children;
    private SchemaObject child;
    private NVPair childnv;
    /// <remarks/>
    protected internal SchemaObjectFactoryInterface f;
    /// <remarks/>
    public bool sourceRoute;
    /// <remarks/>
    public bool loadBalance;
    /// <remarks/>
    public bool failover;
    /// <remarks/>
    public bool keepAlive;
    /// <remarks/>
    public String SDU;
    /// <remarks/>
    public String TDU;
    /// <remarks/>
    public ArrayList protocolStacks;
    /// <remarks/>
    public String sendBufSize;
    /// <remarks/>
    public String receiveBufSize;
    /// <remarks/>
    public String connectData;
    /// <remarks/>
    public String SID;
    /// <remarks/>
    public String server;
    /// <remarks/>
    public String failoverMode;
    /// <remarks/>
    public String instanceRole;
    /// <remarks/>
    public String serviceName;
    /// <remarks/>
    public String instanceName;
    /// <remarks/>
    public String handlerName;
    /// <remarks/>
    public String oracleHome;
    /// <remarks/>
    public String authTypes;
    /// <remarks/>
    public String sslServerCertDN;
    /// <remarks/>
    public ArrayList authParams;
    /// <remarks/>
    public ArrayList extraConnInfo;
    /// <remarks/>
    public ArrayList extraInfo;
  }
}