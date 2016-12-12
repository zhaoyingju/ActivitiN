using System.Collections;
using System.Text;

namespace org.activiti.engine.impl.util.json
{

	/*
	Copyright (c) 2008 JSON.org
	
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
	/// This provides static methods to convert an XML text into a JSONArray or 
	/// JSONObject, and to covert a JSONArray or JSONObject into an XML text using 
	/// the JsonML transform.
	/// @author JSON.org
	/// @version 2010-02-12
	/// </summary>
	public class JSONML
	{

		/// <summary>
		/// Parse XML values and store them in a JSONArray. </summary>
		/// <param name="x">       The XMLTokener containing the source string. </param>
		/// <param name="arrayForm"> true if array form, false if object form. </param>
		/// <param name="ja">      The JSONArray that is containing the current tag or null
		///     if we are at the outermost level. </param>
		/// <returns> A JSONArray if the value is the outermost tag, otherwise null. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static Object parse(XMLTokener x, boolean arrayForm, JSONArray ja) throws JSONException
		private static object parse(XMLTokener x, bool arrayForm, JSONArray ja)
		{
			string attribute;
			char c;
			string closeTag = null;
			int i;
			JSONArray newja = null;
			JSONObject newjo = null;
			object token;
			string tagName = null;

	// Test for and skip past these forms:
	//      <!-- ... -->
	//      <![  ... ]]>
	//      <!   ...   >
	//      <?   ...  ?>

			while (true)
			{
				token = x.nextContent();
				if (token == XML.LT)
				{
					token = x.nextToken();
					if (token is char?)
					{
						if (token == XML.SLASH)
						{

	// Close tag </

							token = x.nextToken();
							if (!(token is string))
							{
								throw new JSONException("Expected a closing name instead of '" + token + "'.");
							}
							if (x.nextToken() != XML.GT)
							{
								throw x.syntaxError("Misshaped close tag");
							}
							return token;
						}
						else if (token == XML.BANG)
						{

	// <!

							c = x.next();
							if (c == '-')
							{
								if (x.next() == '-')
								{
									x.skipPast("-->");
								}
								x.back();
							}
							else if (c == '[')
							{
								token = x.nextToken();
								if (token.Equals("CDATA") && x.next() == '[')
								{
									if (ja != null)
									{
										ja.put(x.nextCDATA());
									}
								}
								else
								{
									throw x.syntaxError("Expected 'CDATA['");
								}
							}
							else
							{
								i = 1;
								do
								{
									token = x.nextMeta();
									if (token == null)
									{
										throw x.syntaxError("Missing '>' after '<!'.");
									}
									else if (token == XML.LT)
									{
										i += 1;
									}
									else if (token == XML.GT)
									{
										i -= 1;
									}
								} while (i > 0);
							}
						}
						else if (token == XML.QUEST)
						{

	// <?

							x.skipPast("?>");
						}
						else
						{
							throw x.syntaxError("Misshaped tag");
						}

	// Open tag <

					}
					else
					{
						if (!(token is string))
						{
							throw x.syntaxError("Bad tagName '" + token + "'.");
						}
						tagName = (string)token;
						newja = new JSONArray();
						newjo = new JSONObject();
						if (arrayForm)
						{
							newja.put(tagName);
							if (ja != null)
							{
								ja.put(newja);
							}
						}
						else
						{
							newjo.put("tagName", tagName);
							if (ja != null)
							{
								ja.put(newjo);
							}
						}
						token = null;
						for (;;)
						{
							if (token == null)
							{
								token = x.nextToken();
							}
							if (token == null)
							{
								throw x.syntaxError("Misshaped tag");
							}
							if (!(token is string))
							{
								break;
							}

	//		              attribute = value

						attribute = (string)token;
						if (!arrayForm && ("tagName".Equals(attribute) || "childNode".Equals(attribute)))
						{
						  throw x.syntaxError("Reserved attribute.");
						}
						token = x.nextToken();
						if (token == XML.EQ)
						{
							token = x.nextToken();
							if (!(token is string))
							{
								throw x.syntaxError("Missing value");
							}
							newjo.accumulate(attribute, JSONObject.stringToValue((string)token));
							token = null;
						}
						else
						{
							newjo.accumulate(attribute, "");
						}
						}
						if (arrayForm && newjo.length() > 0)
						{
							newja.put(newjo);
						}

	// Empty tag <.../>

						if (token == XML.SLASH)
						{
							if (x.nextToken() != XML.GT)
							{
								throw x.syntaxError("Misshaped tag");
							}
							if (ja == null)
							{
								if (arrayForm)
								{
									return newja;
								}
								else
								{
									return newjo;
								}
							}

	// Content, between <...> and </...>

						}
						else
						{
							if (token != XML.GT)
							{
								throw x.syntaxError("Misshaped tag");
							}
							closeTag = (string)parse(x, arrayForm, newja);
							if (closeTag != null)
							{
								if (!closeTag.Equals(tagName))
								{
									throw x.syntaxError("Mismatched '" + tagName + "' and '" + closeTag + "'");
								}
								tagName = null;
								if (!arrayForm && newja.length() > 0)
								{
									newjo.put("childNodes", newja);
								}
								if (ja == null)
								{
									if (arrayForm)
									{
										return newja;
									}
									else
									{
										return newjo;
									}
								}
							}
						}
					}
				}
				else
				{
					if (ja != null)
					{
						ja.put(token is string ? JSONObject.stringToValue((string)token) : token);
					}
				}
			}
		}


		/// <summary>
		/// Convert a well-formed (but not necessarily valid) XML string into a
		/// JSONArray using the JsonML transform. Each XML tag is represented as
		/// a JSONArray in which the first element is the tag name. If the tag has
		/// attributes, then the second element will be JSONObject containing the
		/// name/value pairs. If the tag contains children, then strings and
		/// JSONArrays will represent the child tags.
		/// Comments, prologs, DTDs, and <code>&lt;[ [ ]]></code> are ignored. </summary>
		/// <param name="string"> The source string. </param>
		/// <returns> A JSONArray containing the structured signalData from the XML string. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JSONArray toJSONArray(String string) throws JSONException
		public static JSONArray toJSONArray(string @string)
		{
			return toJSONArray(new XMLTokener(@string));
		}


		/// <summary>
		/// Convert a well-formed (but not necessarily valid) XML string into a
		/// JSONArray using the JsonML transform. Each XML tag is represented as
		/// a JSONArray in which the first element is the tag name. If the tag has
		/// attributes, then the second element will be JSONObject containing the
		/// name/value pairs. If the tag contains children, then strings and
		/// JSONArrays will represent the child content and tags.
		/// Comments, prologs, DTDs, and <code>&lt;[ [ ]]></code> are ignored. </summary>
		/// <param name="x"> An XMLTokener. </param>
		/// <returns> A JSONArray containing the structured signalData from the XML string. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JSONArray toJSONArray(XMLTokener x) throws JSONException
		public static JSONArray toJSONArray(XMLTokener x)
		{
			return (JSONArray)parse(x, true, null);
		}



		/// <summary>
		/// Convert a well-formed (but not necessarily valid) XML string into a
		/// JSONObject using the JsonML transform. Each XML tag is represented as
		/// a JSONObject with a "tagName" property. If the tag has attributes, then 
		/// the attributes will be in the JSONObject as properties. If the tag 
		/// contains children, the object will have a "childNodes" property which 
		/// will be an array of strings and JsonML JSONObjects.
		/// 
		/// Comments, prologs, DTDs, and <code>&lt;[ [ ]]></code> are ignored. </summary>
		/// <param name="x"> An XMLTokener of the XML source text. </param>
		/// <returns> A JSONObject containing the structured signalData from the XML string. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JSONObject toJSONObject(XMLTokener x) throws JSONException
		public static JSONObject toJSONObject(XMLTokener x)
		{
			   return (JSONObject)parse(x, false, null);
		}
		/// <summary>
		/// Convert a well-formed (but not necessarily valid) XML string into a
		/// JSONObject using the JsonML transform. Each XML tag is represented as
		/// a JSONObject with a "tagName" property. If the tag has attributes, then 
		/// the attributes will be in the JSONObject as properties. If the tag 
		/// contains children, the object will have a "childNodes" property which 
		/// will be an array of strings and JsonML JSONObjects.
		/// 
		/// Comments, prologs, DTDs, and <code>&lt;[ [ ]]></code> are ignored. </summary>
		/// <param name="string"> The XML source text. </param>
		/// <returns> A JSONObject containing the structured signalData from the XML string. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static JSONObject toJSONObject(String string) throws JSONException
		public static JSONObject toJSONObject(string @string)
		{
			return toJSONObject(new XMLTokener(@string));
		}


		/// <summary>
		/// Reverse the JSONML transformation, making an XML text from a JSONArray. </summary>
		/// <param name="ja"> A JSONArray. </param>
		/// <returns> An XML string. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String toString(JSONArray ja) throws JSONException
		public static string ToString(JSONArray ja)
		{
			object e;
			int i;
			JSONObject jo;
			string k;
			IEnumerator keys;
			int length;
			StringBuilder sb = new StringBuilder();
			string tagName;
			string v;

	// Emit <tagName	    

			tagName = ja.getString(0);
			XML.noSpace(tagName);
			tagName = XML.escape(tagName);
			sb.Append('<');
			sb.Append(tagName);

			e = ja.opt(1);
			if (e is JSONObject)
			{
				i = 2;
				jo = (JSONObject)e;

	// Emit the attributes

				keys = jo.keys();
				while (keys.hasNext())
				{
					k = keys.next().ToString();
					XML.noSpace(k);
					v = jo.optString(k);
					if (v != null)
					{
						sb.Append(' ');
						sb.Append(XML.escape(k));
						sb.Append('=');
						sb.Append('"');
						sb.Append(XML.escape(v));
						sb.Append('"');
					}
				}
			}
			else
			{
				i = 1;
			}

	//Emit content in body

			length = ja.length();
			if (i >= length)
			{
				sb.Append('/');
				sb.Append('>');
			}
			else
			{
				sb.Append('>');
				do
				{
					e = ja.get(i);
					i += 1;
					if (e != null)
					{
						if (e is string)
						{
							sb.Append(XML.escape(e.ToString()));
						}
						else if (e is JSONObject)
						{
							sb.Append(ToString((JSONObject)e));
						}
						else if (e is JSONArray)
						{
							sb.Append(ToString((JSONArray)e));
						}
					}
				} while (i < length);
				sb.Append('<');
				sb.Append('/');
				sb.Append(tagName);
				sb.Append('>');
			}
			return sb.ToString();
		}

		/// <summary>
		/// Reverse the JSONML transformation, making an XML text from a JSONObject.
		/// The JSONObject must contain a "tagName" property. If it has children, 
		/// then it must have a "childNodes" property containing an array of objects. 
		/// The other properties are attributes with string values. </summary>
		/// <param name="jo"> A JSONObject. </param>
		/// <returns> An XML string. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static String toString(JSONObject jo) throws JSONException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static string ToString(JSONObject jo)
		{
			StringBuilder sb = new StringBuilder();
			object e;
			int i;
			JSONArray ja;
			string k;
			IEnumerator keys;
			int len;
			string tagName;
			string v;

	//Emit <tagName

			tagName = jo.optString("tagName");
			if (tagName == null)
			{
				return XML.escape(jo.ToString());
			}
			XML.noSpace(tagName);
			tagName = XML.escape(tagName);
			sb.Append('<');
			sb.Append(tagName);

	//Emit the attributes

			keys = jo.keys();
			while (keys.hasNext())
			{
				k = keys.next().ToString();
				if (!k.Equals("tagName") && !k.Equals("childNodes"))
				{
					XML.noSpace(k);
					v = jo.optString(k);
					if (v != null)
					{
						sb.Append(' ');
						sb.Append(XML.escape(k));
						sb.Append('=');
						sb.Append('"');
						sb.Append(XML.escape(v));
						sb.Append('"');
					}
				}
			}

	//Emit content in body

			ja = jo.optJSONArray("childNodes");
			if (ja == null)
			{
				sb.Append('/');
				sb.Append('>');
			}
			else
			{
				sb.Append('>');
				len = ja.length();
				for (i = 0; i < len; i += 1)
				{
					e = ja.get(i);
					if (e != null)
					{
						if (e is string)
						{
							sb.Append(XML.escape(e.ToString()));
						}
						else if (e is JSONObject)
						{
							sb.Append(ToString((JSONObject)e));
						}
						else if (e is JSONArray)
						{
							sb.Append(ToString((JSONArray)e));
						}
					}
				}
				sb.Append('<');
				sb.Append('/');
				sb.Append(tagName);
				sb.Append('>');
			}
			return sb.ToString();
		}
	}
}