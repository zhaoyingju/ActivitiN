using System.Collections.Generic;

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
	/// helper/convience methods for working with collections.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class CollectionUtil
	{

	  // No need to instantiate
	  private CollectionUtil()
	  {
	  }

	  /// <summary>
	  /// Helper method that creates a singleton map.
	  /// 
	  /// Alternative for Collections.singletonMap(), since that method returns a
	  /// generic typed map <K,T> depending on the input type, but we often need a
	  /// <String, Object> map.
	  /// </summary>
	  public static IDictionary<string, object> singletonMap(string key, object value)
	  {
		IDictionary<string, object> map = new Dictionary<string, object>();
		map[key] = value;
		return map;
	  }

	  /// <summary>
	  /// Helper method to easily create a map.
	  /// 
	  /// Takes as input a varargs containing the key1, value1, key2, value2, etc.
	  /// Note: altough an Object, we will cast the key to String internally.
	  /// </summary>
	  public static IDictionary<string, object> map(params object[] objects)
	  {

		if (objects.Length % 2 != 0)
		{
		  throw new ActivitiIllegalArgumentException("The input should always be even since we expect a list of key-value pairs!");
		}

		IDictionary<string, object> map = new Dictionary<string, object>();
		for (int i = 0; i < objects.Length; i += 2)
		{
		  map[(string) objects[i]] = objects[i + 1];
		}

		return map;
	  }

	}

}