/*-----------------------------------------------*
 * Convertido de Oracle Project Raptor (Java)    *
 * por Alain Ramírez Cabrejas            *
 * Camagüey, Ciudad de los Tinajones.            *
 * Septiembre 2006                               *
 *-----------------------------------------------*
 */
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Oracle.Utils.TNSParser
{

  /// <remarks/>
  public class NLParamParser
  {
    /// <summary>
    /// Propiedad que retorna el nombre del archivo TNSNAMES.ORA y su path
    /// que se manda a procesar
    /// </summary>
    public virtual String Filename
    {
      get { return filename; }
    }
    /// <summary>
    /// Retorna la cantidad de alias del archivo TNSNAMES.ORA
    /// </summary>
    public virtual int NLPListSize
    {
      get
      {
        nvStringcnt = 0;
        IEnumerator enumeration = ht.Keys.GetEnumerator();
        do
        {
          if (!enumeration.MoveNext()) break;
          String s = (String) enumeration.Current;
          if (s.IndexOf("COMMENT") == - 1) nvStringcnt++;
        } while (true);
        return nvStringcnt;
      }
    }

    /// <summary>
    /// Retorna todos los alias del archivo TNSNAMES.ORA
    /// </summary>
    public virtual String[] NLPAllNames
    {
      get
      {
        int i = NLPListSize;
        String[] as_Renamed = new String[i];
        int j = 0;
        IEnumerator enumeration = ht.Keys.GetEnumerator();
        do
        {
          if (!enumeration.MoveNext()) break;
          String s = (String) enumeration.Current;
          if (s.IndexOf("COMMENT") == - 1)
            as_Renamed[j++] = s;
        } while (true);
        return as_Renamed;
      }
    }

    /// <summary>
    /// Retorna todos los alias con todos sus parámetros
    /// </summary>
    public virtual String[] NLPAllElements
    {
      get
      {
        int i = NLPListSize;
        String[] as_Renamed = new String[i];
        int j = 0;
        IEnumerator enumeration = ht.Values.GetEnumerator();
        do
        {
          if (!enumeration.MoveNext()) break;
          NVPair nvpair = (NVPair) enumeration.Current;
          if (nvpair.Name.IndexOf("COMMENT") == - 1)
          {
            String s = nvpair.ToString();
            as_Renamed[j++] = s;
          }
        } while (true);
        return as_Renamed;
      }
    }

    /// <remarks/>
    public String[] NLPAllDescription
    {
      get
      {
        int i = NLPListSize;
        String[] as_Renamed = new String[i];
        int j = 0;
        IEnumerator enumeration = ht.Values.GetEnumerator();
        do
        {
          if (!enumeration.MoveNext()) break;
          NVPair nvpair = (NVPair)enumeration.Current;
          if (nvpair.Name.IndexOf("COMMENT") == -1)
          {
            String s = nvpair.ToStringDescription();
            as_Renamed[j++] = s;
          }
        } while (true);
        return as_Renamed;
      }
    }

    /// <remarks/>
    public static NLParamParser createEmptyParamParser()
    {
      return new NLParamParser();
    }

    private NLParamParser()
    {
      Commentcnt = 0;
      nvStringcnt = 0;
      Groupcnt = 0;
      hasComments = false;
      hasGroups = false;
      filename = null;
      ht = Hashtable.Synchronized(new Hashtable(128));
    }

    /// <remarks/>
    public NLParamParser(String s)
      : this(s, (sbyte)2)
    {
    }

    /// <remarks/>
    public NLParamParser(String s, sbyte byte0)
    {
      Commentcnt = 0;
      nvStringcnt = 0;
      Groupcnt = 0;
      hasComments = false;
      hasGroups = false;
      filename = s;
      ht = Hashtable.Synchronized(new Hashtable(128));
      StreamReader filereader = null;
      StreamReader bufferedreader = null;
      try
      {
        filereader = new StreamReader(s, Encoding.Default);
        bufferedreader = new StreamReader(filereader.BaseStream, filereader.CurrentEncoding);
        initializeNlpa(bufferedreader, byte0);
      }
      catch
      {
        if ((byte0 & 2) == 0) throw new FileNotFoundException(s);
      }
      finally
      {
        if (filereader != null) filereader.Close();
        if (bufferedreader != null) bufferedreader.Close();
      }
    }

    /// <remarks/>
    public NLParamParser(StreamReader reader)
      : this(reader, (sbyte)0)
    {
    }

    /// <remarks/>
    public NLParamParser(StreamReader reader, sbyte byte0)
    {
      Commentcnt = 0;
      nvStringcnt = 0;
      Groupcnt = 0;
      hasComments = false;
      hasGroups = false;
      StreamReader bufferedreader = new StreamReader(reader.BaseStream, reader.CurrentEncoding);
      filename = null;
      ht = Hashtable.Synchronized(new Hashtable(128));
      initializeNlpa(bufferedreader, byte0);
    }

    private void initializeNlpa(StreamReader bufferedreader, sbyte byte0)
    {
      linebuffer = ArrayList.Synchronized(new ArrayList(100));
      errstr = new String[50];
      try
      {
        do
        {
          String s = bufferedreader.ReadLine();
          if (s == null) break;
          linebuffer.Add(s);
        } while (true);
      }
      catch
      {
        if ((byte0 & 2) == 0) throw new IOException("Unable to read a line from : " + filename);
      }
      String s1 = "";
      String s3 = Environment.NewLine;
      String s4 = "";
      String s5 = "";
      for (int i = 0; i < linebuffer.Count; i++)
      {
        String s2 = (String) linebuffer[i];
        if (s2.Length == 0) continue;
        if (s2[0] == '#')
        {
          if (s2.IndexOf(".ORA Configuration ") != - 1 || s2.IndexOf(" Network Configuration File: ") != - 1 ||
              s2.IndexOf("Generated by") != - 1)
          {
            continue;
          }
          if (s4.Length != 0)
          {
            s5 = s5 + s2 + s3;
            continue;
          }
          s4 = "COMMENT#" + Commentcnt;
          s5 = s2 + s3;
          if (!hasComments) hasComments = true;
          continue;
        }
        if (s2[0] == ' ' || s2[0] == '\t' || s2[0] == ')')
        {
          if (s5.Length == 0)
          {
            if (s1.Length == 0) s2 = eatNLPWS(s2);
            s2 = checkNLPforComments(s2);
            if (s2.Length != 0) s1 = s1 + s2 + s3;
            continue;
          }
          if (s1.Length == 0 && s5.Length != 0)
          {
            s2 = eatNLPWS(s2);
            s2 = checkNLPforComments(s2);
            if (s2.Length != 0 && (byte0 & 1) == 0)
              throw new TnsException("Carácter de continuación no válido después del comentario");
            continue;
          }
          if (s1.Length != 0 && s5.Length != 0)
          {
            s4 = "";
            s5 = "";
            s2 = checkNLPforComments(s2);
            s1 = s1 + s2 + s3;
          }
          continue;
        }
        if (s1.Length == 0 && s5.Length == 0)
        {
          s2 = checkNLPforComments(s2);
          s1 = s1 + s2 + s3;
          continue;
        }
        if (s1.Length == 0 && s5.Length != 0)
        {
          s5 = modifyCommentString(s5);
          try
          {
            addNLPListElement(s4 + "=" + s5);
          }
          catch (Exception nlexception2)
          {
            errstr[errstrcnt++] = s1;
            if ((byte0 & 1) == 0) throw nlexception2;
          }
          s4 = "";
          s5 = "";
          Commentcnt++;
          s2 = checkNLPforComments(s2);
          s1 = s1 + s2 + s3;
          continue;
        }
        if (s1.Length != 0 && s5.Length == 0)
        {
          try
          {
            addNLPListElement(s1);
          }
          catch (Exception nlexception3)
          {
            errstr[errstrcnt++] = s1;
            if ((byte0 & 1) == 0) throw nlexception3;
          }
          s1 = "";
          s2 = checkNLPforComments(s2);
          s1 = s1 + s2 + s3;
          continue;
        }
        if (s1.Length == 0 || s5.Length == 0) continue;
        try
        {
          addNLPListElement(s1);
        }
        catch (Exception nlexception4)
        {
          errstr[errstrcnt++] = s1;
          if ((byte0 & 1) == 0) throw nlexception4;
        }
        s1 = "";
        s2 = checkNLPforComments(s2);
        s1 = s1 + s2 + s3;
        s5 = modifyCommentString(s5);
        try
        {
          addNLPListElement(s4 + "=" + s5);
        }
        catch (Exception nlexception5)
        {
          errstr[errstrcnt++] = s1;
          if ((byte0 & 1) == 0) throw nlexception5;
        }
        s4 = "";
        s5 = "";
        Commentcnt++;
      }

      if (s1.Length != 0)
      {
        try
        {
          addNLPListElement(s1);
        }
        catch (Exception nlexception)
        {
          errstr[errstrcnt++] = s1;
          if ((byte0 & 1) == 0) throw nlexception;
        }
        s1 = "";
      }
      if (s5.Length != 0)
      {
        s5 = modifyCommentString(s5);
        try
        {
          addNLPListElement(s4 + "=" + s5);
        }
        catch (Exception nlexception1)
        {
          errstr[errstrcnt++] = s1;
          if ((byte0 & 1) == 0) throw nlexception1;
        }
        Commentcnt++;
      }
    }

    private String modifyCommentString(String s)
    {
      String s1 = "";
      for (int i = 0; i < s.Length; i++)
      {
        char c = s[i];
        switch (c)
        {
          case (char) (40): // '('
            s1 = s1 + "\\(";
            break;


          case (char) (61): // '='
            s1 = s1 + "\\=";
            break;


          case (char) (41): // ')'
            s1 = s1 + "\\)";
            break;


          case (char) (44): // ','
            s1 = s1 + "\\,";
            break;


          case (char) (92): // '\\'
            s1 = s1 + "\\\\";
            break;


          default:
            s1 = s1 + s[i];
            break;
        }
      }

      return s1;
    }

    private String checkNLPforComments(String s)
    {
      StringBuilder stringbuffer = new StringBuilder(s.Length);
      for (int i = 0; i < s.Length; i++)
      {
        char c = s[i];
        if (c == '#')
        {
          if (i != 0)
          {
            if (s[i - 1] != '\\') break;
            stringbuffer.Append(c);
          }
          else
          {
            return "";
          }
        }
        else
        {
          stringbuffer.Append(c);
        }
      }

      return stringbuffer.ToString();
    }

    private String eatNLPWS(String s)
    {
      StringBuilder stringbuffer = new StringBuilder(s.Length);
      int i = 0;
      bool flag = false;
      char c;
      do
      {
        while (!flag)
        {
          c = s[i++];
          if (c == ' ' && c == '\t')
          {
            //UPGRADE_NOTE: Labeled continue statement was changed to a goto statement. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1015'"
            goto label1;
          }
          flag = true;
          int j = i - 1;
          while (s[j] == '\n')
          {
            stringbuffer.Append(s[j]);
            j++;
          }
        }
        //UPGRADE_NOTE: Labeled break statement was changed to a goto statement. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1012'"
        goto label0_brk;
      //UPGRADE_NOTE: Label 'label1' was moved. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1014'"

        label1:
        ;
      } while (c != '\n');
      return "";
    label0_brk:
      ;

      return stringbuffer.ToString();
    }

    /// <remarks/>
    public virtual void saveNLParams()
    {
      StreamWriter filewriter;
      if (filename == null) return;
      filewriter = new StreamWriter(filename, false, Encoding.Default);
      String s = "unknown";
      for (
        SupportClass.Tokenizer stringtokenizer =
          new SupportClass.Tokenizer(filename, Path.DirectorySeparatorChar.ToString());
        stringtokenizer.HasMoreTokens();
        )
        s = stringtokenizer.NextToken();

      writeToStream(filewriter, s, filename);
      if (filewriter != null) filewriter.Close();
    }

    /// <remarks/>
    public virtual void writeToStream(StreamWriter writer, String s, String s1)
    {
      StreamWriter printwriter;
      printwriter =
        new StreamWriter(new StreamWriter(writer.BaseStream, writer.Encoding).BaseStream,
                         new StreamWriter(writer.BaseStream, writer.Encoding).Encoding);
      printwriter.WriteLine("# " + s + " Network Configuration File: " + s1 + "");
      printwriter.WriteLine("# Generated by Oracle configuration tools.");
      printwriter.WriteLine("");
      if (hasGroups) saveNLPGroups(printwriter);
      IEnumerator enumeration = ht.Values.GetEnumerator();
      do
      {
        if (!enumeration.MoveNext()) break;
        NVPair nvpair = (NVPair) enumeration.Current;
        String s3 = nvpair.toString(0, true);
        if (!s3.Equals(""))
        {
          char[] ac = new char[s3.Length - 2];
          SupportClass.GetCharsFromString(s3, 1, s3.Length - 1, ac, 0);
          String s2 = new String(ac);
          printwriter.WriteLine(s2);
          printwriter.WriteLine("");
        }
      } while (true);
      printwriter.Close();
    }

    /// <remarks/>
    public virtual void saveNLParams(String s)
    {
      String s1 = filename;
      filename = s;
      saveNLParams();
      filename = s1;
    }

    /// <remarks/>
    public virtual bool configuredInFile()
    {
      return filename != null;
    }

    /// <remarks/>
    public virtual bool inErrorList(String s)
    {
      bool flag = false;
      for (int i = 0; (!flag || i < errstrcnt) && errstrcnt != 0; i++)
        if (errstr[i].IndexOf(s) != - 1)
          flag = true;
      return flag;
    }

    /// <summary>
    /// Obtiene un alias con todos sus parámetros
    /// </summary>
    /// <param name="s">Nombre del alias</param>
    /// <returns>Datos del alias</returns>
    public virtual NVPair getNLPListElement(String s)
    {
      return (NVPair) ht[s.ToUpper()];
    }

    /// <remarks/>
    public virtual sbyte addNLPListElement(String s, Object obj)
    {
      Object tempObject = ht[s];
      ht[s] = obj;
      Object obj1 = tempObject;
      return ((sbyte) (obj1 == null ? 1 : 2));
    }

    /// <remarks/>
    public virtual void addNLPGroupProfile(String[] as_Renamed)
    {
      String s = new StringBuilder("GROUP#" + Groupcnt++).ToString();
      String[] as1 = as_Renamed;
      if (!hasGroups) hasGroups = true;
      addNLPListElement(s, as1);
    }

    private String[] getNLPGroupProfile(String s)
    {
      String s1 = s.ToUpper();
      return (String[]) ht[s1];
    }

    private void saveNLPGroups(StreamWriter printwriter)
    {
      for (int i = 0; i < Groupcnt; i++)
      {
        String s = new StringBuilder("GROUP#" + i).ToString();
        String[] as_Renamed = getNLPGroupProfile(s);
        for (int j = 0; j < as_Renamed.Length; j++)
        {
          NVPair nvpair;
          if (as_Renamed[j] == null) continue;
          nvpair = getNLPListElement(as_Renamed[j]);
          if (nvpair != null)
          {
            String s1 = nvpair.toString(0, true);
            char[] ac = new char[s1.Length - 2];
            SupportClass.GetCharsFromString(s1, 1, s1.Length - 1, ac, 0);
            String s2 = new String(ac);
            printwriter.WriteLine(s2);
            printwriter.WriteLine("");
            removeNLPListElement(as_Renamed[j]);
            continue;
          }
        }
        removeNLPGroupProfile(s);
      }
    }

    /// <remarks/>
    public virtual void addNLPListElement(String s)
    {
      char[] ac = new char[s.Length + 2];
      String s1;
      SupportClass.GetCharsFromString(s, 0, s.Length, ac, 1);
      if (ac[1] == '(')
      {
        s1 = s;
      }
      else
      {
        ac[0] = '(';
        String s2 = Environment.GetEnvironmentVariable("OS");
        if (s2.Equals("Windows NT") || s2.Equals("Windows 95"))
        {
          if (ac[ac.Length - 2] == '/' || ac[ac.Length - 2] == '\\')
            ac[ac.Length - 2] = ')';
          else
            ac[ac.Length - 1] = ')';
        }
        else if (ac[ac.Length - 2] == '\\')
          ac[ac.Length - 2] = ')';
        else
          ac[ac.Length - 1] = ')';
        s1 = new String(ac);
      }
      NVFactory nvfactory = new NVFactory();
      NVPair nvpair = nvfactory.createNVPair(s1);
      if (nvpair.RHSType == NVPair.RHS_NONE) throw new TnsException("RHS nulo");
      String s3 = nvpair.Name;
      String s4 = s3.ToUpper();
      nvpair.Name = s4;
      sbyte byte0 = addNLPListElement(s4, nvpair);
      switch (byte0)
      {
        default:
          break;


        case 2: // '\002'
          if (DEBUG)
            Console.Out.WriteLine("The value for the Name: " + s3 + " was overwritten\n");
          break;


        case - 1:
          if (DEBUG)
            Console.Out.WriteLine("The value for the Name: " + s3 + " could not be inserted\n");
          break;
      }
    }

    /// <remarks/>
    public virtual NVPair removeNLPListElement(String s)
    {
      String s1 = s.ToUpper();
      Object tempObject;
      tempObject = ht[s1];
      ht.Remove(s1);
      Object obj = tempObject;
      return obj == null ? null : (NVPair) obj;
    }

    /// <remarks/>
    public virtual void removeNLPGroupProfile(String s)
    {
      ht.Remove(s.ToUpper());
    }

    /// <remarks/>
    public virtual void removeNLPAllElements()
    {
      ht.Clear();
    }

    /// <remarks/>
    public override String ToString()
    {
      String s = "";
      for (IEnumerator enumeration = ht.Values.GetEnumerator(); enumeration.MoveNext();)
      {
        NVPair nvpair = (NVPair) enumeration.Current;
        String s1 = nvpair.ToString();
        s = s + s1 + "\n";
      }
      return s;
    }

    /// <summary>
    /// Retorna verdadero en caso de que archivo contenga comentarios
    /// </summary>
    /// <returns>true SI y false NO</returns>
    public virtual bool fileHasComments()
    {
      return hasComments;
    }

    private String filename;
    private Hashtable ht;
    private ArrayList linebuffer;
    private int Commentcnt;
    private int nvStringcnt;
    private int Groupcnt;
    private bool hasComments;
    private bool hasGroups;
    private String[] errstr;
    private int errstrcnt;
    /// <remarks/>
    public const sbyte IGNORE_NONE = 0;
    /// <remarks/>
    public const sbyte IGNORE_NL_EXCEPTION = 1;
    /// <remarks/>
    public const sbyte IGNORE_FILE_EXCEPTION = 2;
    /// <remarks/>
    public const sbyte NLPASUCC = 1;
    /// <remarks/>
    public const sbyte NLPAOVWR = 2;
    /// <remarks/>
    public const sbyte NLPAFAIL = -1;
    private static bool DEBUG = false;
  }
}