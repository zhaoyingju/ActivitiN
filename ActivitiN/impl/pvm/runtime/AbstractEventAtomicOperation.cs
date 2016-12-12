using System;
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

	using ExecutionListener = org.activiti.engine.@delegate.ExecutionListener;
	using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public abstract class AbstractEventAtomicOperation : AtomicOperation
	{

	  public virtual bool isAsync(InterpretableExecution execution)
	  {
		return false;
	  }

	  public virtual void execute(InterpretableExecution execution)
	  {
		ScopeImpl scope = getScope(execution);
		IList<ExecutionListener> exectionListeners = scope.getExecutionListeners(EventName);
		int executionListenerIndex = execution.ExecutionListenerIndex;

		if (exectionListeners.Count > executionListenerIndex)
		{
		  execution.EventName = EventName;
		  execution.EventSource = scope;
		  ExecutionListener listener = exectionListeners[executionListenerIndex];
		  try
		  {
			listener.notify(execution);
		  }
		  catch (Exception e)
		  {
			throw e;
		  }
		  catch (Exception e)
		  {
			throw new PvmException("couldn't execute event listener : " + e.Message, e);
		  }
		  execution.ExecutionListenerIndex = executionListenerIndex + 1;
		  execution.performOperation(this);

		}
		else
		{
		  execution.ExecutionListenerIndex = 0;
		  execution.EventName = null;
		  execution.EventSource = null;

		  eventNotificationsCompleted(execution);
		}
	  }

	  protected internal abstract ScopeImpl getScope(InterpretableExecution execution);
	  protected internal abstract string EventName {get;}
	  protected internal abstract void eventNotificationsCompleted(InterpretableExecution execution);
	}

}