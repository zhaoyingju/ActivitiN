using System;

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
namespace org.activiti.engine.impl.util
{



	/// <summary>
	/// @author Tom Baeyens
	/// @author Frederik Heremans
	/// @author Joram Barrez
	/// </summary>
	public class IoUtil
	{

	  public static sbyte[] readInputStream(InputStream inputStream, string inputStreamName)
	  {
		ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
		sbyte[] buffer = new sbyte[16 * 1024];
		try
		{
		  int bytesRead = inputStream.read(buffer);
		  while (bytesRead != -1)
		  {
			outputStream.write(buffer, 0, bytesRead);
			bytesRead = inputStream.read(buffer);
		  }
		}
		catch (Exception e)
		{
		  throw new ActivitiException("couldn't read input stream " + inputStreamName, e);
		}
		return outputStream.toByteArray();
	  }

	  public static string readFileAsString(string filePath)
	  {
		sbyte[] buffer = new sbyte[(int) getFile(filePath).length()];
		BufferedInputStream inputStream = null;
		try
		{
		  inputStream = new BufferedInputStream(new FileInputStream(getFile(filePath)));
		  inputStream.read(buffer);
		}
		catch (Exception e)
		{
		  throw new ActivitiException("Couldn't read file " + filePath + ": " + e.Message);
		}
		finally
		{
		  IoUtil.closeSilently(inputStream);
		}
		return StringHelperClass.NewString(buffer);
	  }

	  public static File getFile(string filePath)
	  {
		URL url = typeof(IoUtil).ClassLoader.getResource(filePath);
		try
		{
		  return new File(url.toURI());
		}
		catch (Exception e)
		{
		  throw new ActivitiException("Couldn't get file " + filePath + ": " + e.Message);
		}
	  }

	  public static void writeStringToFile(string content, string filePath)
	  {
		BufferedOutputStream outputStream = null;
		try
		{
		  outputStream = new BufferedOutputStream(new FileOutputStream(getFile(filePath)));
		  outputStream.write(content.GetBytes());
		  outputStream.flush();
		}
		catch (Exception e)
		{
		  throw new ActivitiException("Couldn't write file " + filePath, e);
		}
		finally
		{
		  IoUtil.closeSilently(outputStream);
		}
	  }

	  /// <summary>
	  /// Closes the given stream. The same as calling <seealso cref="InputStream#close()"/>, but
	  /// errors while closing are silently ignored.
	  /// </summary>
	  public static void closeSilently(InputStream inputStream)
	  {
		try
		{
		  if (inputStream != null)
		  {
			inputStream.close();
		  }
		}
		catch (IOException)
		{
		  // Exception is silently ignored
		}
	  }

	  /// <summary>
	  /// Closes the given stream. The same as calling <seealso cref="OutputStream#close()"/>, but
	  /// errors while closing are silently ignored.
	  /// </summary>
	  public static void closeSilently(OutputStream outputStream)
	  {
		try
		{
		  if (outputStream != null)
		  {
			outputStream.close();
		  }
		}
		catch (IOException)
		{
		  // Exception is silently ignored
		}
	  }
	}

}