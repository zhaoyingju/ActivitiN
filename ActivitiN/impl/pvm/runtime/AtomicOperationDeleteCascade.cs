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

namespace org.activiti.engine.impl.pvm.runtime
{


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class AtomicOperationDeleteCascade : AtomicOperation
	{

	  public virtual bool isAsync(InterpretableExecution execution)
	  {
		return false;
	  }

	  public virtual void execute(InterpretableExecution execution)
	  {
		InterpretableExecution firstLeaf = findFirstLeaf(execution);

		if (firstLeaf.getSubProcessInstance() != null)
		{
		  firstLeaf.getSubProcessInstance().deleteCascade(execution.DeleteReason);
		}

		firstLeaf.performOperation(AtomicOperation_Fields.DELETE_CASCADE_FIRE_ACTIVITY_END);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected InterpretableExecution findFirstLeaf(InterpretableExecution execution)
	  protected internal virtual InterpretableExecution findFirstLeaf(InterpretableExecution execution)
	  {
		IList<InterpretableExecution> executions = (IList<InterpretableExecution>) execution.Executions;
		if (executions.Count > 0)
		{
		  return findFirstLeaf(executions[0]);
		}
		return execution;
	  }
	}

}