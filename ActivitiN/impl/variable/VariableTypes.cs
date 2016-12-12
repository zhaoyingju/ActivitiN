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

namespace org.activiti.engine.impl.variable
{

	/// <summary>
	/// Interface describing a container for all available <seealso cref="VariableType"/>s of variables.
	/// @author dsyer
	/// @author Frederik Heremans
	/// </summary>
	public interface VariableTypes
	{

	  /// <returns> the type for the given type name. 
	  /// Returns null if no type was found with the name. </returns>
	  VariableType getVariableType(string typeName);

	  /// <returns> the variable type to be used to store the given value as a variable. </returns>
	  /// <exception cref="ActivitiException"> When no available type is capable of storing the value. </exception>
	  VariableType findVariableType(object value);

	  VariableTypes addType(VariableType type);

	  /// <summary>
	  /// Add type at the given index. The index is used when finding a type for an object. When
	  /// different types can store a specific object value, the one with the smallest
	  /// index will be used.
	  /// </summary>
	  VariableTypes addType(VariableType type, int index);

	  int getTypeIndex(VariableType type);

	  int getTypeIndex(string typeName);

	  VariableTypes removeType(VariableType type);
	}
}