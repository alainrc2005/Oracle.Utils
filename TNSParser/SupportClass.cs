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
  /// <summary>
  /// Contains conversion support elements such as classes, interfaces and static methods.
  /// </summary>
  public class SupportClass
  {
    /// <summary>
    /// The class performs token processing in strings
    /// </summary>
    public class Tokenizer : IEnumerator
    {
      /// Position over the string
      private long currentPos = 0;

      /// Include demiliters in the results.
      private bool includeDelims = false;

      /// Char representation of the String to tokenize.
      private char[] chars = null;

      //The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character and the form-feed character
      private string delimiters = " \t\n\r\f";

      /// <summary>
      /// Initializes a new class instance with a specified string to process
      /// </summary>
      /// <param name="source">String to tokenize</param>
      public Tokenizer(String source)
      {
        chars = source.ToCharArray();
      }

      /// <summary>
      /// Initializes a new class instance with a specified string to process
      /// and the specified token delimiters to use
      /// </summary>
      /// <param name="source">String to tokenize</param>
      /// <param name="delimiters">String containing the delimiters</param>
      public Tokenizer(String source, String delimiters)
        : this(source)
      {
        this.delimiters = delimiters;
      }


      /// <summary>
      /// Initializes a new class instance with a specified string to process, the specified token 
      /// delimiters to use, and whether the delimiters must be included in the results.
      /// </summary>
      /// <param name="source">String to tokenize</param>
      /// <param name="delimiters">String containing the delimiters</param>
      /// <param name="includeDelims">Determines if delimiters are included in the results.</param>
      public Tokenizer(String source, String delimiters, bool includeDelims)
        : this(source, delimiters)
      {
        this.includeDelims = includeDelims;
      }


      /// <summary>
      /// Returns the next token from the token list
      /// </summary>
      /// <returns>The string value of the token</returns>
      public String NextToken()
      {
        return NextToken(delimiters);
      }

      /// <summary>
      /// Returns the next token from the source string, using the provided
      /// token delimiters
      /// </summary>
      /// <param name="charDelimiters">String containing the delimiters to use</param>
      /// <returns>The string value of the token</returns>
      public String NextToken(String charDelimiters)
      {
        //According to documentation, the usage of the received delimiters should be temporary (only for this call).
        //However, it seems it is not true, so the following line is necessary.
        delimiters = charDelimiters;

        //at the end 
        if (currentPos == chars.Length)
          throw new ArgumentOutOfRangeException();
        //if over a delimiter and delimiters must be returned
        else if ((Array.IndexOf(charDelimiters.ToCharArray(), chars[currentPos]) != -1)
                 && includeDelims)
          return "" + chars[currentPos++];
        //need to get the token wo delimiters.
        else
          return nextToken(charDelimiters.ToCharArray());
      }

      //Returns the nextToken wo delimiters
      private String nextToken(char[] charDelimiters)
      {
        string token = "";
        long pos = currentPos;

        //skip possible delimiters
        while (Array.IndexOf(charDelimiters, chars[currentPos]) != -1)
          //The last one is a delimiter (i.e there is no more tokens)
          if (++currentPos == chars.Length)
          {
            currentPos = pos;
            throw new ArgumentOutOfRangeException();
          }

        //getting the token
        while (Array.IndexOf(charDelimiters, chars[currentPos]) == -1)
        {
          token += chars[currentPos];
          //the last one is not a delimiter
          if (++currentPos == chars.Length)
            break;
        }
        return token;
      }


      /// <summary>
      /// Determines if there are more tokens to return from the source string
      /// </summary>
      /// <returns>True or false, depending if there are more tokens</returns>
      public bool HasMoreTokens()
      {
        //keeping the current pos
        long pos = currentPos;

        try
        {
          NextToken();
        }
        catch (ArgumentOutOfRangeException)
        {
          return false;
        }
        finally
        {
          currentPos = pos;
        }
        return true;
      }

      /// <summary>
      /// Remaining tokens count
      /// </summary>
      public int Count
      {
        get
        {
          //keeping the current pos
          long pos = currentPos;
          int i = 0;

          try
          {
            while (true)
            {
              NextToken();
              i++;
            }
          }
          catch (ArgumentOutOfRangeException)
          {
            currentPos = pos;
            return i;
          }
        }
      }

      /// <summary>
      ///  Performs the same action as NextToken.
      /// </summary>
      public Object Current
      {
        get { return NextToken(); }
      }

      /// <summary>
      ///  Performs the same action as HasMoreTokens.
      /// </summary>
      /// <returns>True or false, depending if there are more tokens</returns>
      public bool MoveNext()
      {
        return HasMoreTokens();
      }

      /// <summary>
      /// Does nothing.
      /// </summary>
      public void Reset()
      {
        ;
      }
    }

    /*******************************/

    /// <summary>
    /// Copies an array of chars obtained from a String into a specified array of chars
    /// </summary>
    /// <param name="sourceString">The String to get the chars from</param>
    /// <param name="sourceStart">Position of the String to start getting the chars</param>
    /// <param name="sourceEnd">Position of the String to end getting the chars</param>
    /// <param name="destinationArray">Array to return the chars</param>
    /// <param name="destinationStart">Position of the destination array of chars to start storing the chars</param>
    /// <returns>An array of chars</returns>
    public static void GetCharsFromString(String sourceString, int sourceStart, int sourceEnd, char[] destinationArray,
                                          int destinationStart)
    {
      int sourceCounter;
      int destinationCounter;
      sourceCounter = sourceStart;
      destinationCounter = destinationStart;
      while (sourceCounter < sourceEnd)
      {
        destinationArray[destinationCounter] = sourceString[sourceCounter];
        sourceCounter++;
        destinationCounter++;
      }
    }

    /*******************************/

    /// <summary>
    /// Converts an array of sbytes to an array of chars
    /// </summary>
    /// <param name="sByteArray">The array of sbytes to convert</param>
    /// <returns>The new array of chars</returns>
    public static char[] ToCharArray(sbyte[] sByteArray)
    {
      return UTF8Encoding.UTF8.GetChars(ToByteArray(sByteArray));
    }

    /// <summary>
    /// Converts an array of bytes to an array of chars
    /// </summary>
    /// <param name="byteArray">The array of bytes to convert</param>
    /// <returns>The new array of chars</returns>
    public static char[] ToCharArray(byte[] byteArray)
    {
      return UTF8Encoding.UTF8.GetChars(byteArray);
    }

    /*******************************/

    /// <summary>
    /// Converts an array of sbytes to an array of bytes
    /// </summary>
    /// <param name="sbyteArray">The array of sbytes to be converted</param>
    /// <returns>The new array of bytes</returns>
    public static byte[] ToByteArray(sbyte[] sbyteArray)
    {
      byte[] byteArray = null;

      if (sbyteArray != null)
      {
        byteArray = new byte[sbyteArray.Length];
        for (int index = 0; index < sbyteArray.Length; index++)
          byteArray[index] = (byte)sbyteArray[index];
      }
      return byteArray;
    }

    /// <summary>
    /// Converts a string to an array of bytes
    /// </summary>
    /// <param name="sourceString">The string to be converted</param>
    /// <returns>The new array of bytes</returns>
    public static byte[] ToByteArray(String sourceString)
    {
      return UTF8Encoding.UTF8.GetBytes(sourceString);
    }

    /// <summary>
    /// Converts a array of object-type instances to a byte-type array.
    /// </summary>
    /// <param name="tempObjectArray">Array to convert.</param>
    /// <returns>An array of byte type elements.</returns>
    public static byte[] ToByteArray(Object[] tempObjectArray)
    {
      byte[] byteArray = null;
      if (tempObjectArray != null)
      {
        byteArray = new byte[tempObjectArray.Length];
        for (int index = 0; index < tempObjectArray.Length; index++)
          byteArray[index] = (byte)tempObjectArray[index];
      }
      return byteArray;
    }
  }
}