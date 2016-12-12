using System;

namespace org.activiti.engine.@event
{

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public interface EventLogEntry
	{

			long LogNumber {get;}

			string Type {get;}

			string ProcessDefinitionId {get;}

			string ProcessInstanceId {get;}

			string ExecutionId {get;}

			string TaskId {get;}

			DateTime TimeStamp {get;}

			string UserId {get;}

			sbyte[] Data {get;}

	}

}