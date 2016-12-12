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
	/// This provides static methods to convert comma delimited text into a
	/// JSONArray, and to covert a JSONArray into comma delimited text. Comma
	/// delimited text is a very popular format for signalData interchange. It is
	/// understood by most database, spreadsheet, and organizer programs.
	/// <para>
	/// Each row of text represents a row in a table or a signalData record. Each row
	/// ends with a NEWLINE character. Each row contains one or more values.
	/// Values are separated by commas. A value can contain any character except
	/// for comma, unless is is wrapped in single quotes or double quotes.
	/// </para>
	/// <para>
	/// The first row usually contains the names of the columns.
	/// </para>
	/// <para>
	/// A comma delimited list can be converted into a JSONArray of JSONObjects.
	/// The names for the elements in the JSONObjects can be taken from the names
	/// in the first row.
	/// @author JSON.org
	/// @version 2009-09-11
	/// </para>
	/// </summary>
	public class CDL
	{

		/// <summary>
		/// Get the next value. The value can be wrapped in quotes. The value can
		/// be empty. </summary>
		/// <param name="x"> A JSONTokener of the source text. </param>
		/// <returns> The value string, or null if empty. </returns>
		/// <exception cref="JSONException"> if the quoted string is badly formed. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static String getValue(JSONTokener x) throws JSONException
		private static string getValue(JSONTokener x)
		{
			char c;
			char q;
			StringBuilder sb;
			do
			{
				c = x.next();
			} while (c == ' ' || c == '\t');
			switch (c)
			{
			case 0:
				return null;
			case '"':
			case '\'':
				q = c;
				sb = new StringBuilder();
				for (;;)
				{
					c = x.next();
					if (c == q)
					{
						break;
					}
					if (c == 0 || c == '\n' || c == '\r')
					{
						throw x.syntaxError("Missing close quote '" + q + "'.");
					}
					sb.Append(c);
				}
				return sb.ToString();
			case ',':
				x.back();
				return "";
			default:
				x.back();
				return x.nextTo(',');
			}
		}

		/// <summary>
		/// Produce a JSONArray of strings from a row of comma delimited values. </summary>
		/// <param name="x"> A JSONTokener of the source text. </param>
		/// <returns> A JSONArray of strings. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JSONArray rowToJSONArray(JSONTokener x) throws JSONException
		public static JSONArray rowToJSONArray(JSONTokener x)
		{
			JSONArray ja = new JSONArray();
			for (;;)
			{
				string value = getValue(x);
				char c = x.next();
				if (value == null || (ja.length() == 0 && value.Length == 0 && c != ','))
				{
					return null;
				}
				ja.put(value);
				for (;;)
				{
					if (c == ',')
					{
						break;
					}
					if (c != ' ')
					{
						if (c == '\n' || c == '\r' || c == 0)
						{
							return ja;
						}
						throw x.syntaxError("Bad character '" + c + "' (" + (int)c + ").");
					}
					c = x.next();
				}
			}
		}

		/// <summary>
		/// Produce a JSONObject from a row of comma delimited text, using a
		/// parallel JSONArray of strings to provides the names of the elements. </summary>
		/// <param name="names"> A JSONArray of names. This is commonly obtained from the
		///  first row of a comma delimited text file using the rowToJSONArray
		///  method. </param>
		/// <param name="x"> A JSONTokener of the source text. </param>
		/// <returns> A JSONObject combining the names and values. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JSONObject rowToJSONObject(JSONArray names, JSONTokener x) throws JSONException
		public static JSONObject rowToJSONObject(JSONArray names, JSONTokener x)
		{
			JSONArray ja = rowToJSONArray(x);
			return ja != null ? ja.toJSONObject(names) : null;
		}

		/// <summary>
		/// Produce a JSONArray of JSONObjects from a comma delimited text string,
		/// using the first row as a source of names. </summary>
		/// <param name="string"> The comma delimited text. </param>
		/// <returns> A JSONArray of JSONObjects. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JSONArray toJSONArray(String string) throws JSONException
		public static JSONArray toJSONArray(string @string)
		{
			return toJSONArray(new JSONTokener(@string));
		}

		/// <summary>
		/// Produce a JSONArray of JSONObjects from a comma delimited text string,
		/// using the first row as a source of names. </summary>
		/// <param name="x"> The JSONTokener containing the comma delimited text. </param>
		/// <returns> A JSONArray of JSONObjects. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JSONArray toJSONArray(JSONTokener x) throws JSONException
		public static JSONArray toJSONArray(JSONTokener x)
		{
			return toJSONArray(rowToJSONArray(x), x);
		}

		/// <summary>
		/// Produce a JSONArray of JSONObjects from a comma delimited text string
		/// using a supplied JSONArray as the source of element names. </summary>
		/// <param name="names"> A JSONArray of strings. </param>
		/// <param name="string"> The comma delimited text. </param>
		/// <returns> A JSONArray of JSONObjects. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JSONArray toJSONArray(JSONArray names, String string) throws JSONException
		public static JSONArray toJSONArray(JSONArray names, string @string)
		{
			return toJSONArray(names, new JSONTokener(@string));
		}

		/// <summary>
		/// Produce a JSONArray of JSONObjects from a comma delimited text string
		/// using a supplied JSONArray as the source of element names. </summary>
		/// <param name="names"> A JSONArray of strings. </param>
		/// <param name="x"> A JSONTokener of the source text. </param>
		/// <returns> A JSONArray of JSONObjects. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JSONArray toJSONArray(JSONArray names, JSONTokener x) throws JSONException
		public static JSONArray toJSONArray(JSONArray names, JSONTokener x)
		{
			if (names == null || names.length() == 0)
			{
				return null;
			}
			JSONArray ja = new JSONArray();
			for (;;)
			{
				JSONObject jo = rowToJSONObject(names, x);
				if (jo == null)
				{
					break;
				}
				ja.put(jo);
			}
			if (ja.length() == 0)
			{
				return null;
			}
			return ja;
		}


		/// <summary>
		/// Produce a comma delimited text row from a JSONArray. Values containing
		/// the comma character will be quoted. Troublesome characters may be 
		/// removed. </summary>
		/// <param name="ja"> A JSONArray of strings. </param>
		/// <returns> A string ending in NEWLINE. </returns>
		public static string rowToString(JSONArray ja)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < ja.length(); i += 1)
			{
				if (i > 0)
				{
					sb.Append(',');
				}
				object o = ja.opt(i);
				if (o != null)
				{
					string s = o.ToString();
					if (s.Length > 0 && (s.IndexOf(',') >= 0 || s.IndexOf('\n') >= 0 || s.IndexOf('\r') >= 0 || s.IndexOf(0) >= 0 || s[0] == '"'))
					{
						sb.Append('"');
						int length = s.Length;
						for (int j = 0; j < length; j += 1)
						{
							char c = s[j];
							if (c >= ' ' && c != '"')
							{
								sb.Append(c);
							}
						}
						sb.Append('"');
					}
					else
					{
						sb.Append(s);
					}
				}
			}
			sb.Append('\n');
			return sb.ToString();
		}

		/// <summary>
		/// Produce a comma delimited text from a JSONArray of JSONObjects. The
		/// first row will be a list of names obtained by inspecting the first
		/// JSONObject. </summary>
		/// <param name="ja"> A JSONArray of JSONObjects. </param>
		/// <returns> A comma delimited text. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String toString(JSONArray ja) throws JSONException
		public static string ToString(JSONArray ja)
		{
			JSONObject jo = ja.optJSONObject(0);
			if (jo != null)
			{
				JSONArray names = jo.names();
				if (names != null)
				{
					return rowToString(names) + ToString(names, ja);
				}
			}
			return null;
		}

		/// <summary>
		/// Produce a comma delimited text from a JSONArray of JSONObjects using
		/// a provided list of names. The list of names is not included in the
		/// output. </summary>
		/// <param name="names"> A JSONArray of strings. </param>
		/// <param name="ja"> A JSONArray of JSONObjects. </param>
		/// <returns> A comma delimited text. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String toString(JSONArray names, JSONArray ja) throws JSONException
		public static string ToString(JSONArray names, JSONArray ja)
		{
			if (names == null || names.length() == 0)
			{
				return null;
			}
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < ja.length(); i += 1)
			{
				JSONObject jo = ja.optJSONObject(i);
				if (jo != null)
				{
					sb.Append(rowToString(jo.toJSONArray(names)));
				}
			}
			return sb.ToString();
		}
	}

}