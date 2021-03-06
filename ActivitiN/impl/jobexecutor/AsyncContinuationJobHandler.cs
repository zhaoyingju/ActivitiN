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

	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ExecutionEntity = org.activiti.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using AtomicOperation = org.activiti.engine.impl.pvm.runtime.AtomicOperation;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class AsyncContinuationJobHandler : JobHandler
	{

	  public const string TYPE = "async-continuation";

	  public virtual string Type
	  {
		  get
		  {
			return TYPE;
		  }
	  }

	  public virtual void execute(JobEntity job, string configuration, ExecutionEntity execution, CommandContext commandContext)
	  {
		// ATM only AtomicOperationTransitionCreateScope can be performed asynchronously 
		AtomicOperation atomicOperation = org.activiti.engine.impl.pvm.runtime.AtomicOperation_Fields.TRANSITION_CREATE_SCOPE;
		commandContext.performOperation(atomicOperation, execution);
	  }

	}

}