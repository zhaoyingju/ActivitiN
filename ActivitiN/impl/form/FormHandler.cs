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

namespace org.activiti.engine.impl.form
{


	using FormProperty = org.activiti.bpmn.model.FormProperty;
	using DeploymentEntity = org.activiti.engine.impl.persistence.entity.DeploymentEntity;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public interface FormHandler
	{

	  void parseConfiguration(IList<FormProperty> formProperties, string formKey, DeploymentEntity deployment, ProcessDefinitionEntity processDefinition);

	  void submitFormProperties(IDictionary<string, string> properties, ExecutionEntity execution);
	}

	public static class FormHandler_Fields
	{
	  public static readonly ThreadLocal<FormHandler> current = new ThreadLocal<FormHandler>();
	}

}