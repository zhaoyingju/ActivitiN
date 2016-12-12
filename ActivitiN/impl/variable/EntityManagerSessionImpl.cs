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

namespace org.activiti.engine.impl.variable
{


	using TransactionContext = org.activiti.engine.impl.cfg.TransactionContext;
	using TransactionListener = org.activiti.engine.impl.cfg.TransactionListener;
	using TransactionState = org.activiti.engine.impl.cfg.TransactionState;
	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Frederik Heremans
	/// @author Joram Barrez
	/// </summary>
	public class EntityManagerSessionImpl : EntityManagerSession
	{

	  private EntityManagerFactory entityManagerFactory;
	  private EntityManager entityManager;
	  private bool handleTransactions;
	  private bool closeEntityManager;

	  public EntityManagerSessionImpl(EntityManagerFactory entityManagerFactory, EntityManager entityManager, bool handleTransactions, bool closeEntityManager) : this(entityManagerFactory, handleTransactions, closeEntityManager)
	  {
		this.entityManager = entityManager;
	  }

	  public EntityManagerSessionImpl(EntityManagerFactory entityManagerFactory, bool handleTransactions, bool closeEntityManager)
	  {
		this.entityManagerFactory = entityManagerFactory;
		this.handleTransactions = handleTransactions;
		this.closeEntityManager = closeEntityManager;
	  }

	  public virtual void flush()
	  {
		if (entityManager != null && (!handleTransactions || TransactionActive))
		{
		  try
		  {
			entityManager.flush();
		  }
		  catch (IllegalStateException ise)
		  {
			throw new ActivitiException("Error while flushing EntityManager, illegal state", ise);
		  }
		  catch (TransactionRequiredException tre)
		  {
			throw new ActivitiException("Cannot flush EntityManager, an active transaction is required", tre);
		  }
		  catch (PersistenceException pe)
		  {
			throw new ActivitiException("Error while flushing EntityManager: " + pe.Message, pe);
		  }
		}
	  }

	  protected internal virtual bool TransactionActive
	  {
		  get
		  {
			if (handleTransactions && entityManager.Transaction != null)
			{
			  return entityManager.Transaction.Active;
			}
			return false;
		  }
	  }

	  public virtual void close()
	  {
		if (closeEntityManager && entityManager != null && !entityManager.Open)
		{
		  try
		  {
			entityManager.close();
		  }
		  catch (IllegalStateException ise)
		  {
			throw new ActivitiException("Error while closing EntityManager, may have already been closed or it is container-managed", ise);
		  }
		}
	  }

	  public virtual EntityManager EntityManager
	  {
		  get
		  {
			if (entityManager == null)
			{
			  entityManager = EntityManagerFactory.createEntityManager();
    
			  if (handleTransactions)
			  {
				// Add transaction listeners, if transactions should be handled
				TransactionListener jpaTransactionCommitListener = new TransactionListenerAnonymousInnerClassHelper(this);
    
				TransactionListener jpaTransactionRollbackListener = new TransactionListenerAnonymousInnerClassHelper2(this);
    
				TransactionContext transactionContext = Context.CommandContext.TransactionContext;
				transactionContext.addTransactionListener(TransactionState.COMMITTED, jpaTransactionCommitListener);
				transactionContext.addTransactionListener(TransactionState.ROLLED_BACK, jpaTransactionRollbackListener);
    
				// Also, start a transaction, if one isn't started already
				if (!TransactionActive)
				{
				  entityManager.Transaction.begin();
				}
			  }
			}
    
			return entityManager;
		  }
	  }

	  private class TransactionListenerAnonymousInnerClassHelper : TransactionListener
	  {
		  private readonly EntityManagerSessionImpl outerInstance;

		  public TransactionListenerAnonymousInnerClassHelper(EntityManagerSessionImpl outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public virtual void execute(CommandContext commandContext)
		  {
			if (outerInstance.TransactionActive)
			{
			  outerInstance.entityManager.Transaction.commit();
			}
		  }
	  }

	  private class TransactionListenerAnonymousInnerClassHelper2 : TransactionListener
	  {
		  private readonly EntityManagerSessionImpl outerInstance;

		  public TransactionListenerAnonymousInnerClassHelper2(EntityManagerSessionImpl outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public virtual void execute(CommandContext commandContext)
		  {
			if (outerInstance.TransactionActive)
			{
			  outerInstance.entityManager.Transaction.rollback();
			}
		  }
	  }

	  private EntityManagerFactory EntityManagerFactory
	  {
		  get
		  {
			return entityManagerFactory;
		  }
	  }
	}

}