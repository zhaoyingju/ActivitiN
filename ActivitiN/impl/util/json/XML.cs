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
	/// This provides static methods to convert an XML text into a JSONObject,
	/// and to covert a JSONObject into an XML text.
	/// @author JSON.org
	/// @version 2009-12-12
	/// </summary>
	public class XML
	{

		/// <summary>
		/// The Character '&'. </summary>
		public static readonly char? AMP = new char?('&');

		/// <summary>
		/// The Character '''. </summary>
		public static readonly char? APOS = new char?('\'');

		/// <summary>
		/// The Character '!'. </summary>
		public static readonly char? BANG = new char?('!');

		/// <summary>
		/// The Character '='. </summary>
		public static readonly char? EQ = new char?('=');

		/// <summary>
		/// The Character '>'. </summary>
		public static readonly char? GT = new char?('>');

		/// <summary>
		/// The Character '<'. </summary>
		public static readonly char? LT = new char?('<');

		/// <summary>
		/// The Character '?'. </summary>
		public static readonly char? QUEST = new char?('?');

		/// <summary>
		/// The Character '"'. </summary>
		public static readonly char? QUOT = new char?('"');

		/// <summary>
		/// The Character '/'. </summary>
		public static readonly char? SLASH = new char?('/');

		/// <summary>
		/// Replace special characters with XML escapes:
		/// <pre>
		/// &amp; <small>(ampersand)</small> is replaced by &amp;amp;
		/// &lt; <small>(less than)</small> is replaced by &amp;lt;
		/// &gt; <small>(greater than)</small> is replaced by &amp;gt;
		/// &quot; <small>(double quote)</small> is replaced by &amp;quot;
		/// </pre> </summary>
		/// <param name="string"> The string to be escaped. </param>
		/// <returns> The escaped string. </returns>
		public static string escape(string @string)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0, len = @string.Length; i < len; i++)
			{
				char c = @string[i];
				switch (c)
				{
				case '&':
					sb.Append("&amp;");
					break;
				case '<':
					sb.Append("&lt;");
					break;
				case '>':
					sb.Append("&gt;");
					break;
				case '"':
					sb.Append("&quot;");
					break;
				default:
					sb.Append(c);
				break;
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Throw an exception if the string contains whitespace. 
		/// Whitespace is not allowed in tagNames and attributes. </summary>
		/// <param name="string"> </param>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void noSpace(String string) throws JSONException
		public static void noSpace(string @string)
		{
			int i , length = @string.Length;
			if (length == 0)
			{
				throw new JSONException("Empty string.");
			}
			for (i = 0; i < length; i += 1)
			{
				if (char.IsWhiteSpace(@string[i]))
				{
					throw new JSONException("'" + @string + "' contains a space character.");
				}
			}
		}

		/// <summary>
		/// Scan the content following the named tag, attaching it to the context. </summary>
		/// <param name="x">       The XMLTokener containing the source string. </param>
		/// <param name="context"> The JSONObject that will include the new material. </param>
		/// <param name="name">    The tag name. </param>
		/// <returns> true if the close tag is processed. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static boolean parse(XMLTokener x, JSONObject context, String name) throws JSONException
		private static bool parse(XMLTokener x, JSONObject context, string name)
		{
			char c;
			int i;
			string n;
			JSONObject o = null;
			string s;
			object t;

	// Test for and skip past these forms:
	//      <!-- ... -->
	//      <!   ...   >
	//      <![  ... ]]>
	//      <?   ...  ?>
	// Report errors for these forms:
	//      <>
	//      <=
	//      <<

			t = x.nextToken();

	// <!

			if (t == BANG)
			{
				c = x.next();
				if (c == '-')
				{
					if (x.next() == '-')
					{
						x.skipPast("-->");
						return false;
					}
					x.back();
				}
				else if (c == '[')
				{
					t = x.nextToken();
					if (t.Equals("CDATA"))
					{
						if (x.next() == '[')
						{
							s = x.nextCDATA();
							if (s.Length > 0)
							{
								context.accumulate("content", s);
							}
							return false;
						}
					}
					throw x.syntaxError("Expected 'CDATA['");
				}
				i = 1;
				do
				{
					t = x.nextMeta();
					if (t == null)
					{
						throw x.syntaxError("Missing '>' after '<!'.");
					}
					else if (t == LT)
					{
						i += 1;
					}
					else if (t == GT)
					{
						i -= 1;
					}
				} while (i > 0);
				return false;
			}
			else if (t == QUEST)
			{

	// <?

				x.skipPast("?>");
				return false;
			}
			else if (t == SLASH)
			{

	// Close tag </

				t = x.nextToken();
				if (name == null)
				{
					throw x.syntaxError("Mismatched close tag" + t);
				}
				if (!t.Equals(name))
				{
					throw x.syntaxError("Mismatched " + name + " and " + t);
				}
				if (x.nextToken() != GT)
				{
					throw x.syntaxError("Misshaped close tag");
				}
				return true;

			}
			else if (t is char?)
			{
				throw x.syntaxError("Misshaped tag");

	// Open tag <

			}
			else
			{
				n = (string)t;
				t = null;
				o = new JSONObject();
				for (;;)
				{
					if (t == null)
					{
						t = x.nextToken();
					}

	// attribute = value

					if (t is string)
					{
						s = (string)t;
						t = x.nextToken();
						if (t == EQ)
						{
							t = x.nextToken();
							if (!(t is string))
							{
								throw x.syntaxError("Missing value");
							}
							o.accumulate(s, JSONObject.stringToValue((string)t));
							t = null;
						}
						else
						{
							o.accumulate(s, "");
						}

	// Empty tag <.../>

					}
					else if (t == SLASH)
					{
						if (x.nextToken() != GT)
						{
							throw x.syntaxError("Misshaped tag");
						}
						context.accumulate(n, "");
						return false;

	// Content, between <...> and </...>

					}
					else if (t == GT)
					{
						for (;;)
						{
							t = x.nextContent();
							if (t == null)
							{
								if (n != null)
								{
									throw x.syntaxError("Unclosed tag " + n);
								}
								return false;
							}
							else if (t is string)
							{
								s = (string)t;
								if (s.Length > 0)
								{
									o.accumulate("content", JSONObject.stringToValue(s));
								}

	// Nested element

							}
							else if (t == LT)
							{
								if (parse(x, o, n))
								{
									if (o.length() == 0)
									{
										context.accumulate(n, "");
									}
									else if (o.length() == 1 && o.opt("content") != null)
									{
										context.accumulate(n, o.opt("content"));
									}
									else
									{
										context.accumulate(n, o);
									}
									return false;
								}
							}
						}
					}
					else
					{
						throw x.syntaxError("Misshaped tag");
					}
				}
			}
		}


		/// <summary>
		/// Convert a well-formed (but not necessarily valid) XML string into a
		/// JSONObject. Some information may be lost in this transformation
		/// because JSON is a signalData format and XML is a document format. XML uses
		/// elements, attributes, and content text, while JSON uses unordered
		/// collections of name/value pairs and arrays of values. JSON does not
		/// does not like to distinguish between elements and attributes.
		/// Sequences of similar elements are represented as JSONArrays. Content
		/// text may be placed in a "content" member. Comments, prologs, DTDs, and
		/// <code>&lt;[ [ ]]></code> are ignored. </summary>
		/// <param name="string"> The source string. </param>
		/// <returns> A JSONObject containing the structured signalData from the XML string. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JSONObject toJSONObject(String string) throws JSONException
		public static JSONObject toJSONObject(string @string)
		{
			JSONObject o = new JSONObject();
			XMLTokener x = new XMLTokener(@string);
			while (x.more() && x.skipPast("<"))
			{
				parse(x, o, null);
			}
			return o;
		}


		/// <summary>
		/// Convert a JSONObject into a well-formed, element-normal XML string. </summary>
		/// <param name="o"> A JSONObject. </param>
		/// <returns>  A string. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String toString(Object o) throws JSONException
		public static string ToString(object o)
		{
			return ToString(o, null);
		}


		/// <summary>
		/// Convert a JSONObject into a well-formed, element-normal XML string. </summary>
		/// <param name="o"> A JSONObject. </param>
		/// <param name="tagName"> The optional name of the enclosing tag. </param>
		/// <returns> A string. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String toString(Object o, String tagName) throws JSONException
		public static string ToString(object o, string tagName)
		{
			StringBuilder b = new StringBuilder();
			int i;
			JSONArray ja;
			JSONObject jo;
			string k;
			IEnumerator keys;
			int len;
			string s;
			object v;
			if (o is JSONObject)
			{

	// Emit <tagName>

				if (tagName != null)
				{
					b.Append('<');
					b.Append(tagName);
					b.Append('>');
				}

	// Loop thru the keys.

				jo = (JSONObject)o;
				keys = jo.keys();
				while (keys.hasNext())
				{
					k = keys.next().ToString();
					v = jo.opt(k);
					if (v == null)
					{
						v = "";
					}
					if (v is string)
					{
						s = (string)v;
					}
					else
					{
						s = null;
					}

	// Emit content in body

					if (k.Equals("content"))
					{
						if (v is JSONArray)
						{
							ja = (JSONArray)v;
							len = ja.length();
							for (i = 0; i < len; i += 1)
							{
								if (i > 0)
								{
									b.Append('\n');
								}
								b.Append(escape(ja.get(i).ToString()));
							}
						}
						else
						{
							b.Append(escape(v.ToString()));
						}

	// Emit an array of similar keys

					}
					else if (v is JSONArray)
					{
						ja = (JSONArray)v;
						len = ja.length();
						for (i = 0; i < len; i += 1)
						{
							v = ja.get(i);
							if (v is JSONArray)
							{
								b.Append('<');
								b.Append(k);
								b.Append('>');
								b.Append(ToString(v));
								b.Append("</");
								b.Append(k);
								b.Append('>');
							}
							else
							{
								b.Append(ToString(v, k));
							}
						}
					}
					else if (v.Equals(""))
					{
						b.Append('<');
						b.Append(k);
						b.Append("/>");

	// Emit a new tag <k>

					}
					else
					{
						b.Append(ToString(v, k));
					}
				}
				if (tagName != null)
				{

	// Emit the </tagname> close tag

					b.Append("</");
					b.Append(tagName);
					b.Append('>');
				}
				return b.ToString();

	// XML does not have good support for arrays. If an array appears in a place
	// where XML is lacking, synthesize an <array> element.

			}
			else if (o is JSONArray)
			{
				ja = (JSONArray)o;
				len = ja.length();
				for (i = 0; i < len; ++i)
				{
					v = ja.opt(i);
					b.Append(ToString(v, (tagName == null) ? "array" : tagName));
				}
				return b.ToString();
			}
			else
			{
				s = (o == null) ? "null" : escape(o.ToString());
				return (tagName == null) ? "\"" + s + "\"" : (s.Length == 0) ? "<" + tagName + "/>" : "<" + tagName + ">" + s + "</" + tagName + ">";
			}
		}
	}
}