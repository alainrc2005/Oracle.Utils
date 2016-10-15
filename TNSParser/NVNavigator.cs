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
  public class NVNavigator
  {
    /// <remarks/>
    public virtual NVPair findNVPairRecurse(NVPair nvpair, String s)
    {
      if (nvpair == null || s.ToUpper().Equals(nvpair.Name.ToUpper()))
        return nvpair;
      if (nvpair.RHSType == NVPair.RHS_ATOM) return null;
      for (int i = 0; i < nvpair.ListSize; i++)
      {
        NVPair nvpair1 = findNVPairRecurse(nvpair.getListElement(i), s);
        if (nvpair1 != null) return nvpair1;
      }
      return null;
    }

    /// <remarks/>
    public virtual NVPair findNVPair(NVPair nvpair, String s)
    {
      if (nvpair == null || nvpair.RHSType != NVPair.RHS_LIST)
        return null;
      for (int i = 0; i < nvpair.ListSize; i++)
      {
        NVPair nvpair1 = nvpair.getListElement(i);
        if (s.ToUpper().Equals(nvpair1.Name.ToUpper()))
          return nvpair1;
      }

      return null;
    }

    /// <remarks/>
    public virtual NVPair findNVPair(NVPair nvpair, String[] as_Renamed)
    {
      NVPair nvpair1 = nvpair;
      for (int i = 0; i < as_Renamed.Length; i++)
      {
        nvpair1 = findNVPair(nvpair1, as_Renamed[i]);
        if (nvpair1 == null) return null;
      }
      return nvpair1;
    }
  }
}