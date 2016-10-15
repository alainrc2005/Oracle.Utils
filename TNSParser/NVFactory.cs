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
  public class NVFactory
  {
    /// <remarks/>
    public virtual NVPair createNVPair(String s)
    {
      NVTokens nvtokens = new NVTokens();
      nvtokens.parseTokens(s);
      return _readTopLevelNVPair(nvtokens);
    }

    private NVPair _readTopLevelNVPair(NVTokens nvtokens)
    {
      int i = nvtokens.Token;
      nvtokens.eatToken();
      if (i != 1)
      {
        throw new TnsException("Error de sintaxis no válida.");
      }
      String s = _readNVLiteral(nvtokens);
      NVPair nvpair = new NVPair(s);
      if ((i = nvtokens.Token) == 3)
      {
        for (; i == 8 || i == 3; i = nvtokens.Token)
          s = s + nvtokens.popLiteral();

        nvpair.Name = s;
        return _readRightHandSide(nvpair, nvtokens);
      }
      else
      {
        return _readRightHandSide(nvpair, nvtokens);
      }
    }

    private NVPair _readNVPair(NVTokens nvtokens)
    {
      int i = nvtokens.Token;
      nvtokens.eatToken();
      if (i != 1 && i != 3)
      {
        throw new TnsException("Error de sintaxis no válida.");
      }
      else
      {
        String s = _readNVLiteral(nvtokens);
        NVPair nvpair = new NVPair(s);
        return _readRightHandSide(nvpair, nvtokens);
      }
    }

    private NVPair _readRightHandSide(NVPair nvpair, NVTokens nvtokens)
    {
      int i;
      switch (nvtokens.Token)
      {
        case 4: // '\004'
          nvtokens.eatToken();
          i = nvtokens.Token;
          if (i == 8)
          {
            String s = _readNVLiteral(nvtokens);
            nvpair.Atom = s;
          }
          else
          {
            _readNVList(nvtokens, nvpair);
          }
          break;


        case 2:
          // '\002'
        case 3: // '\003'
          nvpair.Atom = nvpair.Name;
          break;


        default:
          throw new TnsException("Error de sintaxis no válida.");
      }
      i = nvtokens.Token;
      if (i == 2) nvtokens.eatToken();
      else if (i != 3)
      {
        throw new TnsException("Error de sintaxis no válida: Carácter o LITERAL inesperado");
      }
      return nvpair;
    }

    private String _readNVLiteral(NVTokens nvtokens)
    {
      int i = nvtokens.Token;
      if (i != 8)
      {
        throw new TnsException("Error de sintaxis no válida.");
      }
      else
      {
        return nvtokens.popLiteral();
      }
    }

    private void _readNVList(NVTokens nvtokens, NVPair nvpair)
    {
      int i = nvtokens.Token;
      if (i != 1 && i != 3) return;
      NVPair nvpair1 = _readNVPair(nvtokens);
      nvpair.addListElement(nvpair1);
      if ((i == 3 || nvpair1.Name == nvpair1.Atom) && nvpair.ListType != NVPair.LIST_COMMASEP)
        nvpair.ListType = NVPair.LIST_COMMASEP;
      _readNVList(nvtokens, nvpair);
    }

  }
}