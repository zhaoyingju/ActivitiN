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
namespace org.activiti.engine.impl.bpmn.data
{

	/// <summary>
	/// Represents a structure definition based on fields
	/// 
	/// @author Esteban Robles Luna
	/// </summary>
	public interface FieldBaseStructureDefinition : StructureDefinition
	{

	  /// <summary>
	  /// Obtains the number of fields that this structure has
	  /// </summary>
	  /// <returns> the number of fields that this structure has </returns>
	  int FieldSize {get;}

	  /// <summary>
	  /// Obtains the name of the field in the index position
	  /// </summary>
	  /// <param name="index"> the position of the field </param>
	  /// <returns> the name of the field </returns>
	  string getFieldNameAt(int index);

	  /// <summary>
	  /// Obtains the type of the field in the index position
	  /// </summary>
	  /// <param name="index"> the position of the field </param>
	  /// <returns> the type of the field </returns>
	  Type getFieldTypeAt(int index);
	}

}