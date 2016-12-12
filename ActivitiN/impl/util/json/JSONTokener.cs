using System;
using System.Text;

namespace org.activiti.engine.impl.util.json
{


	/*
	Copyright (c) 2002 JSON.org
	
	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:
	
	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.
	
	The Software shall be used for Good, not Evil.
	
	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.
	*/

	/// <summary>
	/// A JSONTokener takes a source string and extracts characters and tokens from
	/// it. It is used by the JSONObject and JSONArray constructors to parse
	/// JSON source strings.
	/// @author JSON.org
	/// @version 2010-02-02
	/// </summary>
	public class JSONTokener
	{

		private int character;
		private bool eof;
		private int index;
		private int line;
		private char previous;
		private Reader reader;
		private bool usePrevious;


		/// <summary>
		/// Construct a JSONTokener from a reader.
		/// </summary>
		/// <param name="reader">     A reader. </param>
		public JSONTokener(Reader reader)
		{
			this.reader = reader.markSupported() ? reader : new BufferedReader(reader);
			this.eof = false;
			this.usePrevious = false;
			this.previous = (char)0;
			this.index = 0;
			this.character = 1;
			this.line = 1;
		}


		/// <summary>
		/// Construct a JSONTokener from a string.
		/// </summary>
		/// <param name="s">     A source string. </param>
		public JSONTokener(string s) : this(new StringReader(s))
		{
		}


		/// <summary>
		/// Back up one character. This provides a sort of lookahead capability,
		/// so that you can test for a digit or letter before attempting to parse
		/// the next number or identifier.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void back() throws JSONException
		public virtual void back()
		{
			if (usePrevious || index <= 0)
			{
				throw new JSONException("Stepping back two steps is not supported");
			}
			this.index -= 1;
			this.character -= 1;
			this.usePrevious = true;
			this.eof = false;
		}


		/// <summary>
		/// Get the hex value of a character (base16). </summary>
		/// <param name="c"> A character between '0' and '9' or between 'A' and 'F' or
		/// between 'a' and 'f'. </param>
		/// <returns>  An int between 0 and 15, or -1 if c was not a hex digit. </returns>
		public static int dehexchar(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return c - '0';
			}
			if (c >= 'A' && c <= 'F')
			{
				return c - ('A' - 10);
			}
			if (c >= 'a' && c <= 'f')
			{
				return c - ('a' - 10);
			}
			return -1;
		}

		public virtual bool end()
		{
			return eof && !usePrevious;
		}


		/// <summary>
		/// Determine if the source string still contains characters that next()
		/// can consume. </summary>
		/// <returns> true if not yet at the end of the source. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean more() throws JSONException
		public virtual bool more()
		{
			next();
			if (end())
			{
				return false;
			}
			back();
			return true;
		}


		/// <summary>
		/// Get the next character in the source string.
		/// </summary>
		/// <returns> The next character, or 0 if past the end of the source string. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public char next() throws JSONException
		public virtual char next()
		{
			int c;
			if (this.usePrevious)
			{
				this.usePrevious = false;
				c = this.previous;
			}
			else
			{
				try
				{
					c = this.reader.read();
				}
				catch (IOException exception)
				{
					throw new JSONException(exception);
				}

				if (c <= 0) // End of stream
				{
					this.eof = true;
					c = 0;
				}
			}
			this.index += 1;
			if (this.previous == '\r')
			{
				this.line += 1;
				this.character = c == '\n' ? 0 : 1;
			}
			else if (c == '\n')
			{
				this.line += 1;
				this.character = 0;
			}
			else
			{
				this.character += 1;
			}
			this.previous = (char) c;
			return this.previous;
		}


		/// <summary>
		/// Consume the next character, and check that it matches a specified
		/// character. </summary>
		/// <param name="c"> The character to match. </param>
		/// <returns> The character. </returns>
		/// <exception cref="JSONException"> if the character does not match. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public char next(char c) throws JSONException
		public virtual char next(char c)
		{
			char n = next();
			if (n != c)
			{
				throw syntaxError("Expected '" + c + "' and instead saw '" + n + "'");
			}
			return n;
		}


		/// <summary>
		/// Get the next n characters.
		/// </summary>
		/// <param name="n">     The number of characters to take. </param>
		/// <returns>      A string of n characters. </returns>
		/// <exception cref="JSONException">
		///   Substring bounds error if there are not
		///   n characters remaining in the source string. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String next(int n) throws JSONException
		 public virtual string next(int n)
		 {
			 if (n == 0)
			 {
				 return "";
			 }

			 char[] buffer = new char[n];
			 int pos = 0;

			 while (pos < n)
			 {
				 buffer[pos] = next();
				 if (end())
				 {
					 throw syntaxError("Substring bounds error");
				 }
				 pos += 1;
			 }
			 return new string(buffer);
		 }


		/// <summary>
		/// Get the next char in the string, skipping whitespace. </summary>
		/// <exception cref="JSONException"> </exception>
		/// <returns>  A character, or 0 if there are no more characters. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public char nextClean() throws JSONException
		public virtual char nextClean()
		{
			for (;;)
			{
				char c = next();
				if (c == 0 || c > ' ')
				{
					return c;
				}
			}
		}


