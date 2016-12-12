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
namespace org.activiti.engine.impl.persistence.entity
{

	using HasRevision = org.activiti.engine.impl.db.HasRevision;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using ValueFields = org.activiti.engine.impl.variable.ValueFields;

	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	public interface VariableInstance : ValueFields, PersistentObject, HasRevision
	{

	  string Name {set;}

	  string LocalizedName {get;set;}


	  string LocalizedDescription {get;set;}


	  string ProcessInstanceId {set;get;}

	  string ExecutionId {set;get;}

	  object Value {get;set;}


	  string TypeName {get;set;}



	  string TaskId {get;set;}



	}
}