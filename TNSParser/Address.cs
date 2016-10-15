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
	public class Address : SchemaObject
	{
    /// <remarks/>
    virtual public String Protocol
		{
      get { return prot; }
    }

    /// <remarks/>
    public Address(SchemaObjectFactoryInterface schemaobjectfactoryinterface)
		{
			f = null;
			f = schemaobjectfactoryinterface;
		}

    /// <remarks/>
    public virtual int isA()
		{
			return 0;
		}

    /// <remarks/>
    public virtual String isA_String()
		{
			return "ADDRESS";
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
			if (nvpair == null || !nvpair.Name.ToUpper().Equals("address".ToUpper()))
				throw new Exception();
			NVNavigator nvnavigator = new NVNavigator();
			NVPair nvpair1 = nvnavigator.findNVPair(nvpair, "PROTOCOL");
			if (nvpair1 == null)
				throw new Exception();
			prot = nvpair1.Atom;
			if (addr == null)
			{
				addr = nvpair.ToString();
			}
		}

    /// <remarks/>
    public override String ToString()
		{
			return addr;
		}

    /// <remarks/>
    protected internal virtual void init()
		{
			addr = null;
			prot = null;
		}

    /// <remarks/>
    public String addr;
    /// <remarks/>
    public String prot;
    /// <remarks/>
    protected internal SchemaObjectFactoryInterface f;
	}
}