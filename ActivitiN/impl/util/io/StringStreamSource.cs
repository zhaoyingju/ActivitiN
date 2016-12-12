/* Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.activiti.engine.impl.util.io
{


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class StringStreamSource : StreamSource
	{

	  internal string @string;
	  internal string byteArrayEncoding = "utf-8";

	  public StringStreamSource(string @string)
	  {
		this.@string = @string;
	  }

	  public StringStreamSource(string @string, string byteArrayEncoding)
	  {
		this.@string = @string;
		this.byteArrayEncoding = byteArrayEncoding;
	  }

	  public virtual InputStream InputStream
	  {
		  get
		  {
			try
			{
			  return new ByteArrayInputStream(byteArrayEncoding == null ? @string.GetBytes() : @string.GetBytes(byteArrayEncoding));
			}
			catch (UnsupportedEncodingException e)
			{
			  throw new ActivitiException("Unsupported enconding for string", e);
			}
		  }
	  }

	  public override string ToString()
	  {
		return "String";
	  }
	}

}