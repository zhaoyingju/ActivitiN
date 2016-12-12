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
	/// A JSONArray is an ordered sequence of values. Its external text form is a
	/// string wrapped in square brackets with commas separating the values. The
	/// internal form is an object having <code>get</code> and <code>opt</code>
	/// methods for accessing the values by index, and <code>put</code> methods for
	/// adding or replacing values. The values can be any of these types:
	/// <code>Boolean</code>, <code>JSONArray</code>, <code>JSONObject</code>,
	/// <code>Number</code>, <code>String</code>, or the
	/// <code>JSONObject.NULL object</code>.
	/// <para>
	/// The constructor can convert a JSON text into a Java object. The
	/// <code>toString</code> method converts to JSON text.
	/// </para>
	/// <para>
	/// A <code>get</code> method returns a value if one can be found, and throws an
	/// exception if one cannot be found. An <code>opt</code> method returns a
	/// default value instead of throwing an exception, and so is useful for
	/// obtaining optional values.
	/// </para>
	/// <para>
	/// The generic <code>get()</code> and <code>opt()</code> methods return an
	/// object which you can cast or query for type. There are also typed
	/// <code>get</code> and <code>opt</code> methods that do type checking and type
	/// coercion for you.
	/// </para>
	/// <para>
	/// The texts produced by the <code>toString</code> methods strictly conform to
	/// JSON syntax rules. The constructors are more forgiving in the texts they will
	/// accept:
	/// <ul>
	/// <li>An extra <code>,</code>&nbsp;<small>(comma)</small> may appear just
	///     before the closing bracket.</li>
	/// <li>The <code>null</code> value will be inserted when there
	///     is <code>,</code>&nbsp;<small>(comma)</small> elision.</li>
	/// <li>Strings may be quoted with <code>'</code>&nbsp;<small>(single
	///     quote)</small>.</li>
	/// <li>Strings do not need to be quoted at all if they do not begin with a quote
	///     or single quote, and if they do not contain leading or trailing spaces,
	///     and if they do not contain any of these characters:
	///     <code>{ } [ ] / \ : , = ; #</code> and if they do not look like numbers
	///     and if they are not the reserved words <code>true</code>,
	///     <code>false</code>, or <code>null</code>.</li>
	/// <li>Values can be separated by <code>;</code> <small>(semicolon)</small> as
	///     well as by <code>,</code> <small>(comma)</small>.</li>
	/// <li>Numbers may have the 
	///     <code>0x-</code> <small>(hex)</small> prefix.</li>
	/// </ul>
	/// 
	/// @author JSON.org
	/// @version 2009-04-14
	/// </para>
	/// </summary>
	public class JSONArray
	{


		/// <summary>
		/// The arrayList where the JSONArray's properties are kept.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private java.util.ArrayList myArrayList;
		private ArrayList myArrayList;


		/// <summary>
		/// Construct an empty JSONArray.
		/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public JSONArray()
		public JSONArray()
		{
			this.myArrayList = new ArrayList();
		}

		/// <summary>
		/// Construct a JSONArray from a JSONTokener. </summary>
		/// <param name="x"> A JSONTokener </param>
		/// <exception cref="JSONException"> If there is a syntax error. </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public JSONArray(JSONTokener x) throws JSONException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public JSONArray(JSONTokener x) : this()
		{
			char c = x.nextClean();
			char q;
			if (c == '[')
			{
				q = ']';
			}
			else if (c == '(')
			{
				q = ')';
			}
			else
			{
				throw x.syntaxError("A JSONArray text must start with '['");
			}
			if (x.nextClean() == ']')
			{
				return;
			}
			x.back();
			for (;;)
			{
				if (x.nextClean() == ',')
				{
					x.back();
					this.myArrayList.Add(null);
				}
				else
				{
					x.back();
					this.myArrayList.Add(x.nextValue());
				}
				c = x.nextClean();
				switch (c)
				{
				case ';':
				case ',':
					if (x.nextClean() == ']')
					{
						return;
					}
					x.back();
					break;
				case ']':
				case ')':
					if (q != c)
					{
						throw x.syntaxError("Expected a '" + new char?(q) + "'");
					}
					return;
				default:
					throw x.syntaxError("Expected a ',' or ']'");
				}
			}
		}


		/// <summary>
		/// Construct a JSONArray from a source JSON text. </summary>
		/// <param name="source">     A string that begins with
		/// <code>[</code>&nbsp;<small>(left bracket)</small>
		///  and ends with <code>]</code>&nbsp;<small>(right bracket)</small>. </param>
		///  <exception cref="JSONException"> If there is a syntax error. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONArray(String source) throws JSONException
		public JSONArray(string source) : this(new JSONTokener(source))
		{
		}


		/// <summary>
		/// Construct a JSONArray from a Collection. </summary>
		/// <param name="collection">     A Collection. </param>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public JSONArray(java.util.Collection collection)
		public JSONArray(ICollection collection)
		{
			this.myArrayList = new ArrayList();
			if (collection != null)
			{
				IEnumerator iter = collection.GetEnumerator();
		  while (iter.hasNext())
		  {
					object o = iter.next();
					this.myArrayList.Add(JSONObject.wrap(o));
		  }
			}
		}


		/// <summary>
		/// Construct a JSONArray from an array </summary>
		/// <exception cref="JSONException"> If not an array. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONArray(Object array) throws JSONException
		public JSONArray(object array) : this()
		{
			if (array.GetType().IsArray)
			{
				int length = Array.getLength(array);
				for (int i = 0; i < length; i += 1)
				{
					this.put(JSONObject.wrap(Array.get(array, i)));
				}
			}
			else
			{
				throw new JSONException("JSONArray initial value should be a string or collection or array.");
			}
		}


		/// <summary>
		/// Get the object value associated with an index. </summary>
		/// <param name="index">
		///  The index must be between 0 and length() - 1. </param>
		/// <returns> An object value. </returns>
		/// <exception cref="JSONException"> If there is no value for the index. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object get(int index) throws JSONException
		public virtual object get(int index)
		{
			object o = opt(index);
			if (o == null)
			{
				throw new JSONException("JSONArray[" + index + "] not found.");
			}
			return o;
		}


		/// <summary>
		/// Get the boolean value associated with an index.
		/// The string values "true" and "false" are converted to boolean.
		/// </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <returns>      The truth. </returns>
		/// <exception cref="JSONException"> If there is no value for the index or if the
		///  value is not convertable to boolean. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean getBoolean(int index) throws JSONException
		public virtual bool getBoolean(int index)
		{
			object o = get(index);
			if (o.Equals(false) || (o is string && ((string)o).Equals("false", StringComparison.CurrentCultureIgnoreCase)))
			{
				return false;
			}
			else if (o.Equals(true) || (o is string && ((string)o).Equals("true", StringComparison.CurrentCultureIgnoreCase)))
			{
				return true;
			}
			throw new JSONException("JSONArray[" + index + "] is not a Boolean.");
		}


		/// <summary>
		/// Get the double value associated with an index.
		/// </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <returns>      The value. </returns>
		/// <exception cref="JSONException"> If the key is not found or if the value cannot
		///  be converted to a number. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public double getDouble(int index) throws JSONException
		public virtual double getDouble(int index)
		{
			object o = get(index);
			try
			{
				return o is Number ? (double)((Number)o) : (double)Convert.ToDouble((string)o);
			}
			catch (Exception)
			{
				throw new JSONException("JSONArray[" + index + "] is not a number.");
			}
		}


		/// <summary>
		/// Get the int value associated with an index.
		/// </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <returns>      The value. </returns>
		/// <exception cref="JSONException"> If the key is not found or if the value cannot
		///  be converted to a number.
		///  if the value cannot be converted to a number. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getInt(int index) throws JSONException
		public virtual int getInt(int index)
		{
			object o = get(index);
			return o is Number ? (int)((Number)o) : (int)getDouble(index);
		}


		/// <summary>
		/// Get the JSONArray associated with an index. </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <returns>      A JSONArray value. </returns>
		/// <exception cref="JSONException"> If there is no value for the index. or if the
		/// value is not a JSONArray </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONArray getJSONArray(int index) throws JSONException
		public virtual JSONArray getJSONArray(int index)
		{
			object o = get(index);
			if (o is JSONArray)
			{
				return (JSONArray)o;
			}
			throw new JSONException("JSONArray[" + index + "] is not a JSONArray.");
		}


		/// <summary>
		/// Get the JSONObject associated with an index. </summary>
		/// <param name="index"> subscript </param>
		/// <returns>      A JSONObject value. </returns>
		/// <exception cref="JSONException"> If there is no value for the index or if the
		/// value is not a JSONObject </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject getJSONObject(int index) throws JSONException
		public virtual JSONObject getJSONObject(int index)
		{
			object o = get(index);
			if (o is JSONObject)
			{
				return (JSONObject)o;
			}
			throw new JSONException("JSONArray[" + index + "] is not a JSONObject.");
		}


		/// <summary>
		/// Get the long value associated with an index.
		/// </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <returns>      The value. </returns>
		/// <exception cref="JSONException"> If the key is not found or if the value cannot
		///  be converted to a number. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public long getLong(int index) throws JSONException
		public virtual long getLong(int index)
		{
			object o = get(index);
			return o is Number ? (long)((Number)o) : (long)getDouble(index);
		}


		/// <summary>
		/// Get the string associated with an index. </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <returns>      A string value. </returns>
		/// <exception cref="JSONException"> If there is no value for the index. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String getString(int index) throws JSONException
		public virtual string getString(int index)
		{
			return get(index).ToString();
		}


		/// <summary>
		/// Determine if the value is null. </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <returns> true if the value at the index is null, or if there is no value. </returns>
		public virtual bool isNull(int index)
		{
			return JSONObject.NULL.Equals(opt(index));
		}


		/// <summary>
		/// Make a string from the contents of this JSONArray. The
		/// <code>separator</code> string is inserted between each element.
		/// Warning: This method assumes that the signalData structure is acyclical. </summary>
		/// <param name="separator"> A string that will be inserted between the elements. </param>
		/// <returns> a string. </returns>
		/// <exception cref="JSONException"> If the array contains an invalid number. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String join(String separator) throws JSONException
		public virtual string join(string separator)
		{
			int len = length();
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < len; i += 1)
			{
				if (i > 0)
				{
					sb.Append(separator);
				}
				sb.Append(JSONObject.valueToString(this.myArrayList[i]));
			}
			return sb.ToString();
		}


		/// <summary>
		/// Get the number of elements in the JSONArray, included nulls.
		/// </summary>
		/// <returns> The length (or size). </returns>
		public virtual int length()
		{
			return this.myArrayList.Count;
		}


		/// <summary>
		/// Get the optional object value associated with an index. </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <returns>      An object value, or null if there is no
		///              object at that index. </returns>
		public virtual object opt(int index)
		{
			return (index < 0 || index >= length()) ? null : this.myArrayList[index];
		}


		/// <summary>
		/// Get the optional boolean value associated with an index.
		/// It returns false if there is no value at that index,
		/// or if the value is not Boolean.TRUE or the String "true".
		/// </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <returns>      The truth. </returns>
		public virtual bool optBoolean(int index)
		{
			return optBoolean(index, false);
		}


		/// <summary>
		/// Get the optional boolean value associated with an index.
		/// It returns the defaultValue if there is no value at that index or if
		/// it is not a Boolean or the String "true" or "false" (case insensitive).
		/// </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <param name="defaultValue">     A boolean default. </param>
		/// <returns>      The truth. </returns>
		public virtual bool optBoolean(int index, bool defaultValue)
		{
			try
			{
				return getBoolean(index);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}


		/// <summary>
		/// Get the optional double value associated with an index.
		/// NaN is returned if there is no value for the index,
		/// or if the value is not a number and cannot be converted to a number.
		/// </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <returns>      The value. </returns>
		public virtual double optDouble(int index)
		{
			return optDouble(index, double.NaN);
		}


		/// <summary>
		/// Get the optional double value associated with an index.
		/// The defaultValue is returned if there is no value for the index,
		/// or if the value is not a number and cannot be converted to a number.
		/// </summary>
		/// <param name="index"> subscript </param>
		/// <param name="defaultValue">     The default value. </param>
		/// <returns>      The value. </returns>
		public virtual double optDouble(int index, double defaultValue)
		{
			try
			{
				return getDouble(index);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}


		/// <summary>
		/// Get the optional int value associated with an index.
		/// Zero is returned if there is no value for the index,
		/// or if the value is not a number and cannot be converted to a number.
		/// </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <returns>      The value. </returns>
		public virtual int optInt(int index)
		{
			return optInt(index, 0);
		}


		/// <summary>
		/// Get the optional int value associated with an index.
		/// The defaultValue is returned if there is no value for the index,
		/// or if the value is not a number and cannot be converted to a number. </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <param name="defaultValue">     The default value. </param>
		/// <returns>      The value. </returns>
		public virtual int optInt(int index, int defaultValue)
		{
			try
			{
				return getInt(index);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}


		/// <summary>
		/// Get the optional JSONArray associated with an index. </summary>
		/// <param name="index"> subscript </param>
		/// <returns>      A JSONArray value, or null if the index has no value,
		/// or if the value is not a JSONArray. </returns>
		public virtual JSONArray optJSONArray(int index)
		{
			object o = opt(index);
			return o is JSONArray ? (JSONArray)o : null;
		}


		/// <summary>
		/// Get the optional JSONObject associated with an index.
		/// Null is returned if the key is not found, or null if the index has
		/// no value, or if the value is not a JSONObject.
		/// </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <returns>      A JSONObject value. </returns>
		public virtual JSONObject optJSONObject(int index)
		{
			object o = opt(index);
			return o is JSONObject ? (JSONObject)o : null;
		}


		/// <summary>
		/// Get the optional long value associated with an index.
		/// Zero is returned if there is no value for the index,
		/// or if the value is not a number and cannot be converted to a number.
		/// </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <returns>      The value. </returns>
		public virtual long optLong(int index)
		{
			return optLong(index, 0);
		}


		/// <summary>
		/// Get the optional long value associated with an index.
		/// The defaultValue is returned if there is no value for the index,
		/// or if the value is not a number and cannot be converted to a number. </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <param name="defaultValue">     The default value. </param>
		/// <returns>      The value. </returns>
		public virtual long optLong(int index, long defaultValue)
		{
			try
			{
				return getLong(index);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}


		/// <summary>
		/// Get the optional string value associated with an index. It returns an
		/// empty string if there is no value at that index. If the value
		/// is not a string and is not null, then it is coverted to a string.
		/// </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <returns>      A String value. </returns>
		public virtual string optString(int index)
		{
			return optString(index, "");
		}


		/// <summary>
		/// Get the optional string associated with an index.
		/// The defaultValue is returned if the key is not found.
		/// </summary>
		/// <param name="index"> The index must be between 0 and length() - 1. </param>
		/// <param name="defaultValue">     The default value. </param>
		/// <returns>      A String value. </returns>
		public virtual string optString(int index, string defaultValue)
		{
			object o = opt(index);
			return o != null ? o.ToString() : defaultValue;
		}


		/// <summary>
		/// Append a boolean value. This increases the array's length by one.
		/// </summary>
		/// <param name="value"> A boolean value. </param>
		/// <returns> this. </returns>
		public virtual JSONArray put(bool value)
		{
			put(value ? true : false);
			return this;
		}


		/// <summary>
		/// Put a value in the JSONArray, where the value will be a
		/// JSONArray which is produced from a Collection. </summary>
		/// <param name="value"> A Collection value. </param>
		/// <returns>      this. </returns>
		public virtual JSONArray put(ICollection value)
		{
			put(new JSONArray(value));
			return this;
		}


		/// <summary>
		/// Append a double value. This increases the array's length by one.
		/// </summary>
		/// <param name="value"> A double value. </param>
		/// <exception cref="JSONException"> if the value is not finite. </exception>
		/// <returns> this. </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONArray put(double value) throws JSONException
		public virtual JSONArray put(double value)
		{
			double? d = new double?(value);
			JSONObject.testValidity(d);
			put(d);
			return this;
		}


		/// <summary>
		/// Append an int value. This increases the array's length by one.
		/// </summary>
		/// <param name="value"> An int value. </param>
		/// <returns> this. </returns>
		public virtual JSONArray put(int value)
		{
			put(new int?(value));
			return this;
		}


		/// <summary>
		/// Append an long value. This increases the array's length by one.
		/// </summary>
		/// <param name="value"> A long value. </param>
		/// <returns> this. </returns>
		public virtual JSONArray put(long value)
		{
			put(new long?(value));
			return this;
		}


		/// <summary>
		/// Put a value in the JSONArray, where the value will be a
		/// JSONObject which is produced from a Map. </summary>
		/// <param name="value"> A Map value. </param>
		/// <returns>      this. </returns>
		public virtual JSONArray put(IDictionary value)
		{
			put(new JSONObject(value));
			return this;
		}


		/// <summary>
		/// Append an object value. This increases the array's length by one. </summary>
		/// <param name="value"> An object value.  The value should be a
		///  Boolean, Double, Integer, JSONArray, JSONObject, Long, or String, or the
		///  JSONObject.NULL object. </param>
		/// <returns> this. </returns>
		public virtual JSONArray put(object value)
		{
			this.myArrayList.Add(value);
			return this;
		}


		/// <summary>
		/// Put or replace a boolean value in the JSONArray. If the index is greater
		/// than the length of the JSONArray, then null elements will be added as
		/// necessary to pad it out. </summary>
		/// <param name="index"> The subscript. </param>
		/// <param name="value"> A boolean value. </param>
		/// <returns> this. </returns>
		/// <exception cref="JSONException"> If the index is negative. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONArray put(int index, boolean value) throws JSONException
		public virtual JSONArray put(int index, bool value)
		{
			put(index, value ? true : false);
			return this;
		}


		/// <summary>
		/// Put a value in the JSONArray, where the value will be a
		/// JSONArray which is produced from a Collection. </summary>
		/// <param name="index"> The subscript. </param>
		/// <param name="value"> A Collection value. </param>
		/// <returns>      this. </returns>
		/// <exception cref="JSONException"> If the index is negative or if the value is
		/// not finite. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONArray put(int index, java.util.Collection value) throws JSONException
		public virtual JSONArray put(int index, ICollection value)
		{
			put(index, new JSONArray(value));
			return this;
		}


		/// <summary>
		/// Put or replace a double value. If the index is greater than the length of
		///  the JSONArray, then null elements will be added as necessary to pad
		///  it out. </summary>
		/// <param name="index"> The subscript. </param>
		/// <param name="value"> A double value. </param>
		/// <returns> this. </returns>
		/// <exception cref="JSONException"> If the index is negative or if the value is
		/// not finite. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONArray put(int index, double value) throws JSONException
		public virtual JSONArray put(int index, double value)
		{
			put(index, new double?(value));
			return this;
		}


		/// <summary>
		/// Put or replace an int value. If the index is greater than the length of
		///  the JSONArray, then null elements will be added as necessary to pad
		///  it out. </summary>
		/// <param name="index"> The subscript. </param>
		/// <param name="value"> An int value. </param>
		/// <returns> this. </returns>
		/// <exception cref="JSONException"> If the index is negative. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONArray put(int index, int value) throws JSONException
		public virtual JSONArray put(int index, int value)
		{
			put(index, new int?(value));
			return this;
		}


		/// <summary>
		/// Put or replace a long value. If the index is greater than the length of
		///  the JSONArray, then null elements will be added as necessary to pad
		///  it out. </summary>
		/// <param name="index"> The subscript. </param>
		/// <param name="value"> A long value. </param>
		/// <returns> this. </returns>
		/// <exception cref="JSONException"> If the index is negative. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONArray put(int index, long value) throws JSONException
		public virtual JSONArray put(int index, long value)
		{
			put(index, new long?(value));
			return this;
		}


		/// <summary>
		/// Put a value in the JSONArray, where the value will be a
		/// JSONObject which is produced from a Map. </summary>
		/// <param name="index"> The subscript. </param>
		/// <param name="value"> The Map value. </param>
		/// <returns>      this. </returns>
		/// <exception cref="JSONException"> If the index is negative or if the the value is
		///  an invalid number. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONArray put(int index, java.util.Map value) throws JSONException
		public virtual JSONArray put(int index, IDictionary value)
		{
			put(index, new JSONObject(value));
			return this;
		}


		/// <summary>
		/// Put or replace an object value in the JSONArray. If the index is greater
		///  than the length of the JSONArray, then null elements will be added as
		///  necessary to pad it out. </summary>
		/// <param name="index"> The subscript. </param>
		/// <param name="value"> The value to put into the array. The value should be a
		///  Boolean, Double, Integer, JSONArray, JSONObject, Long, or String, or the
		///  JSONObject.NULL object. </param>
		/// <returns> this. </returns>
		/// <exception cref="JSONException"> If the index is negative or if the the value is
		///  an invalid number. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONArray put(int index, Object value) throws JSONException
		public virtual JSONArray put(int index, object value)
		{
			JSONObject.testValidity(value);
			if (index < 0)
			{
				throw new JSONException("JSONArray[" + index + "] not found.");
			}
			if (index < length())
			{
				this.myArrayList[index] = value;
			}
			else
			{
				while (index != length())
				{
					put(JSONObject.NULL);
				}
				put(value);
			}
			return this;
		}


		/// <summary>
		/// Remove an index and close the hole. </summary>
		/// <param name="index"> The index of the element to be removed. </param>
		/// <returns> The value that was associated with the index,
		/// or null if there was no value. </returns>
		public virtual object remove(int index)
		{
			object o = opt(index);
			this.myArrayList.RemoveAt(index);
			return o;
		}


		/// <summary>
		/// Produce a JSONObject by combining a JSONArray of names with the values
		/// of this JSONArray. </summary>
		/// <param name="names"> A JSONArray containing a list of key strings. These will be
		/// paired with the values. </param>
		/// <returns> A JSONObject, or null if there are no names or if this JSONArray
		/// has no values. </returns>
		/// <exception cref="JSONException"> If any of the names are null. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONObject toJSONObject(JSONArray names) throws JSONException
		public virtual JSONObject toJSONObject(JSONArray names)
		{
			if (names == null || names.length() == 0 || length() == 0)
			{
				return null;
			}
			JSONObject jo = new JSONObject();
			for (int i = 0; i < names.length(); i += 1)
			{
				jo.put(names.getString(i), this.opt(i));
			}
			return jo;
		}


		/// <summary>
		/// Make a JSON text of this JSONArray. For compactness, no
		/// unnecessary whitespace is added. If it is not possible to produce a
		/// syntactically correct JSON text then null will be returned instead. This
		/// could occur if the array contains an invalid number.
		/// <para>
		/// Warning: This method assumes that the signalData structure is acyclical.
		/// 
		/// </para>
		/// </summary>
		/// <returns> a printable, displayable, transmittable
		///  representation of the array. </returns>
		public override string ToString()
		{
			try
			{
				return '[' + join(",") + ']';
			}
			catch (Exception)
			{
				return null;
			}
		}


		/// <summary>
		/// Make a prettyprinted JSON text of this JSONArray.
		/// Warning: This method assumes that the signalData structure is acyclical. </summary>
		/// <param name="indentFactor"> The number of spaces to add to each level of
		///  indentation. </param>
		/// <returns> a printable, displayable, transmittable
		///  representation of the object, beginning
		///  with <code>[</code>&nbsp;<small>(left bracket)</small> and ending
		///  with <code>]</code>&nbsp;<small>(right bracket)</small>. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public String toString(int indentFactor) throws JSONException
		public virtual string ToString(int indentFactor)
		{
			return ToString(indentFactor, 0);
		}


		/// <summary>
		/// Make a prettyprinted JSON text of this JSONArray.
		/// Warning: This method assumes that the signalData structure is acyclical. </summary>
		/// <param name="indentFactor"> The number of spaces to add to each level of
		///  indentation. </param>
		/// <param name="indent"> The indention of the top level. </param>
		/// <returns> a printable, displayable, transmittable
		///  representation of the array. </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: String toString(int indentFactor, int indent) throws JSONException
		internal virtual string ToString(int indentFactor, int indent)
		{
			int len = length();
			if (len == 0)
			{
				return "[]";
			}
			int i;
			StringBuilder sb = new StringBuilder("[");
			if (len == 1)
			{
				sb.Append(JSONObject.valueToString(this.myArrayList[0], indentFactor, indent));
			}
			else
			{
				int newindent = indent + indentFactor;
				sb.Append('\n');
				for (i = 0; i < len; i += 1)
				{
					if (i > 0)
					{
						sb.Append(",\n");
					}
					for (int j = 0; j < newindent; j += 1)
					{
						sb.Append(' ');
					}
					sb.Append(JSONObject.valueToString(this.myArrayList[i], indentFactor, newindent));
				}
				sb.Append('\n');
				for (i = 0; i < indent; i += 1)
				{
					sb.Append(' ');
				}
			}
			sb.Append(']');
			return sb.ToString();
		}


		/// <summary>
		/// Write the contents of the JSONArray as JSON text to a writer.
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
				int len = length();

				writer.write('[');

				for (int i = 0; i < len; i += 1)
				{
					if (b)
					{
						writer.write(',');
					}
					object v = this.myArrayList[i];
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
						writer.write(JSONObject.valueToString(v));
					}
					b = true;
				}
				writer.write(']');
				return writer;
			}
			catch (IOException e)
			{
			   throw new JSONException(e);
			}
		}
	}
}