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

namespace org.activiti.engine.impl.cfg.jta
{


	using Context = org.activiti.engine.impl.context.Context;
	using org.activiti.engine.impl.interceptor;
	using CommandConfig = org.activiti.engine.impl.interceptor.CommandConfig;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class JtaTransactionContext : TransactionContext
	{

	  protected internal readonly TransactionManager transactionManager;

	  public JtaTransactionContext(TransactionManager transactionManager)
	  {
		this.transactionManager = transactionManager;
	  }

	  public virtual void commit()
	  {
		// managed transaction, ignore
	  }

	  public virtual void rollback()
	  {
		// managed transaction, mark rollback-only if not done so already.
		try
		{
		  Transaction transaction = Transaction;
		  int status = transaction.Status;
		  if (status != Status.STATUS_NO_TRANSACTION && status != Status.STATUS_ROLLEDBACK)
		  {
			transaction.setRollbackOnly();
		  }
		}
		catch (IllegalStateException)
		{
		  throw new ActivitiException("Unexpected IllegalStateException while marking transaction rollback only");
		}
		catch (SystemException)
		{
		  throw new ActivitiException("SystemException while marking transaction rollback only");
		}
	  }

	  protected internal virtual Transaction Transaction
	  {
		  get
		  {
			try
			{
			  return transactionManager.Transaction;
			}
			catch (SystemException e)
			{
			  throw new ActivitiException("SystemException while getting transaction ", e);
			}
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void addTransactionListener(org.activiti.engine.impl.cfg.TransactionState transactionState, final org.activiti.engine.impl.cfg.TransactionListener transactionListener)
	  public virtual void addTransactionListener(TransactionState transactionState, TransactionListener transactionListener)
	  {
		Transaction transaction = Transaction;
		CommandContext commandContext = Context.CommandContext;
		try
		{
		  transaction.registerSynchronization(new TransactionStateSynchronization(transactionState, transactionListener, commandContext));
		}
		catch (IllegalStateException e)
		{
		  throw new ActivitiException("IllegalStateException while registering synchronization ", e);
		}
		catch (RollbackException e)
		{
		  throw new ActivitiException("RollbackException while registering synchronization ", e);
		}
		catch (SystemException e)
		{
		  throw new ActivitiException("SystemException while registering synchronization ", e);
		}
	  }

	  public class TransactionStateSynchronization : Synchronization
	  {

		protected internal readonly TransactionListener transactionListener;
		protected internal readonly TransactionState transactionState;
		internal readonly CommandContext commandContext;

		public TransactionStateSynchronization(TransactionState transactionState, TransactionListener transactionListener, CommandContext commandContext)
		{
		  this.transactionState = transactionState;
		  this.transactionListener = transactionListener;
		  this.commandContext = commandContext;
		}

		public virtual void beforeCompletion()
		{
		  if (TransactionState.COMMITTING.Equals(transactionState) || TransactionState.ROLLINGBACK.Equals(transactionState))
		  {
			transactionListener.execute(commandContext);
		  }
		}

		public virtual void afterCompletion(int status)
		{
		  if (Status.STATUS_ROLLEDBACK == status && TransactionState.ROLLED_BACK.Equals(transactionState))
		  {
			executeTransactionListenerInNewCommandContext();
		  }
		  else if (Status.STATUS_COMMITTED == status && TransactionState.COMMITTED.Equals(transactionState))
		  {
			executeTransactionListenerInNewCommandContext();
		  }
		}

		protected internal virtual void executeTransactionListenerInNewCommandContext()
		{
		  CommandExecutor commandExecutor = commandContext.ProcessEngineConfiguration.CommandExecutor;
		  CommandConfig commandConfig = new CommandConfig(false, TransactionPropagation.REQUIRES_NEW);
		  commandExecutor.execute(commandConfig, new CommandAnonymousInnerClassHelper(this));
		}

		private class CommandAnonymousInnerClassHelper : Command<Void>
		{
			private readonly TransactionStateSynchronization outerInstance;

			public CommandAnonymousInnerClassHelper(TransactionStateSynchronization outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual Void execute(CommandContext commandContext)
			{
			  outerInstance.transactionListener.execute(commandContext);
			  return null;
			}
		}

	  }


	}

}