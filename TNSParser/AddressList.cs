/*-----------------------------------------------*
 * Convertido de Oracle Project Raptor (Java)    *
 * por Alain Ramírez Cabrejas            *
 * Camagüey, Ciudad de los Tinajones.            *
 * Septiembre 2006                               *
 *-----------------------------------------------*
 */
using System;
using System.Collections;
using System.Text;

namespace Oracle.Utils.TNSParser
{

  /// <remarks/>
  public class AddressList : SchemaObject
  {
    /// <remarks/>
    public AddressList(SchemaObjectFactoryInterface schemaobjectfactoryinterface)
    {
      children = ArrayList.Synchronized(new ArrayList(10));
      sourceRoute = false;
      loadBalance = false;
      failover = true;
      f = null;
      f = schemaobjectfactoryinterface;
    }

    /// <remarks/>
    public virtual int isA()
    {
      return 1;
    }

    /// <remarks/>
    public virtual String isA_String()
    {
      return "ADDRESS_LIST";
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
      if (i == 0) throw new Exception();
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
        }
        else
        {
          throw new Exception();
        }
      }

      if (children.Count == 0) throw new Exception();
      else return;
    }

    /// <remarks/>
    public override String ToString()
    {
      String s = new StringBuilder("").ToString();
      if (children.Count < 1)
        return s;
      s = s + "(ADDRESS_LIST=";
      for (int i = 0; i < children.Count; i++)
      {
        s = s + ((SchemaObject) children[i]).ToString();
      }

      if (sourceRoute)
        s = s + "(SOURCE_ROUTE=yes)";
      if (loadBalance)
        s = s + "(LOAD_BALANCE=yes)";
      if (!failover)
        s = s + "(FAILOVER=false)";
      s = s + ")";
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
    }

    /// <remarks/>
    public ArrayList children;
    private SchemaObject child;
    private NVPair childnv;
    /// <remarks/>
    public bool sourceRoute;
    /// <remarks/>
    public bool loadBalance;
    /// <remarks/>
    public bool failover;
    /// <remarks/>
    protected internal SchemaObjectFactoryInterface f;
  }
}