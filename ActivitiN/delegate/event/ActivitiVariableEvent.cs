namespace org.activiti.engine.@delegate.@event
{

	using VariableType = org.activiti.engine.impl.variable.VariableType;


	/// <summary>
	/// An <seealso cref="ActivitiEvent"/> related to a single variable.
	/// </summary>
	public interface ActivitiVariableEvent : ActivitiEvent
	{

		/// <returns> the name of the variable involved. </returns>
		string VariableName {get;}

		/// <returns> the current value of the variable. </returns>
		object VariableValue {get;}

		/// <returns> The <seealso cref="VariableType"/> of the variable. </returns>
		VariableType VariableType {get;}


		/// <returns> the id of the execution the variable is set on. </returns>
		string ExecutionId {get;}

		/// <returns> the id of the task the variable has been set on. </returns>
		string TaskId {get;}
	}

}