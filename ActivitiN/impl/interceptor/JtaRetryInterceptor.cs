namespace org.activiti.engine.impl.interceptor
{



	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// We cannot perform a retry if we are called in an existing transaction. In
	/// that case, the transaction will be marked "rollback-only" after the first
	/// ActivitiOptimisticLockingException.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class JtaRetryInterceptor : RetryInterceptor
	{

	  private readonly Logger log = LoggerFactory.getLogger(typeof(JtaRetryInterceptor));

	  protected internal readonly TransactionManager transactionManager;

	  public JtaRetryInterceptor(TransactionManager transactionManager)
	  {
		this.transactionManager = transactionManager;
	  }

	  public override T execute<T>(CommandConfig config, Command<T> command)
	  {
		if (calledInsideTransaction())
		{
		  log.trace("Called inside transaction, skipping the retry interceptor.");
		  return next.execute(config, command);
		}
		else
		{
		  return base.execute(config, command);
		}
	  }

	  protected internal virtual bool calledInsideTransaction()
	  {
		try
		{
		  return transactionManager.Status != Status.STATUS_NO_TRANSACTION;
		}
		catch (SystemException e)
		{
		  throw new ActivitiException("Could not determine the current status of the transaction manager: " + e.Message, e);
		}
	  }

	}

}