		/// <summary>
		/// Return the characters up to the next close quote character.
		/// Backslash processing is done. The formal JSON format does not
		/// allow strings in single quotes, but an implementation is allowed to
		/// accept them. </summary>
		/// <param name="quote"> The quoting character, either
		///      <code>"</code>&nbsp;<small>(double quote)</small> or
		///      <code>'</code>&nbsp;<small>(single quote)</small>. </param>
		/// <returns>      A String. </returns>
		/// <exception cref="JSONException"> Unterminated string. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String nextString(char quote) throws JSONException
		public virtual string nextString(char quote)
		{
			char c;
			StringBuilder sb = new StringBuilder();
			for (;;)
			{
				c = next();
				switch (c)
				{
				case 0:
				case '\n':
				case '\r':
					throw syntaxError("Unterminated string");
				case '\\':
					c = next();
					switch (c)
					{
					case 'b':
						sb.Append('\b');
						break;
					case 't':
						sb.Append('\t');
						break;
					case 'n':
						sb.Append('\n');
						break;
					case 'f':
						sb.Append('\f');
						break;
					case 'r':
						sb.Append('\r');
						break;
					case 'u':
						sb.Append((char)Convert.ToInt32(next(4), 16));
						break;
					case '"':
					case '\'':
					case '\\':
					case '/':
						sb.Append(c);
						break;
					default:
						throw syntaxError("Illegal escape.");
					}
					break;
				default:
					if (c == quote)
					{
						return sb.ToString();
					}
					sb.Append(c);
				break;
				}
			}
		}


		/// <summary>
		/// Get the text up but not including the specified character or the
		/// end of line, whichever comes first. </summary>
		/// <param name="d"> A delimiter character. </param>
		/// <returns>   A string. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String nextTo(char d) throws JSONException
		public virtual string nextTo(char d)
		{
			StringBuilder sb = new StringBuilder();
			for (;;)
			{
				char c = next();
				if (c == d || c == 0 || c == '\n' || c == '\r')
				{
					if (c != 0)
					{
						back();
					}
					return sb.ToString().Trim();
				}
				sb.Append(c);
			}
		}


		/// <summary>
		/// Get the text up but not including one of the specified delimiter
		/// characters or the end of line, whichever comes first. </summary>
		/// <param name="delimiters"> A set of delimiter characters. </param>
		/// <returns> A string, trimmed. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String nextTo(String delimiters) throws JSONException
		public virtual string nextTo(string delimiters)
		{
			char c;
			StringBuilder sb = new StringBuilder();
			for (;;)
			{
				c = next();
				if (delimiters.IndexOf(c) >= 0 || c == 0 || c == '\n' || c == '\r')
				{
					if (c != 0)
					{
						back();
					}
					return sb.ToString().Trim();
				}
				sb.Append(c);
			}
		}


		/// <summary>
		/// Get the next value. The value can be a Boolean, Double, Integer,
		/// JSONArray, JSONObject, Long, or String, or the JSONObject.NULL object. </summary>
		/// <exception cref="JSONException"> If syntax error.
		/// </exception>
		/// <returns> An object. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object nextValue() throws JSONException
		public virtual object nextValue()
		{
			char c = nextClean();
			string s;

			switch (c)
			{
				case '"':
				case '\'':
					return nextString(c);
				case '{':
					back();
					return new JSONObject(this);
				case '[':
				case '(':
					back();
					return new JSONArray(this);
			}

			/*
			 * Handle unquoted text. This could be the values true, false, or
			 * null, or it can be a number. An implementation (such as this one)
			 * is allowed to also accept non-standard forms.
			 *
			 * Accumulate characters until we reach the end of the text or a
			 * formatting character.
			 */

			StringBuilder sb = new StringBuilder();
			while (c >= ' ' && ",:]}/\\\"[{;=#".IndexOf(c) < 0)
			{
				sb.Append(c);
				c = next();
			}
			back();

			s = sb.ToString().Trim();
			if (s.Equals(""))
			{
				throw syntaxError("Missing value");
			}
			return JSONObject.stringToValue(s);
		}


		/// <summary>
		/// Skip characters until the next character is the requested character.
		/// If the requested character is not found, no characters are skipped. </summary>
		/// <param name="to"> A character to skip to. </param>
		/// <returns> The requested character, or zero if the requested character
		/// is not found. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public char skipTo(char to) throws JSONException
		public virtual char skipTo(char to)
		{
			char c;
			try
			{
				int startIndex = this.index;
				int startCharacter = this.character;
				int startLine = this.line;
				reader.mark(int.MaxValue);
				do
				{
					c = next();
					if (c == 0)
					{
						reader.reset();
						this.index = startIndex;
						this.character = startCharacter;
						this.line = startLine;
						return c;
					}
				} while (c != to);
			}
			catch (IOException exc)
			{
				throw new JSONException(exc);
			}

			back();
			return c;
		}


		/// <summary>
		/// Make a JSONException to signal a syntax error.
		/// </summary>
		/// <param name="message"> The error message. </param>
		/// <returns>  A JSONException object, suitable for throwing </returns>
		public virtual JSONException syntaxError(string message)
		{
			return new JSONException(message + ToString());
		}


		/// <summary>
		/// Make a printable string of this JSONTokener.
		/// </summary>
		/// <returns> " at {index} [character {character} line {line}]" </returns>
		public override string ToString()
		{
			return " at " + index + " [character " + this.character + " line " + this.line + "]";
		}
	}
}