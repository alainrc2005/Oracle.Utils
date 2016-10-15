/*-----------------------------------------------*
 * Convertido de Oracle Project Raptor (Java)    *
 * por Alain Ramírez Cabrejas            *
 * Camagüey, Ciudad de los Tinajones.            *
 * Septiembre 2006                               *
 *-----------------------------------------------*
 */
namespace Oracle.Utils.TNSParser
{

  /// <remarks/>
  public class SchemaObjectFactory : SchemaObjectFactoryInterface
  {
    /// <remarks/>
    public virtual SchemaObject create(int i)
    {
      switch (i)
      {
        case 0: // '\0'
          return new Address(this);


        case 1: // '\001'
          return new AddressList(this);


        case 2: // '\002'
          return new Description(this);


        case 3: // '\003'
          return new DescriptionList(this);


        case 4: // '\004'
          return new ServiceAlias(this);
      }
      return null;
    }
  }
}