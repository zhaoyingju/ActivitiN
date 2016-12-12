namespace org.activiti.engine.impl.persistence
{

	using IdGenerator = org.activiti.engine.impl.cfg.IdGenerator;

	using EthernetAddress = com.fasterxml.uuid.EthernetAddress;
	using Generators = com.fasterxml.uuid.Generators;
	using TimeBasedGenerator = com.fasterxml.uuid.impl.TimeBasedGenerator;

	/// <summary>
	/// <seealso cref="IdGenerator"/> implementation based on the current time and the ethernet
	/// address of the machine it is running on.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class StrongUuidGenerator : IdGenerator
	{

	  // different ProcessEngines on the same classloader share one generator.
	  protected internal static TimeBasedGenerator timeBasedGenerator;

	  public StrongUuidGenerator()
	  {
		ensureGeneratorInitialized();
	  }

	  protected internal virtual void ensureGeneratorInitialized()
	  {
		if (timeBasedGenerator == null)
		{
		  lock (typeof(StrongUuidGenerator))
		  {
			if (timeBasedGenerator == null)
			{
			  timeBasedGenerator = Generators.timeBasedGenerator(EthernetAddress.fromInterface());
			}
		  }
		}
	  }

	  public virtual string NextId
	  {
		  get
		  {
			return timeBasedGenerator.generate().ToString();
		  }
	  }

	}

}