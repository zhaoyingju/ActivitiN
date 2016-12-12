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
	/// Convert a web browser cookie specification to a JSONObject and back.
	/// JSON and Cookies are both notations for name/value pairs.
	/// @author JSON.org
	/// @version 2008-09-18
	/// </summary>
	public class Cookie
	{

		/// <summary>
		/// Produce a copy of a string in which the characters '+', '%', '=', ';'
		/// and control characters are replaced with "%hh". This is a gentle form
		/// of URL encoding, attempting to cause as little distortion to the
		/// string as possible. The characters '=' and ';' are meta characters in
		/// cookies. By convention, they are escaped using the URL-encoding. This is
		/// only a convention, not a standard. Often, cookies are expected to have
		/// encoded values. We encode '=' and ';' because we must. We encode '%' and
		/// '+' because they are meta characters in URL encoding. </summary>
		/// <param name="string"> The source string. </param>
		/// <returns>       The escaped result. </returns>
		public static string escape(string @string)
		{
			char c;
			string s = @string.Trim();
			StringBuilder sb = new StringBuilder();
			int len = s.Length;
			for (int i = 0; i < len; i += 1)
			{
				c = s[i];
				if (c < ' ' || c == '+' || c == '%' || c == '=' || c == ';')
				{
					sb.Append('%');
					sb.Append(char.forDigit((char)(((int)((uint)c >> 4)) & 0x0f), 16));
					sb.Append(char.forDigit((char)(c & 0x0f), 16));
				}
				else
				{
					sb.Append(c);
				}
			}
			return sb.ToString();
		}


		/// <summary>
		/// Convert a cookie specification string into a JSONObject. The string
		/// will contain a name value pair separated by '='. The name and the value
		/// will be unescaped, possibly converting '+' and '%' sequences. The
		/// cookie properties may follow, separated by ';', also represented as
		/// name=value (except the secure property, which does not have a value).
		/// The name will be stored under the key "name", and the value will be
		/// stored under the key "value". This method does not do checking or
		/// validation of the parameters. It only converts the cookie string into
		/// a JSONObject. </summary>
		/// <param name="string"> The cookie specification string. </param>
		/// <returns> A JSONObject containing "name", "value", and possibly other
		///  members. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JSONObject toJSONObject(String string) throws JSONException
		public static JSONObject toJSONObject(string @string)
		{
			string n;
			JSONObject o = new JSONObject();
			object v;
			JSONTokener x = new JSONTokener(@string);
			o.put("name", x.nextTo('='));
			x.next('=');
			o.put("value", x.nextTo(';'));
			x.next();
			while (x.more())
			{
				n = unescape(x.nextTo("=;"));
				if (x.next() != '=')
				{
					if (n.Equals("secure"))
					{
						v = true;
					}
					else
					{
						throw x.syntaxError("Missing '=' in cookie parameter.");
					}
				}
				else
				{
					v = unescape(x.nextTo(';'));
					x.next();
				}
				o.put(n, v);
			}
			return o;
		}


		/// <summary>
		/// Convert a JSONObject into a cookie specification string. The JSONObject
		/// must contain "name" and "value" members.
		/// If the JSONObject contains "expires", "domain", "path", or "secure"
		/// members, they will be appended to the cookie specification string.
		/// All other members are ignored. </summary>
		/// <param name="o"> A JSONObject </param>
		/// <returns> A cookie specification string </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String toString(JSONObject o) throws JSONException
		public static string ToString(JSONObject o)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(escape(o.getString("name")));
			sb.Append("=");
			sb.Append(escape(o.getString("value")));
			if (o.has("expires"))
			{
				sb.Append(";expires=");
				sb.Append(o.getString("expires"));
			}
			if (o.has("domain"))
			{
				sb.Append(";domain=");
				sb.Append(escape(o.getString("domain")));
			}
			if (o.has("path"))
			{
				sb.Append(";path=");
				sb.Append(escape(o.getString("path")));
			}
			if (o.optBoolean("secure"))
			{
				sb.Append(";secure");
			}
			return sb.ToString();
		}

		/// <summary>
		/// Convert <code>%</code><i>hh</i> sequences to single characters, and
		/// convert plus to space. </summary>
		/// <param name="s"> A string that may contain
		///      <code>+</code>&nbsp;<small>(plus)</small> and
		///      <code>%</code><i>hh</i> sequences. </param>
		/// <returns> The unescaped string. </returns>
		public static string unescape(string s)
		{
			int len = s.Length;
			StringBuilder b = new StringBuilder();
			for (int i = 0; i < len; ++i)
			{
				char c = s[i];
				if (c == '+')
				{
					c = ' ';
				}
				else if (c == '%' && i + 2 < len)
				{
					int d = JSONTokener.dehexchar(s[i + 1]);
					int e = JSONTokener.dehexchar(s[i + 2]);
					if (d >= 0 && e >= 0)
					{
						c = (char)(d * 16 + e);
						i += 2;
					}
				}
				b.Append(c);
			}
			return b.ToString();
		}
	}

}