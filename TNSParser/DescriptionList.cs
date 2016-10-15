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
  public class DescriptionList : SchemaObject
  {
    /// <remarks/>
    public DescriptionList(SchemaObjectFactoryInterface schemaobjectfactoryinterface)
    {
      children = ArrayList.Synchronized(new ArrayList(10));
      sourceRoute = false;
      loadBalance = true;
      failover = true;
      f = null;
      f = schemaobjectfactoryinterface;
    }

    /// <remarks/>
    public virtual int isA()
    {
      return 3;
    }

    /// <remarks/>
    public virtual String isA_String()
    {
      return "DESCRIPTION_LIST";
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
        if (childnv.Name.ToUpper().Equals("DESCRIPTION".ToUpper()))
        {
          child = f.create(2);
          child.initFromNVPair(childnv);
          children.Add(child);
        }
        else
        {
          throw new TnsException();
        }
      }

      if (children.Count == 0) throw new TnsException();
      else
        return;
    }

    /// <remarks/>
    public override String ToString()
    {
      String s = "";
      if (children.Count < 1)
        return s;
      for (int i = 0; i < children.Count; i++)
      {
        String s2 = ((SchemaObject) children[i]).ToString();
        if (!s2.Equals("")) s = s + s2;
      }

      if (s.Equals("") && sourceRoute)
        s = s + "(SOURCE_ROUTE=yes)";
      if (s.Equals("") && !loadBalance)
        s = s + "(LOAD_BALANCE=no)";
      if (s.Equals("") && !failover)
        s = s + "(FAILOVER=false)";
      if (!s.Equals(""))
        s = "(DESCRIPTION_LIST=" + s + ")";
      return s;
    }

    /// <remarks/>
    protected internal virtual void init()
    {
      children.Clear();
      child = null;
      childnv = null;
      sourceRoute = false;
      loadBalance = true;
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