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
namespace org.activiti.engine
{

	/// <summary>
	/// Represents information about the initialization of the process engine. 
	/// </summary>
	/// <seealso cref= ProcessEngines
	/// @author Tom Baeyens </seealso>
	public interface ProcessEngineInfo
	{

	  /// <summary>
	  /// Returns the name of the process engine.
	  /// </summary>
	  string Name {get;}

	  /// <summary>
	  /// Returns the resources the engine was configured from.
	  /// </summary>
	  string ResourceUrl {get;}

	  /// <summary>
	  /// Returns the exception stacktrace in case an exception occurred while initializing
	  /// the engine. When no exception occured, null is returned.
	  /// </summary>
	  string Exception {get;}

	}
}