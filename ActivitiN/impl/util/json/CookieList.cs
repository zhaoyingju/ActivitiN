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
	/// Convert a web browser cookie list string to a JSONObject and back.
	/// @author JSON.org
	/// @version 2008-09-18
	/// </summary>
	public class CookieList
	{

		/// <summary>
		/// Convert a cookie list into a JSONObject. A cookie list is a sequence
		/// of name/value pairs. The names are separated from the values by '='.
		/// The pairs are separated by ';'. The names and the values
		/// will be unescaped, possibly converting '+' and '%' sequences.
		/// 
		/// To add a cookie to a cooklist,
		/// cookielistJSONObject.put(cookieJSONObject.getString("name"),
		///     cookieJSONObject.getString("value")); </summary>
		/// <param name="string">  A cookie list string </param>
		/// <returns> A JSONObject </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JSONObject toJSONObject(String string) throws JSONException
		public static JSONObject toJSONObject(string @string)
		{
			JSONObject o = new JSONObject();
			JSONTokener x = new JSONTokener(@string);
			while (x.more())
			{
				string name = Cookie.unescape(x.nextTo('='));
				x.next('=');
				o.put(name, Cookie.unescape(x.nextTo(';')));
				x.next();
			}
			return o;
		}


		/// <summary>
		/// Convert a JSONObject into a cookie list. A cookie list is a sequence
		/// of name/value pairs. The names are separated from the values by '='.
		/// The pairs are separated by ';'. The characters '%', '+', '=', and ';'
		/// in the names and values are replaced by "%hh". </summary>
		/// <param name="o"> A JSONObject </param>
		/// <returns> A cookie list string </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static String toString(JSONObject o) throws JSONException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static string ToString(JSONObject o)
		{
			bool b = false;
			IEnumerator keys = o.keys();
			string s;
			StringBuilder sb = new StringBuilder();
			while (keys.hasNext())
			{
				s = keys.next().ToString();
				if (!o.isNull(s))
				{
					if (b)
					{
						sb.Append(';');
					}
					sb.Append(Cookie.escape(s));
					sb.Append("=");
					sb.Append(Cookie.escape(o.getString(s)));
					b = true;
				}
			}
			return sb.ToString();
		}
	}

}