using System.Collections;
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
	/// The XMLTokener extends the JSONTokener to provide additional methods
	/// for the parsing of XML texts.
	/// @author JSON.org
	/// @version 2010-01-30
	/// </summary>
	public class XMLTokener : JSONTokener
	{


	   /// <summary>
	   /// The table of entity values. It initially contains Character values for
	   /// amp, apos, gt, lt, quot.
	   /// </summary>
	   public static readonly Hashtable entity;

	   static XMLTokener()
	   {
		   entity = new Hashtable(8);
		   entity["amp"] = XML.AMP;
		   entity["apos"] = XML.APOS;
		   entity["gt"] = XML.GT;
		   entity["lt"] = XML.LT;
		   entity["quot"] = XML.QUOT;
	   }

		/// <summary>
		/// Construct an XMLTokener from a string. </summary>
		/// <param name="s"> A source string. </param>
		public XMLTokener(string s) : base(s)
		{
		}

		/// <summary>
		/// Get the text in the CDATA block. </summary>
		/// <returns> The string up to the <code>]]&gt;</code>. </returns>
		/// <exception cref="JSONException"> If the <code>]]&gt;</code> is not found. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String nextCDATA() throws JSONException
		public virtual string nextCDATA()
		{
			char c;
			int i;
			StringBuilder sb = new StringBuilder();
			for (;;)
			{
				c = next();
				if (end())
				{
					throw syntaxError("Unclosed CDATA");
				}
				sb.Append(c);
				i = sb.Length - 3;
				if (i >= 0 && sb[i] == ']' && sb[i + 1] == ']' && sb[i + 2] == '>')
				{
					sb.Length = i;
					return sb.ToString();
				}
			}
		}


		/// <summary>
		/// Get the next XML outer token, trimming whitespace. There are two kinds
		/// of tokens: the '<' character which begins a markup tag, and the content
		/// text between markup tags.
		/// </summary>
		/// <returns>  A string, or a '<' Character, or null if there is no more
		/// source text. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object nextContent() throws JSONException
		public virtual object nextContent()
		{
			char c;
			StringBuilder sb;
			do
			{
				c = next();
			} while (char.IsWhiteSpace(c));
			if (c == 0)
			{
				return null;
			}
			if (c == '<')
			{
				return XML.LT;
			}
			sb = new StringBuilder();
			for (;;)
			{
				if (c == '<' || c == 0)
				{
					back();
					return sb.ToString().Trim();
				}
				if (c == '&')
				{
					sb.Append(nextEntity(c));
				}
				else
				{
					sb.Append(c);
				}
				c = next();
			}
		}


		/// <summary>
		/// Return the next entity. These entities are translated to Characters:
		///     <code>&amp;  &apos;  &gt;  &lt;  &quot;</code>. </summary>
		/// <param name="a"> An ampersand character. </param>
		/// <returns>  A Character or an entity String if the entity is not recognized. </returns>
		/// <exception cref="JSONException"> If missing ';' in XML entity. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object nextEntity(char a) throws JSONException
		public virtual object nextEntity(char a)
		{
			StringBuilder sb = new StringBuilder();
			for (;;)
			{
				char c = next();
				if (char.IsLetterOrDigit(c) || c == '#')
				{
					sb.Append(char.ToLower(c));
				}
				else if (c == ';')
				{
					break;
				}
				else
				{
					throw syntaxError("Missing ';' in XML entity: &" + sb);
				}
			}
			string s = sb.ToString();
			object e = entity[s];
			return e != null ? e : a + s + ";";
		}


		/// <summary>
		/// Returns the next XML meta token. This is used for skipping over <!...>
		/// and <?...?> structures. </summary>
		/// <returns> Syntax characters (<code>< > / = ! ?</code>) are returned as
		///  Character, and strings and names are returned as Boolean. We don't care
		///  what the values actually are. </returns>
		/// <exception cref="JSONException"> If a string is not properly closed or if the XML
		///  is badly structured. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object nextMeta() throws JSONException
		public virtual object nextMeta()
		{
			char c;
			char q;
			do
			{
				c = next();
			} while (char.IsWhiteSpace(c));
			switch (c)
			{
			case 0:
				throw syntaxError("Misshaped meta tag");
			case '<':
				return XML.LT;
			case '>':
				return XML.GT;
			case '/':
				return XML.SLASH;
			case '=':
				return XML.EQ;
			case '!':
				return XML.BANG;
			case '?':
				return XML.QUEST;
			case '"':
			case '\'':
				q = c;
				for (;;)
				{
					c = next();
					if (c == 0)
					{
						throw syntaxError("Unterminated string");
					}
					if (c == q)
					{
						return true;
					}
				}
				goto default;
			default:
				for (;;)
				{
					c = next();
					if (char.IsWhiteSpace(c))
					{
						return true;
					}
					switch (c)
					{
					case 0:
					case '<':
					case '>':
					case '/':
					case '=':
					case '!':
					case '?':
					case '"':
					case '\'':
						back();
						return true;
					}
				}
			break;
			}
		}


		/// <summary>
		/// Get the next XML Token. These tokens are found inside of angle
		/// brackets. It may be one of these characters: <code>/ > = ! ?</code> or it
		/// may be a string wrapped in single quotes or double quotes, or it may be a
		/// name. </summary>
		/// <returns> a String or a Character. </returns>
		/// <exception cref="JSONException"> If the XML is not well formed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object nextToken() throws JSONException
		public virtual object nextToken()
		{
			char c;
			char q;
			StringBuilder sb;
			do
			{
				c = next();
			} while (char.IsWhiteSpace(c));
			switch (c)
			{
			case 0:
				throw syntaxError("Misshaped element");
			case '<':
				throw syntaxError("Misplaced '<'");
			case '>':
				return XML.GT;
			case '/':
				return XML.SLASH;
			case '=':
				return XML.EQ;
			case '!':
				return XML.BANG;
			case '?':
				return XML.QUEST;

	// Quoted string

			case '"':
			case '\'':
				q = c;
				sb = new StringBuilder();
				for (;;)
				{
					c = next();
					if (c == 0)
					{
						throw syntaxError("Unterminated string");
					}
					if (c == q)
					{
						return sb.ToString();
					}
					if (c == '&')
					{
						sb.Append(nextEntity(c));
					}
					else
					{
						sb.Append(c);
					}
				}
				goto default;
			default:

	// Name

				sb = new StringBuilder();
				for (;;)
				{
					sb.Append(c);
					c = next();
					if (char.IsWhiteSpace(c))
					{
						return sb.ToString();
					}
					switch (c)
					{
					case 0:
						return sb.ToString();
					case '>':
					case '/':
					case '=':
					case '!':
					case '?':
					case '[':
					case ']':
						back();
						return sb.ToString();
					case '<':
					case '"':
					case '\'':
						throw syntaxError("Bad character in a name");
					}
				}
			break;
			}
		}


		/// <summary>
		/// Skip characters until past the requested string.
		/// If it is not found, we are left at the end of the source with a result of false. </summary>
		/// <param name="to"> A string to skip past. </param>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean skipPast(String to) throws JSONException
		public virtual bool skipPast(string to)
		{
			bool b;
			char c;
			int i;
			int j;
			int offset = 0;
			int n = to.Length;
			char[] circle = new char[n];

			/*
			 * First fill the circle buffer with as many characters as are in the
			 * to string. If we reach an early end, bail.
			 */

			for (i = 0; i < n; i += 1)
			{
				c = next();
				if (c == 0)
				{
					return false;
				}
				circle[i] = c;
			}
			/*
			 * We will loop, possibly for all of the remaining characters.
			 */
			for (;;)
			{
				j = offset;
				b = true;
				/*
				 * Compare the circle buffer with the to string. 
				 */
				for (i = 0; i < n; i += 1)
				{
					if (circle[j] != to[i])
					{
						b = false;
						break;
					}
					j += 1;
					if (j >= n)
					{
						j -= n;
					}
				}
				/*
				 * If we exit the loop with b intact, then victory is ours.
				 */
				if (b)
				{
					return true;
				}
				/*
				 * Get the next character. If there isn't one, then defeat is ours.
				 */
				c = next();
				if (c == 0)
				{
					return false;
				}
				/*
				 * Shove the character in the circle buffer and advance the 
				 * circle offset. The offset is mod n.
				 */
				circle[offset] = c;
				offset += 1;
				if (offset >= n)
				{
					offset -= n;
				}
			}
		}
	}

}