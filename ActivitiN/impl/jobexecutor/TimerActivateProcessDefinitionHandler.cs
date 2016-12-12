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
namespace org.activiti.engine.impl.jobexecutor
{

	using ActivateProcessDefinitionCmd = org.activiti.engine.impl.cmd.ActivateProcessDefinitionCmd;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using JSONObject = org.activiti.engine.impl.util.json.JSONObject;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class TimerActivateProcessDefinitionHandler : TimerChangeProcessDefinitionSuspensionStateJobHandler
	{

	  public const string TYPE = "activate-processdefinition";

	  public override string Type
	  {
		  get
		  {
			return TYPE;
		  }
	  }

	  public override void execute(JobEntity job, string configuration, ExecutionEntity execution, CommandContext commandContext)
	  {
		JSONObject cfgJson = new JSONObject(configuration);
		string processDefinitionId = job.ProcessDefinitionId;
		bool activateProcessInstances = getIncludeProcessInstances(cfgJson);

		ActivateProcessDefinitionCmd activateProcessDefinitionCmd = new ActivateProcessDefinitionCmd(processDefinitionId, null, activateProcessInstances, null, job.TenantId);
		activateProcessDefinitionCmd.execute(commandContext);
	  }

	}

}