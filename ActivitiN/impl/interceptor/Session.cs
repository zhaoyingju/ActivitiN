namespace org.activiti.engine.impl.interceptor
{
	public interface Session
	{
	  void flush();

	  void close();
	}
}