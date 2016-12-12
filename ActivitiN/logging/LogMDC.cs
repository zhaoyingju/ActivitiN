namespace org.activiti.engine.logging
{

	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using MDC = org.slf4j.MDC;

	/// <summary>
	/// Constants and functions for MDC (Mapped Diagnostic Context) logging
	/// 
	/// @author Saeid Mirzaei
	/// </summary>

	public class LogMDC
	{

	  public const string LOG_MDC_PROCESSDEFINITION_ID = "mdcProcessDefinitionID";
	  public const string LOG_MDC_EXECUTION_ID = "mdcExecutionId";
	  public const string LOG_MDC_PROCESSINSTANCE_ID = "mdcProcessInstanceID";
	  public const string LOG_MDC_BUSINESS_KEY = "mdcBusinessKey";
	  public const string LOG_MDC_TASK_ID = "mdcTaskId";

	  internal static bool enabled = false;

	  public static bool MDCEnabled
	  {
		  get
		  {
			return enabled;
		  }
		  set
		  {
			enabled = value;
		  }
	  }


	  public static void putMDCExecution(ActivityExecution e)
	  {
		if (e.Id != null)
		{
		  MDC.put(LOG_MDC_EXECUTION_ID, e.Id);
		}
		if (e.ProcessDefinitionId != null)
		{
		  MDC.put(LOG_MDC_PROCESSDEFINITION_ID, e.ProcessDefinitionId);
		}
		if (e.ProcessInstanceId != null)
		{
		  MDC.put(LOG_MDC_PROCESSINSTANCE_ID, e.ProcessInstanceId);
		}
		if (e.ProcessBusinessKey != null)
		{
		  MDC.put(LOG_MDC_BUSINESS_KEY, e.ProcessBusinessKey);
		}


	  }

	  public static void clear()
	  {
		MDC.clear();
	  }
	}

}