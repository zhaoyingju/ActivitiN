using System.Collections;

namespace org.activiti.engine.@delegate.@event
{


	/// <summary>
	/// An <seealso cref="ActivitiEntityEvent"/> related to a single entity.
	/// </summary>
	public interface ActivitiEntityWithVariablesEvent : ActivitiEntityEvent
	{
		IDictionary Variables {get;}

		bool LocalScope {get;}
	}

}