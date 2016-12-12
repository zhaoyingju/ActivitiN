using System.Collections;


namespace org.activiti.engine.@delegate.@event.impl
{


	/// <summary>
	/// Base class for all <seealso cref="ActivitiEntityEvent"/> implementations, related to entities with variables.
	/// 
	/// @author Tijs Rademakers
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public class ActivitiEntityWithVariablesEventImpl extends ActivitiEntityEventImpl implements org.activiti.engine.delegate.event.ActivitiEntityWithVariablesEvent
	public class ActivitiEntityWithVariablesEventImpl : ActivitiEntityEventImpl, ActivitiEntityWithVariablesEvent
	{

	  protected internal IDictionary variables;
	  protected internal bool localScope;

	  public ActivitiEntityWithVariablesEventImpl(object entity, IDictionary variables, bool localScope, ActivitiEventType type) : base(entity, type)
	  {

			this.variables = variables;
			this.localScope = localScope;
	  }

	  public override IDictionary Variables
	  {
		  get
		  {
			return variables;
		  }
	  }

	  public override bool LocalScope
	  {
		  get
		  {
			return localScope;
		  }
	  }
	}

}