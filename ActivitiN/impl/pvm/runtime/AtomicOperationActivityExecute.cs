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

	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using Context = org.activiti.engine.impl.context.Context;
	using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
	using LogMDC = org.activiti.engine.logging.LogMDC;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class AtomicOperationActivityExecute : AtomicOperation
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(AtomicOperationActivityExecute));

	  public virtual bool isAsync(InterpretableExecution execution)
	  {
		return false;
	  }

	  public virtual void execute(InterpretableExecution execution)
	  {
		ActivityImpl activity = (ActivityImpl) execution.Activity;

		ActivityBehavior activityBehavior = activity.ActivityBehavior;
		if (activityBehavior == null)
		{
		  throw new PvmException("no behavior specified in " + activity);
		}

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		log.debug("{} executes {}: {}", execution, activity, activityBehavior.GetType().FullName);

		try
		{
			if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
			{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createActivityEvent(ActivitiEventType.ACTIVITY_STARTED, execution.Activity.Id, (string) execution.Activity.getProperty("name"), execution.Id, execution.ProcessInstanceId, execution.ProcessDefinitionId, (string) activity.Properties["type"], activity.ActivityBehavior.GetType().FullName));
			}

		  activityBehavior.execute(execution);
		}
		catch (Exception e)
		{
		  throw e;
		}
		catch (Exception e)
		{
		  LogMDC.putMDCExecution(execution);
		  throw new PvmException("couldn't execute activity <" + activity.getProperty("type") + " id=\"" + activity.Id + "\" ...>: " + e.Message, e);
		}
	  }
	}

}