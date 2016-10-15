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
  public sealed class NVTokens
  {
    /// <remarks/>
    public int Token
    {
      get
      {
        if (_tkType == null)
          throw new TnsException("No se ha inicializado el objeto de análisis.");
        if (_tkPos < _numTokens)
          return ((Int32) _tkType[_tkPos]);
        else
          throw new TnsException("No quedan literales, se ha alcanzado el final del par NV.");
      }
    }

    /// <remarks/>
    public String Literal
    {
      get
      {
        String s;
        if (_tkValue == null)
          throw new TnsException("No se ha inicializado el objeto de análisis.");
        if (_tkPos < _numTokens)
          s = ((String) _tkValue[_tkPos]);
        else
          throw new TnsException("No quedan literales, se ha alcanzado el final del par NV.");
        return s;
      }
    }

    /// <remarks/>
    public NVTokens()
    {
      _tkType = null;
      _tkValue = null;
      _numTokens = 0;
      _tkPos = 0;
    }

    private static bool _isWhiteSpace(char c)
    {
      return c == ' ' || c == '\t' || c == '\n' || c == '\r';
    }

    private static String _trimWhiteSpace(String s)
    {
      int i = s.Length;
      int j = 0;
      int k = i;
      for (; j < i && _isWhiteSpace(s[j]); j++) ;
      for (; j < k && _isWhiteSpace(s[k - 1]); k--) ;
      return s.Substring(j, (k) - (j));
    }

    /// <remarks/>
    public bool parseTokens(String s)
    {
      _numTokens = 0;
      _tkPos = 0;
      _tkType = ArrayList.Synchronized(new ArrayList(25));
      _tkValue = ArrayList.Synchronized(new ArrayList(25));
      int i = s.Length;
      char[] ac = s.ToCharArray();
      int j = 0;
      do
      {
        if (j >= i)
          break;
        for (; j < i && _isWhiteSpace(ac[j]); j++) ;
        switch (ac[j])
        {
          case (char) (40): // '('
            _addToken(1, '(');
            j++;
            break;


          case (char) (61): // '='
            _addToken(4, '=');
            j++;
            break;


          case (char) (41): // ')'
            _addToken(2, ')');
            j++;
            break;


          case (char) (44): // ','
            _addToken(3, ',');
            j++;
            break;


          default:
            int k = j;
            int l = - 1;
            bool flag = false;
            char c = '"';
            if (ac[j] == '\'' || ac[j] == '"')
            {
              flag = true;
              c = ac[j];
              j++;
            }
            do
            {
              if (j >= i)
                break;
              if (ac[j] == '\\')
              {
                j += 2;
                continue;
              }
              if (flag)
              {
                if (ac[j] == c)
                {
                  l = ++j;
                  break;
                }
              }
              else if (ac[j] == '(' || ac[j] == ')' || ac[j] == ',' || ac[j] == '=')
              {
                l = j;
                break;
              }
              j++;
            } while (true);
            if (l == - 1)
              l = j;
            _addToken(8, _trimWhiteSpace(s.Substring(k, (l) - (k))));
            break;
        }
      } while (true);
      _addToken(9, '%');
      return true;
    }

    /// <remarks/>
    public int popToken()
    {
      int i;
      if (_tkType == null)
        throw new TnsException("No se ha inicializado el objeto de análisis.");
      if (_tkPos < _numTokens)
        i = ((Int32) _tkType[_tkPos++]);
      else
        throw new TnsException("No quedan literales, se ha alcanzado el final del par NV.");
      return i;
    }

    /// <remarks/>
    public String popLiteral()
    {
      String s;
      if (_tkValue == null)
        throw new TnsException("No se ha inicializado el objeto de análisis.");
      if (_tkPos < _numTokens)
        s = ((String) _tkValue[_tkPos++]);
      else
        throw new TnsException("No quedan literales, se ha alcanzado el final del par NV.");
      return s;
    }

    /// <remarks/>
    public void eatToken()
    {
      if (_tkPos < _numTokens)
        _tkPos++;
    }

    /// <remarks/>
    public override String ToString()
    {
      if (_tkType == null)
        return "*NO TOKENS*";
      String s = "Tokens";
      for (int i = 0; i < _numTokens; i++)
      {
        s = s + " : " + _tkValue[i];
      }

      return s;
    }

    private void _addToken(int i, char c)
    {
      _addToken(i, Convert.ToString(c));
    }

    private void _addToken(int i, String s)
    {
      _tkType.Add(i);
      _tkValue.Add(s);
      _numTokens++;
    }

    /// <remarks/>
    public const int TKN_NONE = 0;
    /// <remarks/>
    public const int TKN_LPAREN = 1;
    /// <remarks/>
    public const int TKN_RPAREN = 2;
    /// <remarks/>
    public const int TKN_COMMA = 3;
    /// <remarks/>
    public const int TKN_EQUAL = 4;
    /// <remarks/>
    public const int TKN_LITERAL = 8;
    /// <remarks/>
    public const int TKN_EOS = 9;
    private ArrayList _tkType;
    private ArrayList _tkValue;
    private int _numTokens;
    private int _tkPos;
  }
}