using System;

namespace org.activiti.engine.impl.history.handler
{

	using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;
	using ExecutionListener = org.activiti.engine.@delegate.ExecutionListener;


	[Serializable]
	public class CallActivityStartHandler : ExecutionListener
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void notify(org.activiti.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void notify(DelegateExecution execution)
	  {

	  }

	}

}