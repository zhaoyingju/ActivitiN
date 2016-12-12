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
	/// @author Joram Barrez
	/// </summary>
	public class InputStreamSource : StreamSource
	{

	  // This class is used for bpmn parsing.
	  // The bpmn parsers needs to go over the stream at least twice:
	  // Once for the schema validation and once for the parsing itself.
	  // So we keep the content of the inputstream in memory so we can
	  // re-read it.

	  protected internal BufferedInputStream inputStream;
	  protected internal sbyte[] bytes;

	  public InputStreamSource(InputStream inputStream)
	  {
		this.inputStream = new BufferedInputStream(inputStream);
	  }

	  public virtual InputStream InputStream
	  {
		  get
		  {
			if (bytes == null)
			{
			  try
			  {
				bytes = getBytesFromInputStream(inputStream);
			  }
			  catch (IOException e)
			  {
				throw new ActivitiException("Could not read from inputstream", e);
			  }
			}
			return new BufferedInputStream(new ByteArrayInputStream(bytes));
		  }
	  }

	  public override string ToString()
	  {
		return "InputStream";
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public byte[] getBytesFromInputStream(java.io.InputStream inStream) throws java.io.IOException
	  public virtual sbyte[] getBytesFromInputStream(InputStream inStream)
	  {
		long length = inStream.available();
		sbyte[] bytes = new sbyte[(int) length];

		int offset = 0;
		int numRead = 0;
		while (offset < bytes.Length && (numRead = inStream.read(bytes, offset, bytes.Length - offset)) >= 0)
		{
		  offset += numRead;
		}

		if (offset < bytes.Length)
		{
		  throw new ActivitiException("Could not completely read inputstream ");
		}

		// Close the input stream and return bytes
		inStream.close();
		return bytes;
	  }

	}

}