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
	/// The HTTPTokener extends the JSONTokener to provide additional methods
	/// for the parsing of HTTP headers.
	/// @author JSON.org
	/// @version 2008-09-18
	/// </summary>
	public class HTTPTokener : JSONTokener
	{

		/// <summary>
		/// Construct an HTTPTokener from a string. </summary>
		/// <param name="s"> A source string. </param>
		public HTTPTokener(string s) : base(s)
		{
		}


		/// <summary>
		/// Get the next token or string. This is used in parsing HTTP headers. </summary>
		/// <exception cref="JSONException"> </exception>
		/// <returns> A String. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String nextToken() throws JSONException
		public virtual string nextToken()
		{
			char c;
			char q;
			StringBuilder sb = new StringBuilder();
			do
			{
				c = next();
			} while (char.IsWhiteSpace(c));
			if (c == '"' || c == '\'')
			{
				q = c;
				for (;;)
				{
					c = next();
					if (c < ' ')
					{
						throw syntaxError("Unterminated string.");
					}
					if (c == q)
					{
						return sb.ToString();
					}
					sb.Append(c);
				}
			}
			for (;;)
			{
				if (c == 0 || char.IsWhiteSpace(c))
				{
					return sb.ToString();
				}
				sb.Append(c);
				c = next();
			}
		}
	}

}