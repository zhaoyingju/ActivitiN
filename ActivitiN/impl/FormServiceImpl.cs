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

namespace org.activiti.engine.impl
{

	using StartFormData = org.activiti.engine.form.StartFormData;
	using TaskFormData = org.activiti.engine.form.TaskFormData;
	using GetRenderedStartFormCmd = org.activiti.engine.impl.cmd.GetRenderedStartFormCmd;
	using GetRenderedTaskFormCmd = org.activiti.engine.impl.cmd.GetRenderedTaskFormCmd;
	using GetStartFormCmd = org.activiti.engine.impl.cmd.GetStartFormCmd;
	using GetTaskFormCmd = org.activiti.engine.impl.cmd.GetTaskFormCmd;
	using GetFormKeyCmd = org.activiti.engine.impl.cmd.GetFormKeyCmd;
	using SubmitStartFormCmd = org.activiti.engine.impl.cmd.SubmitStartFormCmd;
	using SubmitTaskFormCmd = org.activiti.engine.impl.cmd.SubmitTaskFormCmd;
	using ProcessInstance = org.activiti.engine.runtime.ProcessInstance;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Falko Menge (camunda)
	/// </summary>
	public class FormServiceImpl : ServiceImpl, FormService
	{

	  public virtual object getRenderedStartForm(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetRenderedStartFormCmd(processDefinitionId, null));
	  }

	  public virtual object getRenderedStartForm(string processDefinitionId, string engineName)
	  {
		return commandExecutor.execute(new GetRenderedStartFormCmd(processDefinitionId, engineName));
	  }

	  public virtual object getRenderedTaskForm(string taskId)
	  {
		return commandExecutor.execute(new GetRenderedTaskFormCmd(taskId, null));
	  }

	  public virtual object getRenderedTaskForm(string taskId, string engineName)
	  {
		return commandExecutor.execute(new GetRenderedTaskFormCmd(taskId, engineName));
	  }

	  public virtual StartFormData getStartFormData(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetStartFormCmd(processDefinitionId));
	  }

	  public virtual TaskFormData getTaskFormData(string taskId)
	  {
		return commandExecutor.execute(new GetTaskFormCmd(taskId));
	  }

	  public virtual ProcessInstance submitStartFormData(string processDefinitionId, IDictionary<string, string> properties)
	  {
		return commandExecutor.execute(new SubmitStartFormCmd(processDefinitionId, null, properties));
	  }

	  public virtual ProcessInstance submitStartFormData(string processDefinitionId, string businessKey, IDictionary<string, string> properties)
	  {
		  return commandExecutor.execute(new SubmitStartFormCmd(processDefinitionId, businessKey, properties));
	  }

	  public virtual void submitTaskFormData(string taskId, IDictionary<string, string> properties)
	  {
		commandExecutor.execute(new SubmitTaskFormCmd(taskId, properties, true));
	  }

	  public virtual string getStartFormKey(string processDefinitionId)
	  {
		return commandExecutor.execute(new GetFormKeyCmd(processDefinitionId));
	  }

	  public virtual string getTaskFormKey(string processDefinitionId, string taskDefinitionKey)
	  {
		return commandExecutor.execute(new GetFormKeyCmd(processDefinitionId, taskDefinitionKey));
	  }

	  public virtual void saveFormData(string taskId, IDictionary<string, string> properties)
	  {
		commandExecutor.execute(new SubmitTaskFormCmd(taskId, properties, false));
	  }
	}

}