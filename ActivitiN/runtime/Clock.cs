using System;

namespace org.activiti.engine.runtime
{


	/// <summary>
	/// This interface provides full access to the clock
	/// </summary>
	public interface Clock : ClockReader
	{
	  void reset();
	}
}