using System;

namespace org.activiti.engine.impl.util.json
{


	/*
	Copyright (c) 2006 JSON.org
	
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
	/// JSONWriter provides a quick and convenient way of producing JSON text.
	/// The texts produced strictly conform to JSON syntax rules. No whitespace is
	/// added, so the results are ready for transmission or storage. Each instance of
	/// JSONWriter can produce one JSON text.
	/// <para>
	/// A JSONWriter instance provides a <code>value</code> method for appending
	/// values to the
	/// text, and a <code>key</code>
	/// method for adding keys before values in objects. There are <code>array</code>
	/// and <code>endArray</code> methods that make and bound array values, and
	/// <code>object</code> and <code>endObject</code> methods which make and bound
	/// object values. All of these methods return the JSONWriter instance,
	/// permitting a cascade style. For example, <pre>
	/// new JSONWriter(myWriter)
	///     .object()
	///         .key("JSON")
	///         .value("Hello, World!")
	///     .endObject();</pre> which writes <pre>
	/// {"JSON":"Hello, World!"}</pre>
	/// </para>
	/// <para>
	/// The first method called must be <code>array</code> or <code>object</code>.
	/// There are no methods for adding commas or colons. JSONWriter adds them for
	/// you. Objects and arrays can be nested up to 20 levels deep.
	/// </para>
	/// <para>
	/// This can sometimes be easier than using a JSONObject to build a string.
	/// @author JSON.org
	/// @version 2010-03-11
	/// </para>
	/// </summary>
	public class JSONWriter
	{
		private const int maxdepth = 20;

		/// <summary>
		/// The comma flag determines if a comma should be output before the next
		/// value.
		/// </summary>
		private bool comma;

		/// <summary>
		/// The current mode. Values:
		/// 'a' (array),
		/// 'd' (done),
		/// 'i' (initial),
		/// 'k' (key),
		/// 'o' (object).
		/// </summary>
		protected internal char mode;

		/// <summary>
		/// The object/array stack.
		/// </summary>
		private JSONObject[] stack;

		/// <summary>
		/// The stack top index. A value of 0 indicates that the stack is empty.
		/// </summary>
		private int top;

		/// <summary>
		/// The writer that will receive the output.
		/// </summary>
		protected internal Writer writer;

		/// <summary>
		/// Make a fresh JSONWriter. It can be used to build one JSON text.
		/// </summary>
		public JSONWriter(Writer w)
		{
			this.comma = false;
			this.mode = 'i';
			this.stack = new JSONObject[maxdepth];
			this.top = 0;
			this.writer = w;
		}

		/// <summary>
		/// Append a value. </summary>
		/// <param name="s"> A string value. </param>
		/// <returns> this </returns>
		/// <exception cref="JSONException"> If the value is out of sequence. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private JSONWriter append(String s) throws JSONException
		private JSONWriter append(string s)
		{
			if (s == null)
			{
				throw new JSONException("Null pointer");
			}
			if (this.mode == 'o' || this.mode == 'a')
			{
				try
				{
					if (this.comma && this.mode == 'a')
					{
						this.writer.write(',');
					}
					this.writer.write(s);
				}
				catch (IOException e)
				{
					throw new JSONException(e);
				}
				if (this.mode == 'o')
				{
					this.mode = 'k';
				}
				this.comma = true;
				return this;
			}
			throw new JSONException("Value out of sequence.");
		}

		/// <summary>
		/// Begin appending a new array. All values until the balancing
		/// <code>endArray</code> will be appended to this array. The
		/// <code>endArray</code> method must be called to mark the array's end. </summary>
		/// <returns> this </returns>
		/// <exception cref="JSONException"> If the nesting is too deep, or if the object is
		/// started in the wrong place (for example as a key or after the end of the
		/// outermost array or object). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONWriter array() throws JSONException
		public virtual JSONWriter array()
		{
			if (this.mode == 'i' || this.mode == 'o' || this.mode == 'a')
			{
				this.push(null);
				this.append("[");
				this.comma = false;
				return this;
			}
			throw new JSONException("Misplaced array.");
		}

		/// <summary>
		/// End something. </summary>
		/// <param name="m"> Mode </param>
		/// <param name="c"> Closing character </param>
		/// <returns> this </returns>
		/// <exception cref="JSONException"> If unbalanced. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private JSONWriter end(char m, char c) throws JSONException
		private JSONWriter end(char m, char c)
		{
			if (this.mode != m)
			{
				throw new JSONException(m == 'a' ? "Misplaced endArray." : "Misplaced endObject.");
			}
			this.pop(m);
			try
			{
				this.writer.write(c);
			}
			catch (IOException e)
			{
				throw new JSONException(e);
			}
			this.comma = true;
			return this;
		}

		/// <summary>
		/// End an array. This method most be called to balance calls to
		/// <code>array</code>. </summary>
		/// <returns> this </returns>
		/// <exception cref="JSONException"> If incorrectly nested. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONWriter endArray() throws JSONException
		public virtual JSONWriter endArray()
		{
			return this.end('a', ']');
		}

		/// <summary>
		/// End an object. This method most be called to balance calls to
		/// <code>object</code>. </summary>
		/// <returns> this </returns>
		/// <exception cref="JSONException"> If incorrectly nested. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONWriter endObject() throws JSONException
		public virtual JSONWriter endObject()
		{
			return this.end('k', '}');
		}

		/// <summary>
		/// Append a key. The key will be associated with the next value. In an
		/// object, every value must be preceded by a key. </summary>
		/// <param name="s"> A key string. </param>
		/// <returns> this </returns>
		/// <exception cref="JSONException"> If the key is out of place. For example, keys
		///  do not belong in arrays or if the key is null. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONWriter key(String s) throws JSONException
		public virtual JSONWriter key(string s)
		{
			if (s == null)
			{
				throw new JSONException("Null key.");
			}
			if (this.mode == 'k')
			{
				try
				{
					stack[top - 1].putOnce(s, true);
					if (this.comma)
					{
						this.writer.write(',');
					}
					this.writer.write(JSONObject.quote(s));
					this.writer.write(':');
					this.comma = false;
					this.mode = 'o';
					return this;
				}
				catch (IOException e)
				{
					throw new JSONException(e);
				}
			}
			throw new JSONException("Misplaced key.");
		}


		/// <summary>
		/// Begin appending a new object. All keys and values until the balancing
		/// <code>endObject</code> will be appended to this object. The
		/// <code>endObject</code> method must be called to mark the object's end. </summary>
		/// <returns> this </returns>
		/// <exception cref="JSONException"> If the nesting is too deep, or if the object is
		/// started in the wrong place (for example as a key or after the end of the
		/// outermost array or object). </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONWriter object() throws JSONException
		public virtual JSONWriter @object()
		{
			if (this.mode == 'i')
			{
				this.mode = 'o';
			}
			if (this.mode == 'o' || this.mode == 'a')
			{
				this.append("{");
				this.push(new JSONObject());
				this.comma = false;
				return this;
			}
			throw new JSONException("Misplaced object.");

		}


		/// <summary>
		/// Pop an array or object scope. </summary>
		/// <param name="c"> The scope to close. </param>
		/// <exception cref="JSONException"> If nesting is wrong. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void pop(char c) throws JSONException
		private void pop(char c)
		{
			if (this.top <= 0)
			{
				throw new JSONException("Nesting error.");
			}
			char m = this.stack[this.top - 1] == null ? 'a' : 'k';
			if (m != c)
			{
				throw new JSONException("Nesting error.");
			}
			this.top -= 1;
			this.mode = this.top == 0 ? 'd' : this.stack[this.top - 1] == null ? 'a' : 'k';
		}

		/// <summary>
		/// Push an array or object scope. </summary>
		/// <exception cref="JSONException"> If nesting is too deep. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void push(JSONObject jo) throws JSONException
		private void push(JSONObject jo)
		{
			if (this.top >= maxdepth)
			{
				throw new JSONException("Nesting too deep.");
			}
			this.stack[this.top] = jo;
			this.mode = jo == null ? 'a' : 'k';
			this.top += 1;
		}


		/// <summary>
		/// Append either the value <code>true</code> or the value
		/// <code>false</code>. </summary>
		/// <param name="b"> A boolean. </param>
		/// <returns> this </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONWriter value(boolean b) throws JSONException
		public virtual JSONWriter value(bool b)
		{
			return this.append(b ? "true" : "false");
		}

		/// <summary>
		/// Append a double value. </summary>
		/// <param name="d"> A double. </param>
		/// <returns> this </returns>
		/// <exception cref="JSONException"> If the number is not finite. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONWriter value(double d) throws JSONException
		public virtual JSONWriter value(double d)
		{
			return this.value(new double?(d));
		}

		/// <summary>
		/// Append a long value. </summary>
		/// <param name="l"> A long. </param>
		/// <returns> this </returns>
		/// <exception cref="JSONException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONWriter value(long l) throws JSONException
		public virtual JSONWriter value(long l)
		{
			return this.append(Convert.ToString(l));
		}


		/// <summary>
		/// Append an object value. </summary>
		/// <param name="o"> The object to append. It can be null, or a Boolean, Number,
		///   String, JSONObject, or JSONArray, or an object with a toJSONString()
		///   method. </param>
		/// <returns> this </returns>
		/// <exception cref="JSONException"> If the value is out of sequence. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JSONWriter value(Object o) throws JSONException
		public virtual JSONWriter value(object o)
		{
			return this.append(JSONObject.valueToString(o));
		}
	}

}