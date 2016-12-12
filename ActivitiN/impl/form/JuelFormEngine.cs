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

	using FormData = org.activiti.engine.form.FormData;
	using StartFormData = org.activiti.engine.form.StartFormData;
	using TaskFormData = org.activiti.engine.form.TaskFormData;
	using Context = org.activiti.engine.impl.context.Context;
	using ResourceEntity = org.activiti.engine.impl.persistence.entity.ResourceEntity;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;
	using ScriptingEngines = org.activiti.engine.impl.scripting.ScriptingEngines;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class JuelFormEngine : FormEngine
	{

	  public virtual string Name
	  {
		  get
		  {
			return "juel";
		  }
	  }

	  public virtual object renderStartForm(StartFormData startForm)
	  {
		if (startForm.FormKey == null)
		{
		  return null;
		}
		string formTemplateString = getFormTemplateString(startForm, startForm.FormKey);
		ScriptingEngines scriptingEngines = Context.ProcessEngineConfiguration.ScriptingEngines;
		return scriptingEngines.evaluate(formTemplateString, ScriptingEngines.DEFAULT_SCRIPTING_LANGUAGE, null);
	  }

	  public virtual object renderTaskForm(TaskFormData taskForm)
	  {
		if (taskForm.FormKey == null)
		{
		  return null;
		}
		string formTemplateString = getFormTemplateString(taskForm, taskForm.FormKey);
		ScriptingEngines scriptingEngines = Context.ProcessEngineConfiguration.ScriptingEngines;
		TaskEntity task = (TaskEntity) taskForm.Task;
		return scriptingEngines.evaluate(formTemplateString, ScriptingEngines.DEFAULT_SCRIPTING_LANGUAGE, task.getExecution());
	  }

	  protected internal virtual string getFormTemplateString(FormData formInstance, string formKey)
	  {
		string deploymentId = formInstance.DeploymentId;

		ResourceEntity resourceStream = Context.CommandContext.ResourceEntityManager.findResourceByDeploymentIdAndResourceName(deploymentId, formKey);

		if (resourceStream == null)
		{
		  throw new ActivitiObjectNotFoundException("Form with formKey '" + formKey + "' does not exist", typeof(string));
		}

		sbyte[] resourceBytes = resourceStream.Bytes;
		string encoding = "UTF-8";
		string formTemplateString = "";
		try
		{
		  formTemplateString = StringHelperClass.NewString(resourceBytes, encoding);
		}
		catch (UnsupportedEncodingException e)
		{
		  throw new ActivitiException("Unsupported encoding of :" + encoding, e);
		}
		return formTemplateString;
	  }
	}

}