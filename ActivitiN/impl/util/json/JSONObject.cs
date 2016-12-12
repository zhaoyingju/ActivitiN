using System;
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
	/// A JSONObject is an unordered collection of name/value pairs. Its
	/// external form is a string wrapped in curly braces with colons between the
	/// names and values, and commas between the values and names. The internal form
	/// is an object having <code>get</code> and <code>opt</code> methods for
	/// accessing the values by name, and <code>put</code> methods for adding or
	/// replacing values by name. The values can be any of these types:
	/// <code>Boolean</code>, <code>JSONArray</code>, <code>JSONObject</code>,
	/// <code>Number</code>, <code>String</code>, or the <code>JSONObject.NULL</code>
	/// object. A JSONObject constructor can be used to convert an external form
	/// JSON text into an internal form whose values can be retrieved with the
	/// <code>get</code> and <code>opt</code> methods, or to convert values into a
	/// JSON text using the <code>put</code> and <code>toString</code> methods.
	/// A <code>get</code> method returns a value if one can be found, and throws an
	/// exception if one cannot be found. An <code>opt</code> method returns a
	/// default value instead of throwing an exception, and so is useful for
	/// obtaining optional values.
	/// <para>
	/// The generic <code>get()</code> and <code>opt()</code> methods return an
	/// object, which you can cast or query for type. There are also typed
	/// <code>get</code> and <code>opt</code> methods that do type checking and type
	/// coercion for you.
	/// </para>
	/// <para>
	/// The <code>put</code> methods adds values to an object. For example, <pre>
	///     myString = new JSONObject().put("JSON", "Hello, World!").toString();</pre>
	/// produces the string <code>{"JSON": "Hello, World"}</code>.
	/// </para>
	/// <para>
	/// The texts produced by the <code>toString</code> methods strictly conform to
	/// the JSON syntax rules.
	/// The constructors are more forgiving in the texts they will accept:
	/// <ul>
	/// <li>An extra <code>,</code>&nbsp;<small>(comma)</small> may appear just
	///     before the closing brace.</li>
	/// <li>Strings may be quoted with <code>'</code>&nbsp;<small>(single
	///     quote)</small>.</li>
	/// <li>Strings do not need to be quoted at all if they do not begin with a quote
	///     or single quote, and if they do not contain leading or trailing spaces,
	///     and if they do not contain any of these characters:
	///     <code>{ } [ ] / \ : , = ; #</code> and if they do not look like numbers
	///     and if they are not the reserved words <code>true</code>,
	///     <code>false</code>, or <code>null</code>.</li>
	/// <li>Keys can be followed by <code>=</code> or <code>=></code> as well as
	///     by <code>:</code>.</li>
	/// <li>Values can be followed by <code>;</code> <small>(semicolon)</small> as
	///     well as by <code>,</code> <small>(comma)</small>.</li>
	/// <li>Numbers may have the <code>0-</code> <small>(octal)</small> or
	///     <code>0x-</code> <small>(hex)</small> prefix.</li>
	/// </ul>
	/// @author JSON.org
	/// @version 2010-01-05
	/// </para>
	/// </summary>
	public class JSONObject
	{

		/// <summary>
		/// JSONObject.NULL is equivalent to the value that JavaScript calls null,
		/// whilst Java's null is equivalent to the value that JavaScript calls
		/// undefined.
		/// </summary>
		 private sealed class Null
		 {

			/// <summary>
			/// There is only intended to be a single instance of the NULL object,
			/// so the clone method returns itself. </summary>
			/// <returns>     NULL. </returns>
			protected internal object clone()
			{
				return this;
			}


			/// <summary>
			/// A Null object is equal to the null value and to itself. </summary>
			/// <param name="object">    An object to test for nullness. </param>
			/// <returns> true if the object parameter is the JSONObject.NULL object
			///  or null. </returns>
			public override bool Equals(object @object)
			{
				return @object == null || @object == this;
			}


			/// <summary>
			/// Get the "null" string value. </summary>
			/// <returns> The string "null". </returns>
			public override string ToString()
			{
				return "null";
			}
		 }


		/// <summary>
		/// The map where the JSONObject's properties are kept.
		/// </summary>
		private IDictionary map;


		/// <summary>
		/// It is sometimes more convenient and less ambiguous to have a
		/// <code>NULL</code> object than to use Java's <code>null</code> value.
		/// <code>JSONObject.NULL.equals(null)</code> returns <code>true</code>.
		/// <code>JSONObject.NULL.toString()</code> returns <code>"null"</code>.
		/// </summary>
		public static readonly object NULL = new Null();


		/// <summary>
		/// Construct an empty JSONObject.
		/// </summary>
		public JSONObject()
		{
			this.map = new Hashtable();
		}


		/// <summary>
		/// Construct a JSONObject from a subset of another JSONObject.
		/// An array of strings is used to identify the keys that should be copied.
		/// Missing keys are ignored. </summary>
		/// <param name="jo"> A JSONObject. </param>
		/// <param name="names"> An array of strings. </param>
		/// <exception cref="JSONException"> </exception>
		/// <exception cref="JSONException"> If a value is a non-finite number or if a name is duplicated. </exception>
		public JSONObject(JSONObject jo, string[] names) : this()
		{
			for (int i = 0; i < names.Length; i += 1)
			{
				try
				{
					putOnce(names[i], jo.opt(names[i]));
				}
				catch (Exception)
				{
				}
			}
		}


		/// <summary>
		/// Construct a JSONObject from a JSONTokener. </summary>
		/// <param name="x"> A JSONTokener object containing the source string. </param>
		/// <exception cref="JSONException"> If there is a syntax error in the source string
		///  or a duplicated key. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject(JSONTokener x) throws JSONException
		public JSONObject(JSONTokener x) : this()
		{
			char c;
			string key;

			if (x.nextClean() != '{')
			{
				throw x.syntaxError("A JSONObject text must begin with '{'");
			}
			for (;;)
			{
				c = x.nextClean();
				switch (c)
				{
				case 0:
					throw x.syntaxError("A JSONObject text must end with '}'");
				case '}':
					return;
				default:
					x.back();
					key = x.nextValue().ToString();
				break;
				}

				/*
				 * The key is followed by ':'. We will also tolerate '=' or '=>'.
				 */

				c = x.nextClean();
				if (c == '=')
				{
					if (x.next() != '>')
					{
						x.back();
					}
				}
				else if (c != ':')
				{
					throw x.syntaxError("Expected a ':' after a key");
				}
				putOnce(key, x.nextValue());

				/*
				 * Pairs are separated by ','. We will also tolerate ';'.
				 */

				switch (x.nextClean())
				{
				case ';':
				case ',':
					if (x.nextClean() == '}')
					{
						return;
					}
					x.back();
					break;
				case '}':
					return;
				default:
					throw x.syntaxError("Expected a ',' or '}'");
				}
			}
		}


		/// <summary>
		/// Construct a JSONObject from a Map.
		/// </summary>
		/// <param name="map"> A map object that can be used to initialize the contents of
		///  the JSONObject. </param>
		/// <exception cref="JSONException">  </exception>
		public JSONObject(IDictionary map)
		{
			this.map = new Hashtable();
			if (map != null)
			{
				IEnumerator i = map.GetEnumerator();
				while (i.hasNext())
				{
					DictionaryEntry e = (DictionaryEntry)i.next();
					this.map[e.Key] = wrap(e.Value);
				}
			}
		}


		/// <summary>
		/// Construct a JSONObject from an Object using bean getters.
		/// It reflects on all of the public methods of the object.
		/// For each of the methods with no parameters and a name starting
		/// with <code>"get"</code> or <code>"is"</code> followed by an uppercase letter,
		/// the method is invoked, and a key and the value returned from the getter method
		/// are put into the new JSONObject.
		/// 
		/// The key is formed by removing the <code>"get"</code> or <code>"is"</code> prefix.
		/// If the second remaining character is not upper case, then the first
		/// character is converted to lower case.
		/// 
		/// For example, if an object has a method named <code>"getName"</code>, and
		/// if the result of calling <code>object.getName()</code> is <code>"Larry Fine"</code>,
		/// then the JSONObject will contain <code>"name": "Larry Fine"</code>.
		/// </summary>
		/// <param name="bean"> An object that has getter methods that should be used
		/// to make a JSONObject. </param>
		public JSONObject(object bean) : this()
		{
			populateMap(bean);
		}


		/// <summary>
		/// Construct a JSONObject from an Object, using reflection to find the
		/// public members. The resulting JSONObject's keys will be the strings
		/// from the names array, and the values will be the field values associated
		/// with those keys in the object. If a key is not found or not visible,
		/// then it will not be copied into the new JSONObject. </summary>
		/// <param name="object"> An object that has fields that should be used to make a
		/// JSONObject. </param>
		/// <param name="names"> An array of strings, the names of the fields to be obtained
		/// from the object. </param>
		public JSONObject(object @object, string[] names) : this()
		{
			Type c = @object.GetType();
			for (int i = 0; i < names.Length; i += 1)
			{
				string name = names[i];
				try
				{
					putOpt(name, c.GetField(name).get(@object));
				}
				catch (Exception)
				{
				}
			}
		}


		/// <summary>
		/// Construct a JSONObject from a source JSON text string.
		/// This is the most commonly used JSONObject constructor. </summary>
		/// <param name="source">    A string beginning
		///  with <code>{</code>&nbsp;<small>(left brace)</small> and ending
		///  with <code>}</code>&nbsp;<small>(right brace)</small>. </param>
		/// <exception cref="JSONException"> If there is a syntax error in the source
		///  string or a duplicated key. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject(String source) throws JSONException
		public JSONObject(string source) : this(new JSONTokener(source))
		{
		}


		/// <summary>
		/// Accumulate values under a key. It is similar to the put method except
		/// that if there is already an object stored under the key then a
		/// JSONArray is stored under the key to hold all of the accumulated values.
		/// If there is already a JSONArray, then the new value is appended to it.
		/// In contrast, the put method replaces the previous value. </summary>
		/// <param name="key">   A key string. </param>
		/// <param name="value"> An object to be accumulated under the key. </param>
		/// <returns> this. </returns>
		/// <exception cref="JSONException"> If the value is an invalid number
		///  or if the key is null. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject accumulate(String key, Object value) throws JSONException
		public virtual JSONObject accumulate(string key, object value)
		{
			testValidity(value);
			object o = opt(key);
			if (o == null)
			{
				put(key, value is JSONArray ? (new JSONArray()).put(value) : value);
			}
			else if (o is JSONArray)
			{
				((JSONArray)o).put(value);
			}
			else
			{
				put(key, (new JSONArray()).put(o).put(value));
			}
			return this;
		}


		/// <summary>
		/// Append values to the array under a key. If the key does not exist in the
		/// JSONObject, then the key is put in the JSONObject with its value being a
		/// JSONArray containing the value parameter. If the key was already
		/// associated with a JSONArray, then the value parameter is appended to it. </summary>
		/// <param name="key">   A key string. </param>
		/// <param name="value"> An object to be accumulated under the key. </param>
		/// <returns> this. </returns>
		/// <exception cref="JSONException"> If the key is null or if the current value
		///  associated with the key is not a JSONArray. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject append(String key, Object value) throws JSONException
		public virtual JSONObject append(string key, object value)
		{
			testValidity(value);
			object o = opt(key);
			if (o == null)
			{
				put(key, (new JSONArray()).put(value));
			}
			else if (o is JSONArray)
			{
				put(key, ((JSONArray)o).put(value));
			}
			else
			{
				throw new JSONException("JSONObject[" + key + "] is not a JSONArray.");
			}
			return this;
		}


		/// <summary>
		/// Produce a string from a double. The string "null" will be returned if
		/// the number is not finite. </summary>
		/// <param name="d"> A double. </param>
		/// <returns> A String. </returns>
		public static string doubleToString(double d)
		{
			if (double.IsInfinity(d) || double.IsNaN(d))
			{
				return "null";
			}

	// Shave off trailing zeros and decimal point, if possible.

			string s = Convert.ToString(d);
			if (s.IndexOf('.') > 0 && s.IndexOf('e') < 0 && s.IndexOf('E') < 0)
			{
				while (s.EndsWith("0"))
				{
					s = s.Substring(0, s.Length - 1);
				}
				if (s.EndsWith("."))
				{
					s = s.Substring(0, s.Length - 1);
				}
			}
			return s;
		}


		/// <summary>
		/// Get the value object associated with a key.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      The object associated with the key. </returns>
		/// <exception cref="JSONException"> if the key is not found. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object get(String key) throws JSONException
		public virtual object get(string key)
		{
			object o = opt(key);
			if (o == null)
			{
				throw new JSONException("JSONObject[" + quote(key) + "] not found.");
			}
			return o;
		}


		/// <summary>
		/// Get the boolean value associated with a key.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      The truth. </returns>
		/// <exception cref="JSONException">
		///  if the value is not a Boolean or the String "true" or "false". </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean getBoolean(String key) throws JSONException
		public virtual bool getBoolean(string key)
		{
			object o = get(key);
			if (o.Equals(false) || (o is string && ((string)o).Equals("false", StringComparison.CurrentCultureIgnoreCase)))
			{
				return false;
			}
			else if (o.Equals(true) || (o is string && ((string)o).Equals("true", StringComparison.CurrentCultureIgnoreCase)))
			{
				return true;
			}
			throw new JSONException("JSONObject[" + quote(key) + "] is not a Boolean.");
		}


		/// <summary>
		/// Get the double value associated with a key. </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      The numeric value. </returns>
		/// <exception cref="JSONException"> if the key is not found or
		///  if the value is not a Number object and cannot be converted to a number. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getDouble(String key) throws JSONException
		public virtual double getDouble(string key)
		{
			object o = get(key);
			try
			{
				return o is Number ? (double)((Number)o) : (double)Convert.ToDouble((string)o);
			}
			catch (Exception)
			{
				throw new JSONException("JSONObject[" + quote(key) + "] is not a number.");
			}
		}


		/// <summary>
		/// Get the int value associated with a key. 
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      The integer value. </returns>
		/// <exception cref="JSONException"> if the key is not found or if the value cannot
		///  be converted to an integer. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getInt(String key) throws JSONException
		public virtual int getInt(string key)
		{
			object o = get(key);
			try
			{
				return o is Number ? (int)((Number)o) : Convert.ToInt32((string)o);
			}
			catch (Exception)
			{
				throw new JSONException("JSONObject[" + quote(key) + "] is not an int.");
			}
		}


		/// <summary>
		/// Get the JSONArray value associated with a key.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      A JSONArray which is the value. </returns>
		/// <exception cref="JSONException"> if the key is not found or
		///  if the value is not a JSONArray. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONArray getJSONArray(String key) throws JSONException
		public virtual JSONArray getJSONArray(string key)
		{
			object o = get(key);
			if (o is JSONArray)
			{
				return (JSONArray)o;
			}
			throw new JSONException("JSONObject[" + quote(key) + "] is not a JSONArray.");
		}


		/// <summary>
		/// Get the JSONObject value associated with a key.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      A JSONObject which is the value. </returns>
		/// <exception cref="JSONException"> if the key is not found or
		///  if the value is not a JSONObject. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject getJSONObject(String key) throws JSONException
		public virtual JSONObject getJSONObject(string key)
		{
			object o = get(key);
			if (o is JSONObject)
			{
				return (JSONObject)o;
			}
			throw new JSONException("JSONObject[" + quote(key) + "] is not a JSONObject.");
		}


		/// <summary>
		/// Get the long value associated with a key. 
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      The long value. </returns>
		/// <exception cref="JSONException"> if the key is not found or if the value cannot
		///  be converted to a long. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long getLong(String key) throws JSONException
		public virtual long getLong(string key)
		{
			object o = get(key);
			try
			{
				return o is Number ? (long)((Number)o) : Convert.ToInt64((string)o);
			}
			catch (Exception)
			{
				throw new JSONException("JSONObject[" + quote(key) + "] is not a long.");
			}
		}


		/// <summary>
		/// Get an array of field names from a JSONObject.
		/// </summary>
		/// <returns> An array of field names, or null if there are no names. </returns>
		public static string[] getNames(JSONObject jo)
		{
			int length = jo.length();
			if (length == 0)
			{
				return null;
			}
			IEnumerator i = jo.keys();
			string[] names = new string[length];
			int j = 0;
			while (i.hasNext())
			{
				names[j] = (string)i.next();
				j += 1;
			}
			return names;
		}


		/// <summary>
		/// Get an array of field names from an Object.
		/// </summary>
		/// <returns> An array of field names, or null if there are no names. </returns>
		public static string[] getNames(object @object)
		{
			if (@object == null)
			{
				return null;
			}
			Type klass = @object.GetType();
			Field[] fields = klass.GetFields();
			int length = fields.Length;
			if (length == 0)
			{
				return null;
			}
			string[] names = new string[length];
			for (int i = 0; i < length; i += 1)
			{
				names[i] = fields[i].Name;
			}
			return names;
		}


		/// <summary>
		/// Get the string associated with a key.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      A string which is the value. </returns>
		/// <exception cref="JSONException"> if the key is not found. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getString(String key) throws JSONException
		public virtual string getString(string key)
		{
			return get(key).ToString();
		}


		/// <summary>
		/// Determine if the JSONObject contains a specific key. </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      true if the key exists in the JSONObject. </returns>
		public virtual bool has(string key)
		{
			return this.map.Contains(key);
		}


		/// <summary>
		/// Increment a property of a JSONObject. If there is no such property,
		/// create one with a value of 1. If there is such a property, and if
		/// it is an Integer, Long, Double, or Float, then add one to it. </summary>
		/// <param name="key">  A key string. </param>
		/// <returns> this. </returns>
		/// <exception cref="JSONException"> If there is already a property with this name
		/// that is not an Integer, Long, Double, or Float. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject increment(String key) throws JSONException
		public virtual JSONObject increment(string key)
		{
			object value = opt(key);
			if (value == null)
			{
				put(key, 1);
			}
			else
			{
				if (value is int?)
				{
					put(key, (int)((int?)value) + 1);
				}
				else if (value is long?)
				{
					put(key, (long)((long?)value) + 1);
				}
				else if (value is double?)
				{
					put(key, (double)((double?)value) + 1);
				}
				else if (value is float?)
				{
					put(key, (float)((float?)value) + 1);
				}
				else
				{
					throw new JSONException("Unable to increment [" + key + "].");
				}
			}
			return this;
		}


		/// <summary>
		/// Determine if the value associated with the key is null or if there is
		///  no value. </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      true if there is no value associated with the key or if
		///  the value is the JSONObject.NULL object. </returns>
		public virtual bool isNull(string key)
		{
			return JSONObject.NULL.Equals(opt(key));
		}


		/// <summary>
		/// Get an enumeration of the keys of the JSONObject.
		/// </summary>
		/// <returns> An iterator of the keys. </returns>
		public virtual IEnumerator keys()
		{
			return this.map.Keys.GetEnumerator();
		}


		/// <summary>
		/// Get the number of keys stored in the JSONObject.
		/// </summary>
		/// <returns> The number of keys in the JSONObject. </returns>
		public virtual int length()
		{
			return this.map.Count;
		}


		/// <summary>
		/// Produce a JSONArray containing the names of the elements of this
		/// JSONObject. </summary>
		/// <returns> A JSONArray containing the key strings, or null if the JSONObject
		/// is empty. </returns>
		public virtual JSONArray names()
		{
			JSONArray ja = new JSONArray();
			IEnumerator keys = keys();
			while (keys.hasNext())
			{
				ja.put(keys.next());
			}
			return ja.length() == 0 ? null : ja;
		}

		/// <summary>
		/// Produce a string from a Number. </summary>
		/// <param name="n"> A Number </param>
		/// <returns> A String. </returns>
		/// <exception cref="JSONException"> If n is a non-finite number. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static String numberToString(Number n) throws JSONException
		public static string numberToString(Number n)
		{
			if (n == null)
			{
				throw new JSONException("Null pointer");
			}
			testValidity(n);

	// Shave off trailing zeros and decimal point, if possible.

			string s = n.ToString();
			if (s.IndexOf('.') > 0 && s.IndexOf('e') < 0 && s.IndexOf('E') < 0)
			{
				while (s.EndsWith("0"))
				{
					s = s.Substring(0, s.Length - 1);
				}
				if (s.EndsWith("."))
				{
					s = s.Substring(0, s.Length - 1);
				}
			}
			return s;
		}


		/// <summary>
		/// Get an optional value associated with a key. </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      An object which is the value, or null if there is no value. </returns>
		public virtual object opt(string key)
		{
			return key == null ? null : this.map[key];
		}


		/// <summary>
		/// Get an optional boolean associated with a key.
		/// It returns false if there is no such key, or if the value is not
		/// Boolean.TRUE or the String "true".
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      The truth. </returns>
		public virtual bool optBoolean(string key)
		{
			return optBoolean(key, false);
		}


		/// <summary>
		/// Get an optional boolean associated with a key.
		/// It returns the defaultValue if there is no such key, or if it is not
		/// a Boolean or the String "true" or "false" (case insensitive).
		/// </summary>
		/// <param name="key">              A key string. </param>
		/// <param name="defaultValue">     The default. </param>
		/// <returns>      The truth. </returns>
		public virtual bool optBoolean(string key, bool defaultValue)
		{
			try
			{
				return getBoolean(key);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}


		/// <summary>
		/// Get an optional double associated with a key,
		/// or NaN if there is no such key or if its value is not a number.
		/// If the value is a string, an attempt will be made to evaluate it as
		/// a number.
		/// </summary>
		/// <param name="key">   A string which is the key. </param>
		/// <returns>      An object which is the value. </returns>
		public virtual double optDouble(string key)
		{
			return optDouble(key, double.NaN);
		}


		/// <summary>
		/// Get an optional double associated with a key, or the
		/// defaultValue if there is no such key or if its value is not a number.
		/// If the value is a string, an attempt will be made to evaluate it as
		/// a number.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <param name="defaultValue">     The default. </param>
		/// <returns>      An object which is the value. </returns>
		public virtual double optDouble(string key, double defaultValue)
		{
			try
			{
				object o = opt(key);
				return o is Number ? (double)((Number)o) : (double)(Convert.ToDouble((string)o));
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}


		/// <summary>
		/// Get an optional int value associated with a key,
		/// or zero if there is no such key or if the value is not a number.
		/// If the value is a string, an attempt will be made to evaluate it as
		/// a number.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      An object which is the value. </returns>
		public virtual int optInt(string key)
		{
			return optInt(key, 0);
		}


		/// <summary>
		/// Get an optional int value associated with a key,
		/// or the default if there is no such key or if the value is not a number.
		/// If the value is a string, an attempt will be made to evaluate it as
		/// a number.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <param name="defaultValue">     The default. </param>
		/// <returns>      An object which is the value. </returns>
		public virtual int optInt(string key, int defaultValue)
		{
			try
			{
				return getInt(key);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}


		/// <summary>
		/// Get an optional JSONArray associated with a key.
		/// It returns null if there is no such key, or if its value is not a
		/// JSONArray.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      A JSONArray which is the value. </returns>
		public virtual JSONArray optJSONArray(string key)
		{
			object o = opt(key);
			return o is JSONArray ? (JSONArray)o : null;
		}


		/// <summary>
		/// Get an optional JSONObject associated with a key.
		/// It returns null if there is no such key, or if its value is not a
		/// JSONObject.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      A JSONObject which is the value. </returns>
		public virtual JSONObject optJSONObject(string key)
		{
			object o = opt(key);
			return o is JSONObject ? (JSONObject)o : null;
		}


		/// <summary>
		/// Get an optional long value associated with a key,
		/// or zero if there is no such key or if the value is not a number.
		/// If the value is a string, an attempt will be made to evaluate it as
		/// a number.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      An object which is the value. </returns>
		public virtual long optLong(string key)
		{
			return optLong(key, 0);
		}


		/// <summary>
		/// Get an optional long value associated with a key,
		/// or the default if there is no such key or if the value is not a number.
		/// If the value is a string, an attempt will be made to evaluate it as
		/// a number.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <param name="defaultValue">     The default. </param>
		/// <returns>      An object which is the value. </returns>
		public virtual long optLong(string key, long defaultValue)
		{
			try
			{
				return getLong(key);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}


		/// <summary>
		/// Get an optional string associated with a key.
		/// It returns an empty string if there is no such key. If the value is not
		/// a string and is not null, then it is coverted to a string.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <returns>      A string which is the value. </returns>
		public virtual string optString(string key)
		{
			return optString(key, "");
		}


		/// <summary>
		/// Get an optional string associated with a key.
		/// It returns the defaultValue if there is no such key.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <param name="defaultValue">     The default. </param>
		/// <returns>      A string which is the value. </returns>
		public virtual string optString(string key, string defaultValue)
		{
			object o = opt(key);
			return o != null ? o.ToString() : defaultValue;
		}


		private void populateMap(object bean)
		{
			Type klass = bean.GetType();

	// If klass is a System class then set includeSuperClass to false. 

			bool includeSuperClass = klass.ClassLoader != null;

			Method[] methods = (includeSuperClass) ? klass.GetMethods() : klass.DeclaredMethods;
			for (int i = 0; i < methods.Length; i += 1)
			{
				try
				{
					Method method = methods[i];
					if (Modifier.isPublic(method.Modifiers))
					{
						string name = method.Name;
						string key = "";
						if (name.StartsWith("get"))
						{
							if (name.Equals("getClass") || name.Equals("getDeclaringClass"))
							{
								key = "";
							}
							else
							{
								key = name.Substring(3);
							}
						}
						else if (name.StartsWith("is"))
						{
							key = name.Substring(2);
						}
						if (key.Length > 0 && char.IsUpper(key[0]) && method.ParameterTypes.length == 0)
						{
							if (key.Length == 1)
							{
								key = key.ToLower();
							}
							else if (!char.IsUpper(key[1]))
							{
								key = key.Substring(0, 1).ToLower() + key.Substring(1);
							}

							object result = method.invoke(bean, (object[])null);

							map[key] = wrap(result);
						}
					}
				}
				catch (Exception)
				{
				}
			}
		}


		/// <summary>
		/// Put a key/boolean pair in the JSONObject.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <param name="value"> A boolean which is the value. </param>
		/// <returns> this. </returns>
		/// <exception cref="JSONException"> If the key is null. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject put(String key, boolean value) throws JSONException
		public virtual JSONObject put(string key, bool value)
		{
			put(key, value ? true : false);
			return this;
		}


		/// <summary>
		/// Put a key/value pair in the JSONObject, where the value will be a
		/// JSONArray which is produced from a Collection. </summary>
		/// <param name="key">   A key string. </param>
		/// <param name="value"> A Collection value. </param>
		/// <returns>      this. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject put(String key, java.util.Collection value) throws JSONException
		public virtual JSONObject put(string key, ICollection value)
		{
			put(key, new JSONArray(value));
			return this;
		}


		/// <summary>
		/// Put a key/double pair in the JSONObject.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <param name="value"> A double which is the value. </param>
		/// <returns> this. </returns>
		/// <exception cref="JSONException"> If the key is null or if the number is invalid. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject put(String key, double value) throws JSONException
		public virtual JSONObject put(string key, double value)
		{
			put(key, new double?(value));
			return this;
		}


		/// <summary>
		/// Put a key/int pair in the JSONObject.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <param name="value"> An int which is the value. </param>
		/// <returns> this. </returns>
		/// <exception cref="JSONException"> If the key is null. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject put(String key, int value) throws JSONException
		public virtual JSONObject put(string key, int value)
		{
			put(key, new int?(value));
			return this;
		}


		/// <summary>
		/// Put a key/long pair in the JSONObject.
		/// </summary>
		/// <param name="key">   A key string. </param>
		/// <param name="value"> A long which is the value. </param>
		/// <returns> this. </returns>
		/// <exception cref="JSONException"> If the key is null. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject put(String key, long value) throws JSONException
		public virtual JSONObject put(string key, long value)
		{
			put(key, new long?(value));
			return this;
		}


		/// <summary>
		/// Put a key/value pair in the JSONObject, where the value will be a
		/// JSONObject which is produced from a Map. </summary>
		/// <param name="key">   A key string. </param>
		/// <param name="value"> A Map value. </param>
		/// <returns>      this. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject put(String key, java.util.Map value) throws JSONException
		public virtual JSONObject put(string key, IDictionary value)
		{
			put(key, new JSONObject(value));
			return this;
		}


		/// <summary>
		/// Put a key/value pair in the JSONObject. If the value is null,
		/// then the key will be removed from the JSONObject if it is present. </summary>
		/// <param name="key">   A key string. </param>
		/// <param name="value"> An object which is the value. It should be of one of these
		///  types: Boolean, Double, Integer, JSONArray, JSONObject, Long, String,
		///  or the JSONObject.NULL object. </param>
		/// <returns> this. </returns>
		/// <exception cref="JSONException"> If the value is non-finite number
		///  or if the key is null. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject put(String key, Object value) throws JSONException
		public virtual JSONObject put(string key, object value)
		{
			if (key == null)
			{
				throw new JSONException("Null key.");
			}
			if (value != null)
			{
				testValidity(value);
				this.map[key] = value;
			}
			else
			{
				remove(key);
			}
			return this;
		}


		/// <summary>
		/// Put a key/value pair in the JSONObject, but only if the key and the
		/// value are both non-null, and only if there is not already a member
		/// with that name. </summary>
		/// <param name="key"> </param>
		/// <param name="value"> </param>
		/// <returns> his. </returns>
		/// <exception cref="JSONException"> if the key is a duplicate </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject putOnce(String key, Object value) throws JSONException
		public virtual JSONObject putOnce(string key, object value)
		{
			if (key != null && value != null)
			{
				if (opt(key) != null)
				{
					throw new JSONException("Duplicate key \"" + key + "\"");
				}
				put(key, value);
			}
			return this;
		}


		/// <summary>
		/// Put a key/value pair in the JSONObject, but only if the
		/// key and the value are both non-null. </summary>
		/// <param name="key">   A key string. </param>
		/// <param name="value"> An object which is the value. It should be of one of these
		///  types: Boolean, Double, Integer, JSONArray, JSONObject, Long, String,
		///  or the JSONObject.NULL object. </param>
		/// <returns> this. </returns>
		/// <exception cref="JSONException"> If the value is a non-finite number. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject putOpt(String key, Object value) throws JSONException
		public virtual JSONObject putOpt(string key, object value)
		{
			if (key != null && value != null)
			{
				put(key, value);
			}
			return this;
		}


		/// <summary>
		/// Produce a string in double quotes with backslash sequences in all the
		/// right places. A backslash will be inserted within </, allowing JSON
		/// text to be delivered in HTML. In JSON text, a string cannot contain a
		/// control character or an unescaped quote or backslash. </summary>
		/// <param name="string"> A String </param>
		/// <returns>  A String correctly formatted for insertion in a JSON text. </returns>
		public static string quote(string @string)
		{
			if (@string == null || @string.Length == 0)
			{
				return "\"\"";
			}

			char b;
			char c = (char)0;
			int i;
			int len = @string.Length;
			StringBuilder sb = new StringBuilder(len + 4);
			string t;

			sb.Append('"');
			for (i = 0; i < len; i += 1)
			{
				b = c;
				c = @string[i];
				switch (c)
				{
				case '\\':
				case '"':
					sb.Append('\\');
					sb.Append(c);
					break;
				case '/':
					if (b == '<')
					{
						sb.Append('\\');
					}
					sb.Append(c);
					break;
				case '\b':
					sb.Append("\\b");
					break;
				case '\t':
					sb.Append("\\t");
					break;
				case '\n':
					sb.Append("\\n");
					break;
				case '\f':
					sb.Append("\\f");
					break;
				case '\r':
					sb.Append("\\r");
					break;
				default:
					if (c < ' ' || (c >= '\u0080' && c < '\u00a0') || (c >= '\u2000' && c < '\u2100'))
					{
						t = "000" + c.ToString("x");
						sb.Append("\\u" + t.Substring(t.Length - 4));
					}
					else
					{
						sb.Append(c);
					}
				break;
				}
			}
			sb.Append('"');
			return sb.ToString();
		}

		/// <summary>
		/// Remove a name and its value, if present. </summary>
		/// <param name="key"> The name to be removed. </param>
		/// <returns> The value that was associated with the name,
		/// or null if there was no value. </returns>
		public virtual object remove(string key)
		{
			return this.map.Remove(key);
		}

		/// <summary>
		/// Get an enumeration of the keys of the JSONObject.
		/// The keys will be sorted alphabetically.
		/// </summary>
		/// <returns> An iterator of the keys. </returns>
		public virtual IEnumerator sortedKeys()
		{
		  return (new SortedSet(this.map.Keys)).GetEnumerator();
		}

		/// <summary>
		/// Try to convert a string into a number, boolean, or null. If the string
		/// can't be converted, return the string. </summary>
		/// <param name="s"> A String. </param>
		/// <returns> A simple JSON value. </returns>
		public static object stringToValue(string s)
		{
			if (s.Equals(""))
			{
				return s;
			}
			if (s.Equals("true", StringComparison.CurrentCultureIgnoreCase))
			{
				return true;
			}
			if (s.Equals("false", StringComparison.CurrentCultureIgnoreCase))
			{
				return false;
			}
			if (s.Equals("null", StringComparison.CurrentCultureIgnoreCase))
			{
				return JSONObject.NULL;
			}

			/*
			 * If it might be a number, try converting it. 
			 * We support the non-standard 0x- convention. 
			 * If a number cannot be produced, then the value will just
			 * be a string. Note that the 0x-, plus, and implied string
			 * conventions are non-standard. A JSON parser may accept
			 * non-JSON forms as long as it accepts all correct JSON forms.
			 */

			char b = s[0];
			if ((b >= '0' && b <= '9') || b == '.' || b == '-' || b == '+')
			{
				if (b == '0' && s.Length > 2 && (s[1] == 'x' || s[1] == 'X'))
				{
					try
					{
						return new int?(Convert.ToInt32(s.Substring(2), 16));
					}
					catch (Exception)
					{
					}
				}
				try
				{
					if (s.IndexOf('.') > -1 || s.IndexOf('e') > -1 || s.IndexOf('E') > -1)
					{
						return Convert.ToDouble(s);
					}
					else
					{
						long? myLong = Convert.ToInt64(s);
						if ((long)myLong == (int)myLong)
						{
							return new int?((int)myLong);
						}
						else
						{
							return myLong;
						}
					}
				}
				catch (Exception)
				{
				}
			}
			return s;
		}


		/// <summary>
		/// Throw an exception if the object is an NaN or infinite number. </summary>
		/// <param name="o"> The object to test. </param>
		/// <exception cref="JSONException"> If o is a non-finite number. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static void testValidity(Object o) throws JSONException
		internal static void testValidity(object o)
		{
			if (o != null)
			{
				if (o is double?)
				{
					if (((double?)o).Infinite || ((double?)o).NaN)
					{
						throw new JSONException("JSON does not allow non-finite numbers.");
					}
				}
				else if (o is float?)
				{
					if (((float?)o).Infinite || ((float?)o).NaN)
					{
						throw new JSONException("JSON does not allow non-finite numbers.");
					}
				}
			}
		}


		/// <summary>
		/// Produce a JSONArray containing the values of the members of this
		/// JSONObject. </summary>
		/// <param name="names"> A JSONArray containing a list of key strings. This
		/// determines the sequence of the values in the result. </param>
		/// <returns> A JSONArray of values. </returns>
		/// <exception cref="JSONException"> If any of the values are non-finite numbers. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONArray toJSONArray(JSONArray names) throws JSONException
		public virtual JSONArray toJSONArray(JSONArray names)
		{
			if (names == null || names.length() == 0)
			{
				return null;
			}
			JSONArray ja = new JSONArray();
			for (int i = 0; i < names.length(); i += 1)
			{
				ja.put(this.opt(names.getString(i)));
			}
			return ja;
		}

		/// <summary>
		/// Make a JSON text of this JSONObject. For compactness, no whitespace
		/// is added. If this would not result in a syntactically correct JSON text,
		/// then null will be returned instead.
		/// <para>
		/// Warning: This method assumes that the signalData structure is acyclical.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a printable, displayable, portable, transmittable
		///  representation of the object, beginning
		///  with <code>{</code>&nbsp;<small>(left brace)</small> and ending
		///  with <code>}</code>&nbsp;<small>(right brace)</small>. </returns>
		public override string ToString()
		{
			try
			{
				IEnumerator keys = keys();
				StringBuilder sb = new StringBuilder("{");

				while (keys.hasNext())
				{
					if (sb.Length > 1)
					{
						sb.Append(',');
					}
					object o = keys.next();
					sb.Append(quote(o.ToString()));
					sb.Append(':');
					sb.Append(valueToString(this.map[o]));
				}
				sb.Append('}');
				return sb.ToString();
			}
			catch (Exception)
			{
				return null;
			}
		}


		/// <summary>
		/// Make a prettyprinted JSON text of this JSONObject.
		/// <para>
		/// Warning: This method assumes that the signalData structure is acyclical.
		/// </para>
		/// </summary>
		/// <param name="indentFactor"> The number of spaces to add to each level of
		///  indentation. </param>
		/// <returns> a printable, displayable, portable, transmittable
		///  representation of the object, beginning
		///  with <code>{</code>&nbsp;<small>(left brace)</small> and ending
		///  with <code>}</code>&nbsp;<small>(right brace)</small>. </returns>
		/// <exception cref="JSONException"> If the object contains an invalid number. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String toString(int indentFactor) throws JSONException
		public virtual string ToString(int indentFactor)
		{
			return ToString(indentFactor, 0);
		}


		/// <summary>
		/// Make a prettyprinted JSON text of this JSONObject.
		/// <para>
		/// Warning: This method assumes that the signalData structure is acyclical.
		/// </para>
		/// </summary>
		/// <param name="indentFactor"> The number of spaces to add to each level of
		///  indentation. </param>
		/// <param name="indent"> The indentation of the top level. </param>
		/// <returns> a printable, displayable, transmittable
		///  representation of the object, beginning
		///  with <code>{</code>&nbsp;<small>(left brace)</small> and ending
		///  with <code>}</code>&nbsp;<small>(right brace)</small>. </returns>
		/// <exception cref="JSONException"> If the object contains an invalid number. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String toString(int indentFactor, int indent) throws JSONException
		internal virtual string ToString(int indentFactor, int indent)
		{
			int j;
			int n = length();
			if (n == 0)
			{
				return "{}";
			}
			IEnumerator keys = sortedKeys();
			StringBuilder sb = new StringBuilder("{");
			int newindent = indent + indentFactor;
			object o;
			if (n == 1)
			{
				o = keys.next();
				sb.Append(quote(o.ToString()));
				sb.Append(": ");
				sb.Append(valueToString(this.map[o], indentFactor, indent));
			}
			else
			{
				while (keys.hasNext())
				{
					o = keys.next();
					if (sb.Length > 1)
					{
						sb.Append(",\n");
					}
					else
					{
						sb.Append('\n');
					}
					for (j = 0; j < newindent; j += 1)
					{
						sb.Append(' ');
					}
					sb.Append(quote(o.ToString()));
					sb.Append(": ");
					sb.Append(valueToString(this.map[o], indentFactor, newindent));
				}
				if (sb.Length > 1)
				{
					sb.Append('\n');
					for (j = 0; j < indent; j += 1)
					{
						sb.Append(' ');
					}
				}
			}
			sb.Append('}');
			return sb.ToString();
		}


		/// <summary>
		/// Make a JSON text of an Object value. If the object has an
		/// value.toJSONString() method, then that method will be used to produce
		/// the JSON text. The method is required to produce a strictly
		/// conforming text. If the object does not contain a toJSONString
		/// method (which is the most common case), then a text will be
		/// produced by other means. If the value is an array or Collection,
		/// then a JSONArray will be made from it and its toJSONString method
		/// will be called. If the value is a MAP, then a JSONObject will be made
		/// from it and its toJSONString method will be called. Otherwise, the
		/// value's toString method will be called, and the result will be quoted.
		/// 
		/// <para>
		/// Warning: This method assumes that the signalData structure is acyclical.
		/// </para>
		/// </summary>
		/// <param name="value"> The value to be serialized. </param>
		/// <returns> a printable, displayable, transmittable
		///  representation of the object, beginning
		///  with <code>{</code>&nbsp;<small>(left brace)</small> and ending
		///  with <code>}</code>&nbsp;<small>(right brace)</small>. </returns>
		/// <exception cref="JSONException"> If the value is or contains an invalid number. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static String valueToString(Object value) throws JSONException
		internal static string valueToString(object value)
		{
			if (value == null || value.Equals(null))
			{
				return "null";
			}
			if (value is JSONString)
			{
				object o;
				try
				{
					o = ((JSONString)value).toJSONString();
				}
				catch (Exception e)
				{
					throw new JSONException(e);
				}
				if (o is string)
				{
					return (string)o;
				}
				throw new JSONException("Bad value from toJSONString: " + o);
			}
			if (value is Number)
			{
				return numberToString((Number) value);
			}
			if (value is bool? || value is JSONObject || value is JSONArray)
			{
				return value.ToString();
			}
			if (value is IDictionary)
			{
				return (new JSONObject((IDictionary)value)).ToString();
			}
			if (value is ICollection)
			{
				return (new JSONArray((ICollection)value)).ToString();
			}
			if (value.GetType().IsArray)
			{
				return (new JSONArray(value)).ToString();
			}
			return quote(value.ToString());
		}


		/// <summary>
		/// Make a prettyprinted JSON text of an object value.
		/// <para>
		/// Warning: This method assumes that the signalData structure is acyclical.
		/// </para>
		/// </summary>
		/// <param name="value"> The value to be serialized. </param>
		/// <param name="indentFactor"> The number of spaces to add to each level of
		///  indentation. </param>
		/// <param name="indent"> The indentation of the top level. </param>
		/// <returns> a printable, displayable, transmittable
		///  representation of the object, beginning
		///  with <code>{</code>&nbsp;<small>(left brace)</small> and ending
		///  with <code>}</code>&nbsp;<small>(right brace)</small>. </returns>
		/// <exception cref="JSONException"> If the object contains an invalid number. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static String valueToString(Object value, int indentFactor, int indent) throws JSONException
		 internal static string valueToString(object value, int indentFactor, int indent)
		 {
			if (value == null || value.Equals(null))
			{
				return "null";
			}
			try
			{
				if (value is JSONString)
				{
					object o = ((JSONString)value).toJSONString();
					if (o is string)
					{
						return (string)o;
					}
				}
			}
			catch (Exception)
			{
			}
			if (value is Number)
			{
				return numberToString((Number) value);
			}
			if (value is bool?)
			{
				return value.ToString();
			}
			if (value is JSONObject)
			{
				return ((JSONObject)value).ToString(indentFactor, indent);
			}
			if (value is JSONArray)
			{
				return ((JSONArray)value).ToString(indentFactor, indent);
			}
			if (value is IDictionary)
			{
				return (new JSONObject((IDictionary)value)).ToString(indentFactor, indent);
			}
			if (value is ICollection)
			{
				return (new JSONArray((ICollection)value)).ToString(indentFactor, indent);
			}
			if (value.GetType().IsArray)
			{
				return (new JSONArray(value)).ToString(indentFactor, indent);
			}
			return quote(value.ToString());
		 }


		 /// <summary>
		 /// Wrap an object, if necessary. If the object is null, return the NULL 
		 /// object. If it is an array or collection, wrap it in a JSONArray. If 
		 /// it is a map, wrap it in a JSONObject. If it is a standard property 
		 /// (Double, String, et al) then it is already wrapped. Otherwise, if it 
		 /// comes from one of the java packages, turn it into a string. And if 
		 /// it doesn't, try to wrap it in a JSONObject. If the wrapping fails,
		 /// then null is returned.
		 /// </summary>
		 /// <param name="object"> The object to wrap </param>
		 /// <returns> The wrapped value </returns>
		 internal static object wrap(object @object)
		 {
			 try
			 {
				 if (@object == null)
				 {
					 return NULL;
				 }
				 if (@object is JSONObject || @object is JSONArray || @object is sbyte? || @object is char? || @object is short? || @object is int? || @object is long? || @object is bool? || @object is float? || @object is double? || @object is string || NULL.Equals(@object))
				 {
					 return @object;
				 }

				 if (@object is ICollection)
				 {
					 return new JSONArray((ICollection)@object);
				 }
				 if (@object.GetType().IsArray)
				 {
					 return new JSONArray(@object);
				 }
				 if (@object is IDictionary)
				 {
					 return new JSONObject((IDictionary)@object);
				 }
				 Package objectPackage = @object.GetType().Assembly;
				 string objectPackageName = (objectPackage != null ? objectPackage.Name : "");
				 if (objectPackageName.StartsWith("java.") || objectPackageName.StartsWith("javax.") || @object.GetType().ClassLoader == null)
				 {
					 return @object.ToString();
				 }
				 return new JSONObject(@object);
			 }
			 catch (Exception)
			 {
				 return null;
			 }
		 }


		 /// <summary>
		 /// Write the contents of the JSONObject as JSON text to a writer.
		 /// For compactness, no whitespace is added.
		 /// <para>
		 /// Warning: This method assumes that the signalData structure is acyclical.
		 /// 
		 /// </para>
		 /// </summary>
		 /// <returns> The writer. </returns>
		 /// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.Writer write(java.io.Writer writer) throws JSONException
		 public virtual Writer write(Writer writer)
		 {
			try
			{
				bool b = false;
				IEnumerator keys = keys();
				writer.write('{');

				while (keys.hasNext())
				{
					if (b)
					{
						writer.write(',');
					}
					object k = keys.next();
					writer.write(quote(k.ToString()));
					writer.write(':');
					object v = this.map[k];
					if (v is JSONObject)
					{
						((JSONObject)v).write(writer);
					}
					else if (v is JSONArray)
					{
						((JSONArray)v).write(writer);
					}
					else
					{
						writer.write(valueToString(v));
					}
					b = true;
				}
				writer.write('}');
				return writer;
			}
			catch (IOException exception)
			{
				throw new JSONException(exception);
			}
		 }
	}

}