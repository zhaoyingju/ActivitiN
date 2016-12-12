namespace org.activiti.engine.impl.pvm.@delegate
{

	using DelegateTask = org.activiti.engine.@delegate.DelegateTask;

	public interface TaskListener
	{

	  void notify(DelegateTask delegateTask);
	}

	public static class TaskListener_Fields
	{
	  public const string EVENTNAME_CREATE = "create";
	  public const string EVENTNAME_ASSIGNMENT = "assignment";
	  public const string EVENTNAME_COMPLETE = "complete";
	}

}