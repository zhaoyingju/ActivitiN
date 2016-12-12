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
namespace org.activiti.engine.impl.cmd
{

	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using org.activiti.engine.impl.persistence.deploy;
	using ProcessDefinitionEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class SetProcessDefinitionCategoryCmd : Command<Void>
	{

	  protected internal string processDefinitionId;
	  protected internal string category;

	  public SetProcessDefinitionCategoryCmd(string processDefinitionId, string category)
	  {
		this.processDefinitionId = processDefinitionId;
		this.category = category;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {

		if (processDefinitionId == null)
		{
		  throw new ActivitiIllegalArgumentException("Process definition id is null");
		}

		ProcessDefinitionEntity processDefinition = commandContext.ProcessDefinitionEntityManager.findProcessDefinitionById(processDefinitionId);

		if (processDefinition == null)
		{
		  throw new ActivitiObjectNotFoundException("No process definition found for id = '" + processDefinitionId + "'", typeof(ProcessDefinition));
		}

		// Update category
		processDefinition.Category = category;

		// Remove process definition from cache, it will be refetched later
		DeploymentCache<ProcessDefinitionEntity> processDefinitionCache = commandContext.ProcessEngineConfiguration.ProcessDefinitionCache;
		if (processDefinitionCache != null)
		{
		  processDefinitionCache.remove(processDefinitionId);
		}

		if (commandContext.EventDispatcher.Enabled)
		{
		  commandContext.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createEntityEvent(ActivitiEventType.ENTITY_UPDATED, processDefinition));
		}

		return null;
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId;
		  }
		  set
		  {
			this.processDefinitionId = value;
		  }
	  }


	  public virtual string Category
	  {
		  get
		  {
			return category;
		  }
		  set
		  {
			this.category = value;
		  }
	  }


	}

}