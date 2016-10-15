/*-----------------------------------------------*
 * Convertido de Oracle Project Raptor (Java)    *
 * por Alain Ramírez Cabrejas            *
 * Camagüey, Ciudad de los Tinajones.            *
 * Septiembre 2006                               *
 *-----------------------------------------------*
 */
using System;

namespace Oracle.Utils.TNSParser
{

  /// <remarks/>
  public class ServiceAlias : SchemaObject
  {
    /// <remarks/>
    public ServiceAlias(SchemaObjectFactoryInterface schemaobjectfactoryinterface)
    {
      f = null;
      f = schemaobjectfactoryinterface;
    }

    /// <remarks/>
    public virtual int isA()
    {
      return 4;
    }

    /// <remarks/>
    public virtual String isA_String()
    {
      return null;
    }

    /// <remarks/>
    public virtual void initFromString(String s)
    {
      if (s[0] != '(')
        s = "(" + s + ")";
      NVPair nvpair = (new NVFactory()).createNVPair(s);
      initFromNVPair(nvpair);
    }

    /// <remarks/>
    public virtual void initFromNVPair(NVPair nvpair)
    {
      if (nvpair.ListSize != 1) throw new TnsException();
      NVPair nvpair1 = nvpair.getListElement(0);
      if (nvpair1.Name.ToUpper().Equals("DESCRIPTION_LIST".ToUpper()))
      {
        child = f.create(3);
      }
      else if (nvpair1.Name.ToUpper().Equals("DESCRIPTION".ToUpper()))
      {
        child = f.create(2);
      }
      else if (nvpair1.Name.ToUpper().Equals("ADDRESS_LIST".ToUpper()))
      {
        child = f.create(1);
      }
      else if (nvpair1.Name.ToUpper().Equals("ADDRESS".ToUpper()))
      {
        child = f.create(0);
      }
      else
      {
        throw new TnsException();
      }
      child.initFromNVPair(nvpair1);
      name = nvpair.Name;
    }

    /// <remarks/>
    public override String ToString()
    {
      return name + "=" + child.ToString();
    }

    /// <remarks/>
    protected internal SchemaObject child;
    /// <remarks/>
    public String name;
    private SchemaObjectFactoryInterface f;
  }
}