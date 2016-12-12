using System;

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

	using SubProcessActivityBehavior = org.activiti.engine.impl.pvm.@delegate.SubProcessActivityBehavior;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using ScopeImpl = org.activiti.engine.impl.pvm.process.ScopeImpl;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class AtomicOperationProcessEnd : AbstractEventAtomicOperation
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(AtomicOperationProcessEnd));

	  protected internal override ScopeImpl getScope(InterpretableExecution execution)
	  {
		return execution.ProcessDefinition;
	  }

	  protected internal override string EventName
	  {
		  get
		  {
			return org.activiti.engine.impl.pvm.PvmEvent.EVENTNAME_END;
		  }
	  }

	  protected internal override void eventNotificationsCompleted(InterpretableExecution execution)
	  {
		InterpretableExecution superExecution = execution.SuperExecution;
		SubProcessActivityBehavior subProcessActivityBehavior = null;

		// copy variables before destroying the ended sub process instance
		if (superExecution != null)
		{
		  ActivityImpl activity = (ActivityImpl) superExecution.Activity;
		  subProcessActivityBehavior = (SubProcessActivityBehavior) activity.ActivityBehavior;
		  try
		  {
			subProcessActivityBehavior.completing(superExecution, execution);
		  }
		  catch (Exception e)
		  {
			  log.error("Error while completing sub process of execution {}", execution, e);
			  throw e;
		  }
		  catch (Exception e)
		  {
			  log.error("Error while completing sub process of execution {}", execution, e);
			  throw new ActivitiException("Error while completing sub process of execution " + execution, e);
		  }
		}

		execution.destroy();
		execution.remove();

		// and trigger execution afterwards
		if (superExecution != null)
		{
		  superExecution.setSubProcessInstance(null);
		  try
		  {
			  subProcessActivityBehavior.completed(superExecution);
		  }
		  catch (Exception e)
		  {
			  log.error("Error while completing sub process of execution {}", execution, e);
			  throw e;
		  }
		  catch (Exception e)
		  {
			  log.error("Error while completing sub process of execution {}", execution, e);
			  throw new ActivitiException("Error while completing sub process of execution " + execution, e);
		  }
		}
	  }
	}

}