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
  public sealed class NVPair
  {
    /// <remarks/>
    public String Name
    {
      get { return _name; }
      set { _name = value; }
    }

    /// <remarks/>
    public NVPair Parent
    {
      get { return _parent; }
    }

    /// <remarks/>
    public int RHSType
    {
      get { return _rhsType; }
    }

    /// <remarks/>
    public int ListType
    {
      get { return _listType; }
      set { _listType = value; }
    }

    /// <remarks/>
    public String Atom
    {
      get { return _atom; }

      set
      {
        if (_name.IndexOf("COMMENT") == - 1 && containsComment(value))
        {
          throw new TnsException("Error de sintaxis no válida: Carácter inesperado.");
        }
        else
        {
          _rhsType = RHS_ATOM;
          _atom = value;
          _list = null;
          return;
        }
      }
    }

    /// <remarks/>
    public int ListSize
    {
      get
      {
        if (_list == null)
          return 0;
        else
          return _list.Count;
      }
    }

    /// <remarks/>
    public NVPair(String s)
    {
      _linesep = Environment.NewLine;
      _name = s;
      _atom = null;
      _list = null;
      _listType = LIST_REGULAR;
      _parent = null;
      _rhsType = RHS_NONE;
    }

    /// <remarks/>
    public NVPair(String s, String s1)
      : this(s)
    {
      Atom = s1;
    }

    /// <remarks/>
    public NVPair(String s, NVPair nvpair)
      : this(s)
    {
      addListElement(nvpair);
    }

    private void _setParent(NVPair nvpair)
    {
      _parent = nvpair;
    }

    private bool containsComment(String s)
    {
      for (int i = 0; i < s.Length; i++)
      {
        if (s[i] != '#')
          continue;
        if (i != 0)
        {
          if (s[i - 1] != '\\')
            return true;
        }
        else
        {
          return true;
        }
      }

      return false;
    }

    /// <remarks/>
    public NVPair getListElement(int i)
    {
      if (_list == null)
        return null;
      else
        return (NVPair) _list[i];
    }

    /// <remarks/>
    public void addListElement(NVPair nvpair)
    {
      if (_list == null)
      {
        _rhsType = RHS_LIST;
        _list = ArrayList.Synchronized(new ArrayList(3));
        _atom = null;
      }
      _list.Add(nvpair);
      nvpair._setParent(this);
    }

    /// <remarks/>
    public void removeListElement(int i)
    {
      if (_list != null)
      {
        _list.RemoveAt(i);
        if (ListSize == 0)
        {
          _list = null;
          _rhsType = RHS_NONE;
        }
      }
    }

    private String space(int i)
    {
      String s = new StringBuilder("").ToString();
      for (int j = 0; j < i; j++)
        s = s + " ";

      return s;
    }

    /// <remarks/>
    public String trimValueToString()
    {
      String s = valueToString().Trim();
      return s.Substring(1, (s.Length - 1) - (1));
    }

    /// <remarks/>
    public String valueToString()
    {
      String s = "";
      if (_rhsType == RHS_ATOM)
        s = s + _atom;
      else if (_rhsType == RHS_LIST)
        if (_listType == LIST_REGULAR)
        {
          for (int i = 0; i < ListSize; i++)
          {
            s = s + getListElement(i).ToString();
          }
        }
        else if (_listType == LIST_COMMASEP)
        {
          for (int j = 0; j < ListSize; j++)
          {
            NVPair nvpair = getListElement(j);
            s = s + nvpair.Name;
            if (j != ListSize - 1)
              s = s + ", ";
          }
        }
      return s;
    }

    /// <remarks/>
    public override String ToString()
    {
      String s = "(" + _name + "=";
      if (_rhsType == RHS_ATOM)
        s = s + _atom;
      else if (_rhsType == RHS_LIST)
        if (_listType == LIST_REGULAR)
        {
          for (int i = 0; i < ListSize; i++)
          {
            s = s + getListElement(i).ToString();
          }
        }
        else if (_listType == LIST_COMMASEP)
        {
          s = s + " (";
          for (int j = 0; j < ListSize; j++)
          {
            NVPair nvpair = getListElement(j);
            s = s + nvpair.Name;
            if (j != ListSize - 1)
              s = s + ", ";
          }

          s = s + ")";
        }
      s = s + ")";
      return s;
    }

    /// <summary>
    /// Retorna solo la descripción del alias
    /// Alain Ramírez Cabrejas
    /// </summary>
    /// <returns>Descripción del Alias</returns>
    public String ToStringDescription()
    {
      String s = "";
      if (_rhsType == RHS_ATOM)
        s = s + _atom;
      else if (_rhsType == RHS_LIST)
        if (_listType == LIST_REGULAR)
        {
          for (int i = 0; i < ListSize; i++)
          {
            s = s + getListElement(i).ToString();
          }
        }
        else if (_listType == LIST_COMMASEP)
        {
          s = s + " (";
          for (int j = 0; j < ListSize; j++)
          {
            NVPair nvpair = getListElement(j);
            s = s + nvpair.Name;
            if (j != ListSize - 1)
              s = s + ", ";
          }

          s = s + ")";
        }
      return s;
    }

    /// <remarks/>
    public String toString(int i, bool flag)
    {
      String s = "";
      String s1 = new StringBuilder(_name).ToString();
      if (_rhsType == RHS_LIST)
      {
        if (_listType == LIST_REGULAR)
        {
          String s2 = "";
          for (int k = 0; k < ListSize; k++)
            if (s1.ToUpper().Equals("ADDRESS".ToUpper()) || s1.ToUpper().Equals("RULE".ToUpper()))
              s2 = s2 + getListElement(k).toString(i + 1, false);
            else
              s2 = s2 + getListElement(k).toString(i + 1, true);

          if (!s2.Equals(""))
          {
            if (s1.ToUpper().Equals("ADDRESS".ToUpper()) || s1.ToUpper().Equals("RULE".ToUpper()))
              s = s + space(i*2) + "(" + _name + " = ";
            else
              s = s + space(i*2) + "(" + _name + " =" + _linesep;
            s = s + s2;
            if (s1.ToUpper().Equals("ADDRESS".ToUpper()) || s1.ToUpper().Equals("RULE".ToUpper()))
              s = s + ")" + _linesep;
            else if (i == 0)
              s = s + ")";
            else if (i == 1)
              s = s + space(i*2) + ")";
            else
              s = s + space(i*2) + ")" + _linesep;
          }
        }
        else if (_listType == LIST_COMMASEP)
        {
          s = s + "(" + _name + "=" + " (";
          for (int j = 0; j < ListSize; j++)
          {
            NVPair nvpair = getListElement(j);
            s = s + nvpair.Name;
            if (j != ListSize - 1)
              s = s + ", ";
          }

          s = s + ")" + ")";
        }
      }
      else if (_rhsType == RHS_ATOM)
        if (i == 0)
        {
          if (s1.IndexOf("COMMENT") != - 1)
          {
            _atom = modifyCommentString(_atom);
            s = s + "(" + _atom + ")";
          }
          else
          {
            s = s + "(" + _name + " = " + _atom + ")";
          }
        }
        else if (s1.IndexOf("COMMENT") != - 1)
        {
          _atom = modifyCommentString(_atom);
          s = s + _atom + _linesep;
        }
        else if (!flag)
        {
          s = s + "(" + _name + " = " + _atom + ")";
        }
        else
        {
          s = s + space(i*2) + "(" + _name + " = " + _atom + ")";
          s = s + _linesep;
        }
      return s;
    }

    /// <remarks/>
    public String modifyCommentString(String s)
    {
      String s1 = "";
      for (int i = 0; i < s.Length;)
      {
        char c = s[i];
        switch (c)
        {
          default:
            break;


          case (char) (92): // '\\'
            if (s[i + 1] == '(' || s[i + 1] == '=' || s[i + 1] == ')' || s[i + 1] == ',' || s[i + 1] == '\\')
              i++;
            break;
        }
        s1 = s1 + s[i++];
      }

      return s1;
    }

    /// <remarks/>
    public static int RHS_NONE = 0;
    /// <remarks/>
    public static int RHS_ATOM = 1;
    /// <remarks/>
    public static int RHS_LIST = 2;
    /// <remarks/>
    public static int LIST_REGULAR = 3;
    /// <remarks/>
    public static int LIST_COMMASEP = 4;
    private String _name;
    private int _rhsType;
    private String _atom;
    private ArrayList _list;
    private int _listType;
    private NVPair _parent;
    private String _linesep;
  }
}