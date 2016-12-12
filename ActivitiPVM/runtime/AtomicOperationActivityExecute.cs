using System;

namespace org.activiti.engine.impl.pvm.runtime
{

    using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
    using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
    using Context = org.activiti.engine.impl.context.Context;
    using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
    using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;
    using LogMDC = org.activiti.engine.logging.LogMDC;

    public class AtomicOperationActivityExecute : AtomicOperation
    {
        public virtual bool isAsync(InterpretableExecution execution)
        {
            return false;
        }

        public virtual void execute(InterpretableExecution execution)
        {
            ActivityImpl activity = (ActivityImpl)execution.Activity;

            ActivityBehavior activityBehavior = activity.ActivityBehavior;
            if (activityBehavior == null)
            {
                throw new PvmException("no behavior specified in " + activity);
            }


            //log.debug("{} executes {}: {}", execution, activity, activityBehavior.GetType().FullName);

            try
            {
                if (Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
                {
                    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
                    Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createActivityEvent(ActivitiEventType.ACTIVITY_STARTED, execution.Activity.Id, (string)execution.Activity.getProperty("name"), execution.Id, execution.ProcessInstanceId, execution.ProcessDefinitionId, (string)activity.Properties["type"], activity.ActivityBehavior.GetType().FullName));
                }

                activityBehavior.execute(execution);
            }

            catch (Exception e)
            {
                LogMDC.putMDCExecution(execution);
                throw new PvmException("couldn't execute activity <" + activity.getProperty("type") + " id=\"" + activity.Id + "\" ...>: " + e.Message, e);
            }
        }
    }
}