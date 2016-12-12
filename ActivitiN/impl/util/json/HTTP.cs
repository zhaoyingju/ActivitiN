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
	/// Convert an HTTP header to a JSONObject and back.
	/// @author JSON.org
	/// @version 2008-09-18
	/// </summary>
	public class HTTP
	{

		/// <summary>
		/// Carriage return/line feed. </summary>
		public const string CRLF = "\r\n";

		/// <summary>
		/// Convert an HTTP header string into a JSONObject. It can be a request
		/// header or a response header. A request header will contain
		/// <pre>{
		///    Method: "POST" (for example),
		///    "Request-URI": "/" (for example),
		///    "HTTP-Version": "HTTP/1.1" (for example)
		/// }</pre>
		/// A response header will contain
		/// <pre>{
		///    "HTTP-Version": "HTTP/1.1" (for example),
		///    "Status-Code": "200" (for example),
		///    "Reason-Phrase": "OK" (for example)
		/// }</pre>
		/// In addition, the other parameters in the header will be captured, using
		/// the HTTP field names as JSON names, so that <pre>
		///    Date: Sun, 26 May 2002 18:06:04 GMT
		///    Cookie: Q=q2=PPEAsg--; B=677gi6ouf29bn&b=2&f=s
		///    Cache-Control: no-cache</pre>
		/// become
		/// <pre>{...
		///    Date: "Sun, 26 May 2002 18:06:04 GMT",
		///    Cookie: "Q=q2=PPEAsg--; B=677gi6ouf29bn&b=2&f=s",
		///    "Cache-Control": "no-cache",
		/// ...}</pre>
		/// It does no further checking or conversion. It does not parse dates.
		/// It does not do '%' transforms on URLs. </summary>
		/// <param name="string"> An HTTP header string. </param>
		/// <returns> A JSONObject containing the elements and attributes
		/// of the XML string. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JSONObject toJSONObject(String string) throws JSONException
		public static JSONObject toJSONObject(string @string)
		{
			JSONObject o = new JSONObject();
			HTTPTokener x = new HTTPTokener(@string);
			string t;

			t = x.nextToken();
			if (t.ToUpper().StartsWith("HTTP"))
			{

	// Response

				o.put("HTTP-Version", t);
				o.put("Status-Code", x.nextToken());
				o.put("Reason-Phrase", x.nextTo('\0'));
				x.next();

			}
			else
			{

	// Request

				o.put("Method", t);
				o.put("Request-URI", x.nextToken());
				o.put("HTTP-Version", x.nextToken());
			}

	// Fields

			while (x.more())
			{
				string name = x.nextTo(':');
				x.next(':');
				o.put(name, x.nextTo('\0'));
				x.next();
			}
			return o;
		}


		/// <summary>
		/// Convert a JSONObject into an HTTP header. A request header must contain
		/// <pre>{
		///    Method: "POST" (for example),
		///    "Request-URI": "/" (for example),
		///    "HTTP-Version": "HTTP/1.1" (for example)
		/// }</pre>
		/// A response header must contain
		/// <pre>{
		///    "HTTP-Version": "HTTP/1.1" (for example),
		///    "Status-Code": "200" (for example),
		///    "Reason-Phrase": "OK" (for example)
		/// }</pre>
		/// Any other members of the JSONObject will be output as HTTP fields.
		/// The result will end with two CRLF pairs. </summary>
		/// <param name="o"> A JSONObject </param>
		/// <returns> An HTTP header string. </returns>
		/// <exception cref="JSONException"> if the object does not contain enough
		///  information. </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static String toString(JSONObject o) throws JSONException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static string ToString(JSONObject o)
		{
			IEnumerator keys = o.keys();
			string s;
			StringBuilder sb = new StringBuilder();
			if (o.has("Status-Code") && o.has("Reason-Phrase"))
			{
				sb.Append(o.getString("HTTP-Version"));
				sb.Append(' ');
				sb.Append(o.getString("Status-Code"));
				sb.Append(' ');
				sb.Append(o.getString("Reason-Phrase"));
			}
			else if (o.has("Method") && o.has("Request-URI"))
			{
				sb.Append(o.getString("Method"));
				sb.Append(' ');
				sb.Append('"');
				sb.Append(o.getString("Request-URI"));
				sb.Append('"');
				sb.Append(' ');
				sb.Append(o.getString("HTTP-Version"));
			}
			else
			{
				throw new JSONException("Not enough material for an HTTP header.");
			}
			sb.Append(CRLF);
			while (keys.hasNext())
			{
				s = keys.next().ToString();
				if (!s.Equals("HTTP-Version") && !s.Equals("Status-Code") && !s.Equals("Reason-Phrase") && !s.Equals("Method") && !s.Equals("Request-URI") && !o.isNull(s))
				{
					sb.Append(s);
					sb.Append(": ");
					sb.Append(o.getString(s));
					sb.Append(CRLF);
				}
			}
			sb.Append(CRLF);
			return sb.ToString();
		}
	}

}