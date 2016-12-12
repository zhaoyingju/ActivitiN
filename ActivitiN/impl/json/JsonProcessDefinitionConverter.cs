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
namespace org.activiti.engine.impl.json
{

	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using JSONObject = org.activiti.engine.impl.util.json.JSONObject;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class JsonProcessDefinitionConverter : JsonObjectConverter<ProcessDefinition>
	{

	  public virtual JSONObject toJsonObject(ProcessDefinition processDefinition)
	  {
		ProcessDefinitionEntity processDefinitionEntity = (ProcessDefinitionEntity) processDefinition;
		JSONObject jsonObject = new JSONObject();
		jsonObject.put("id", processDefinitionEntity.Id);
		if (processDefinitionEntity.Key != null)
		{
		  jsonObject.put("key", processDefinitionEntity.Key);
		}
		if (processDefinitionEntity.DeploymentId != null)
		{
		  jsonObject.put("deploymentId", processDefinitionEntity.DeploymentId);
		}
		return jsonObject;
	  }

	  public virtual ProcessDefinition toObject(Reader reader)
	  {
		return null;
	  }
	}

}