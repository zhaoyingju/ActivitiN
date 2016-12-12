using System;
using System.Collections.Generic;

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
namespace org.activiti.engine.impl.cfg.standalone
{


	using DbSqlSession = org.activiti.engine.impl.db.DbSqlSession;
	using org.activiti.engine.impl.interceptor;
	using CommandConfig = org.activiti.engine.impl.interceptor.CommandConfig;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using SqlSession = org.apache.ibatis.session.SqlSession;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class StandaloneMybatisTransactionContext : TransactionContext
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(StandaloneMybatisTransactionContext));

	  protected internal CommandContext commandContext;
	  protected internal IDictionary<TransactionState, IList<TransactionListener>> stateTransactionListeners = null;

	  public StandaloneMybatisTransactionContext(CommandContext commandContext)
	  {
		this.commandContext = commandContext;
	  }

	  public virtual void addTransactionListener(TransactionState transactionState, TransactionListener transactionListener)
	  {
		if (stateTransactionListeners == null)
		{
		  stateTransactionListeners = new Dictionary<TransactionState, IList<TransactionListener>>();
		}
		IList<TransactionListener> transactionListeners = stateTransactionListeners[transactionState];
		if (transactionListeners == null)
		{
		  transactionListeners = new List<TransactionListener>();
		  stateTransactionListeners[transactionState] = transactionListeners;
		}
		transactionListeners.Add(transactionListener);
	  }

	  public virtual void commit()
	  {

		log.debug("firing event committing...");
		fireTransactionEvent(TransactionState.COMMITTING, false);

		log.debug("committing the ibatis sql session...");
		DbSqlSession.commit();
		log.debug("firing event committed...");
		fireTransactionEvent(TransactionState.COMMITTED, true);

	  }

	  /// <summary>
	  /// Fires the event for the provided <seealso cref="TransactionState"/>.
	  /// </summary>
	  /// <param name="transactionState"> The <seealso cref="TransactionState"/> for which the listeners will be called. </param>
	  /// <param name="executeInNewContext"> If true, the listeners will be called in a new command context.
	  ///                            This is needed for example when firing the <seealso cref="TransactionState#COMMITTED"/>
	  ///                            event: the transaction is already committed and executing logic in the same
	  ///                            context could lead to strange behaviour (for example doing a <seealso cref="SqlSession#update(String)"/>
	  ///                            would actually roll back the update (as the MyBatis context is already committed
	  ///                            and the internal flags have not been correctly set). </param>
	  protected internal virtual void fireTransactionEvent(TransactionState transactionState, bool executeInNewContext)
	  {
		if (stateTransactionListeners == null)
		{
		  return;
		}
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.activiti.engine.impl.cfg.TransactionListener> transactionListeners = stateTransactionListeners.get(transactionState);
		IList<TransactionListener> transactionListeners = stateTransactionListeners[transactionState];
		if (transactionListeners == null)
		{
		  return;
		}

		if (executeInNewContext)
		{
		  CommandExecutor commandExecutor = commandContext.ProcessEngineConfiguration.CommandExecutor;
		  CommandConfig commandConfig = new CommandConfig(false, TransactionPropagation.REQUIRES_NEW);
		  commandExecutor.execute(commandConfig, new CommandAnonymousInnerClassHelper(this, transactionListeners));
		}
		else
		{
		  executeTransactionListeners(transactionListeners, commandContext);
		}

	  }

	  private class CommandAnonymousInnerClassHelper : Command<Void>
	  {
		  private readonly StandaloneMybatisTransactionContext outerInstance;

		  private IList<TransactionListener> transactionListeners;

		  public CommandAnonymousInnerClassHelper(StandaloneMybatisTransactionContext outerInstance, IList<TransactionListener> transactionListeners)
		  {
			  this.outerInstance = outerInstance;
			  this.transactionListeners = transactionListeners;
		  }

		  public virtual Void execute(CommandContext commandContext)
		  {
			outerInstance.executeTransactionListeners(transactionListeners, commandContext);
			return null;
		  }
	  }

	  protected internal virtual void executeTransactionListeners(IList<TransactionListener> transactionListeners, CommandContext commandContext)
	  {
		foreach (TransactionListener transactionListener in transactionListeners)
		{
		  transactionListener.execute(commandContext);
		}
	  }

	  protected internal virtual DbSqlSession DbSqlSession
	  {
		  get
		  {
			return commandContext.getSession(typeof(DbSqlSession));
		  }
	  }

	  public virtual void rollback()
	  {
		try
		{
		  try
		  {
			log.debug("firing event rolling back...");
			fireTransactionEvent(TransactionState.ROLLINGBACK, false);

		  }
		  catch (Exception exception)
		  {
			log.info("Exception during transaction: {}",exception.Message);
			commandContext.exception(exception);
		  }
		  finally
		  {
			log.debug("rolling back ibatis sql session...");
			DbSqlSession.rollback();
		  }

		}
		catch (Exception exception)
		{
		  log.info("Exception during transaction: {}",exception.Message);
		  commandContext.exception(exception);

		}
		finally
		{
		  log.debug("firing event rolled back...");
		  fireTransactionEvent(TransactionState.ROLLED_BACK, true);
		}
	  }
	}

}