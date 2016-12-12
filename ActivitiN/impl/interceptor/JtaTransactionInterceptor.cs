using System;

/* Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace org.activiti.engine.impl.interceptor
{


	using TransactionPropagation = org.activiti.engine.impl.cfg.TransactionPropagation;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// @author Guillaume Nodet
	/// </summary>
	public class JtaTransactionInterceptor : AbstractCommandInterceptor
	{

	  private static readonly Logger LOGGER = LoggerFactory.getLogger(typeof(JtaTransactionInterceptor));

	  private readonly TransactionManager transactionManager;

	  public JtaTransactionInterceptor(TransactionManager transactionManager)
	  {
		this.transactionManager = transactionManager;
	  }

	  public override T execute<T>(CommandConfig config, Command<T> command)
	  {
		LOGGER.debug("Running command with propagation {}", config.TransactionPropagation);

		if (config.TransactionPropagation == TransactionPropagation.NOT_SUPPORTED)
		{
		  return next.execute(config, command);
		}

		bool requiresNew = config.TransactionPropagation == TransactionPropagation.REQUIRES_NEW;
		Transaction oldTx = null;
		try
		{
		  bool existing = Existing;
		  bool isNew = !existing || requiresNew;
		  if (existing && requiresNew)
		  {
			oldTx = doSuspend();
		  }
		  if (isNew)
		  {
			doBegin();
		  }
		  T result;
		  try
		  {
			result = next.execute(config, command);
		  }
		  catch (Exception ex)
		  {
			doRollback(isNew, ex);
			throw ex;
		  }
		  catch (Exception err)
		  {
			doRollback(isNew, err);
			throw err;
		  }
		  catch (Exception ex)
		  {
			doRollback(isNew, ex);
			throw new UndeclaredThrowableException(ex, "TransactionCallback threw undeclared checked exception");
		  }
		  if (isNew)
		  {
			doCommit();
		  }
		  return result;
		}
		finally
		{
		  doResume(oldTx);
		}
	  }

	  private void doBegin()
	  {
		try
		{
		  transactionManager.begin();
		}
		catch (NotSupportedException e)
		{
		  throw new TransactionException("Unable to begin transaction", e);
		}
		catch (SystemException e)
		{
		  throw new TransactionException("Unable to begin transaction", e);
		}
	  }

	  private bool Existing
	  {
		  get
		  {
			try
			{
			  return transactionManager.Status != Status.STATUS_NO_TRANSACTION;
			}
			catch (SystemException e)
			{
			  throw new TransactionException("Unable to retrieve transaction status", e);
			}
		  }
	  }

	  private Transaction doSuspend()
	  {
		try
		{
		  return transactionManager.suspend();
		}
		catch (SystemException e)
		{
		  throw new TransactionException("Unable to suspend transaction", e);
		}
	  }

	  private void doResume(Transaction tx)
	  {
		if (tx != null)
		{
		  try
		  {
			transactionManager.resume(tx);
		  }
		  catch (SystemException e)
		  {
			throw new TransactionException("Unable to resume transaction", e);
		  }
		  catch (InvalidTransactionException e)
		  {
			throw new TransactionException("Unable to resume transaction", e);
		  }
		}
	  }

	  private void doCommit()
	  {
		try
		{
		  transactionManager.commit();
		}
		catch (HeuristicMixedException e)
		{
		  throw new TransactionException("Unable to commit transaction", e);
		}
		catch (HeuristicRollbackException e)
		{
		  throw new TransactionException("Unable to commit transaction", e);
		}
		catch (RollbackException e)
		{
		  throw new TransactionException("Unable to commit transaction", e);
		}
		catch (SystemException e)
		{
		  throw new TransactionException("Unable to commit transaction", e);
		}
		catch (Exception e)
		{
		  doRollback(true, e);
		  throw e;
		}
		catch (Exception e)
		{
		  doRollback(true, e);
		  throw e;
		}
	  }

	  private void doRollback(bool isNew, Exception originalException)
	  {
		Exception rollbackEx = null;
		try
		{
		  if (isNew)
		  {
			transactionManager.rollback();
		  }
		  else
		  {
			transactionManager.setRollbackOnly();
		  }
		}
		catch (SystemException e)
		{
		  LOGGER.debug("Error when rolling back transaction", e);
		}
		catch (Exception e)
		{
		  rollbackEx = e;
		  throw e;
		}
		catch (Exception e)
		{
		  rollbackEx = e;
		  throw e;
		}
		finally
		{
		  if (rollbackEx != null && originalException != null)
		  {
			LOGGER.error("Error when rolling back transaction, orginal exception was:", originalException);
		  }
		}
	  }

	  private class TransactionException : Exception
	  {

		internal const long serialVersionUID = 1L;

		internal TransactionException()
		{
		}

		internal TransactionException(string s) : base(s)
		{
		}

		internal TransactionException(string s, Exception throwable) : base(s, throwable)
		{
		}

		internal TransactionException(Exception throwable) : base(throwable)
		{
		}
	  }
	}

}