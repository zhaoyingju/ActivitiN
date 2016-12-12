using System;
using System.Collections.Generic;

namespace org.activiti.engine.impl.interceptor
{


    using ActivitiEventDispatcher = org.activiti.engine.@delegate.@event.ActivitiEventDispatcher;
    using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
    using TransactionContext = org.activiti.engine.impl.cfg.TransactionContext;
    using Context = org.activiti.engine.impl.context.Context;
    using DbSqlSession = org.activiti.engine.impl.db.DbSqlSession;
    using HistoryManager = org.activiti.engine.impl.history.HistoryManager;
    using FailedJobCommandFactory = org.activiti.engine.impl.jobexecutor.FailedJobCommandFactory;
    using AttachmentEntityManager = org.activiti.engine.impl.persistence.entity.AttachmentEntityManager;
    using ByteArrayEntityManager = org.activiti.engine.impl.persistence.entity.ByteArrayEntityManager;
    using CommentEntityManager = org.activiti.engine.impl.persistence.entity.CommentEntityManager;
    using DeploymentEntityManager = org.activiti.engine.impl.persistence.entity.DeploymentEntityManager;
    using EventLogEntryEntityManager = org.activiti.engine.impl.persistence.entity.EventLogEntryEntityManager;
    using EventSubscriptionEntityManager = org.activiti.engine.impl.persistence.entity.EventSubscriptionEntityManager;
    using ExecutionEntityManager = org.activiti.engine.impl.persistence.entity.ExecutionEntityManager;
    using GroupIdentityManager = org.activiti.engine.impl.persistence.entity.GroupIdentityManager;
    using HistoricActivityInstanceEntityManager = org.activiti.engine.impl.persistence.entity.HistoricActivityInstanceEntityManager;
    using HistoricDetailEntityManager = org.activiti.engine.impl.persistence.entity.HistoricDetailEntityManager;
    using HistoricIdentityLinkEntityManager = org.activiti.engine.impl.persistence.entity.HistoricIdentityLinkEntityManager;
    using HistoricProcessInstanceEntityManager = org.activiti.engine.impl.persistence.entity.HistoricProcessInstanceEntityManager;
    using HistoricTaskInstanceEntityManager = org.activiti.engine.impl.persistence.entity.HistoricTaskInstanceEntityManager;
    using HistoricVariableInstanceEntityManager = org.activiti.engine.impl.persistence.entity.HistoricVariableInstanceEntityManager;
    using IdentityInfoEntityManager = org.activiti.engine.impl.persistence.entity.IdentityInfoEntityManager;
    using IdentityLinkEntityManager = org.activiti.engine.impl.persistence.entity.IdentityLinkEntityManager;
    using JobEntityManager = org.activiti.engine.impl.persistence.entity.JobEntityManager;
    using MembershipIdentityManager = org.activiti.engine.impl.persistence.entity.MembershipIdentityManager;
    using ModelEntityManager = org.activiti.engine.impl.persistence.entity.ModelEntityManager;
    using ProcessDefinitionEntityManager = org.activiti.engine.impl.persistence.entity.ProcessDefinitionEntityManager;
    using ProcessDefinitionInfoEntityManager = org.activiti.engine.impl.persistence.entity.ProcessDefinitionInfoEntityManager;
    using PropertyEntityManager = org.activiti.engine.impl.persistence.entity.PropertyEntityManager;
    using ResourceEntityManager = org.activiti.engine.impl.persistence.entity.ResourceEntityManager;
    using TableDataManager = org.activiti.engine.impl.persistence.entity.TableDataManager;
    using TaskEntityManager = org.activiti.engine.impl.persistence.entity.TaskEntityManager;
    using UserIdentityManager = org.activiti.engine.impl.persistence.entity.UserIdentityManager;
    using VariableInstanceEntityManager = org.activiti.engine.impl.persistence.entity.VariableInstanceEntityManager;
    using AtomicOperation = org.activiti.engine.impl.pvm.runtime.AtomicOperation;
    using InterpretableExecution = org.activiti.engine.impl.pvm.runtime.InterpretableExecution;
    using LogMDC = org.activiti.engine.logging.LogMDC;

    public class CommandContext<T>
    {
        protected internal Command<T> command;
        protected internal TransactionContext transactionContext;
        protected internal IDictionary<Type, SessionFactory> sessionFactories;
        protected internal IDictionary<Type, Session> sessions = new Dictionary<Type, Session>();
        protected internal Exception exception_Renamed = null;
        protected internal LinkedList<AtomicOperation> nextOperations = new LinkedList<AtomicOperation>();
        protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
        protected internal FailedJobCommandFactory failedJobCommandFactory;
        protected internal IList<CommandContextCloseListener> closeListeners;
        protected internal IDictionary<string, object> attributes; // General-purpose storing of anything during the lifetime of a command context


        public virtual void performOperation(AtomicOperation executionOperation, InterpretableExecution execution)
        {
            nextOperations.AddLast(executionOperation);
            if (nextOperations.Count == 1)
            {
                try
                {
                    Context.ExecutionContext = execution;
                    while (nextOperations.Count > 0)
                    {
                        AtomicOperation currentOperation = nextOperations.RemoveFirst();
                        //if (log.TraceEnabled)
                        //{
                        //    log.trace("AtomicOperation: {} on {}", currentOperation, this);
                        //}
                        if (execution.getReplacedBy() == null)
                        {
                            currentOperation.execute(execution);
                        }
                        else
                        {
                            currentOperation.execute(execution.getReplacedBy());
                        }
                    }
                }
                finally
                {
                    Context.removeExecutionContext();
                }
            }
        }

        public CommandContext(Command<T> command, ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            this.command = command;
            this.processEngineConfiguration = processEngineConfiguration;
            this.failedJobCommandFactory = processEngineConfiguration.FailedJobCommandFactory;
            sessionFactories = processEngineConfiguration.SessionFactories;
            this.transactionContext = processEngineConfiguration.getTransactionContextFactory().openTransactionContext(this);
        }

        public virtual void close()
        {
            // the intention of this method is that all resources are closed properly, even
            // if exceptions occur in close or flush methods of the sessions or the
            // transaction context.

            try
            {
                try
                {
                    try
                    {

                        if (exception_Renamed == null && closeListeners != null)
                        {
                            try
                            {
                                foreach (CommandContextCloseListener listener in closeListeners)
                                {
                                    listener.closing(this);
                                }
                            }
                            catch (Exception exception)
                            {
                                exception(exception);
                            }
                        }

                        if (exception_Renamed == null)
                        {
                            flushSessions();
                        }

                    }
                    catch (Exception exception)
                    {
                        exception(exception);
                    }
                    finally
                    {

                        try
                        {
                            if (exception == null)
                            {
                                transactionContext.commit();
                            }
                        }
                        catch (Exception exception)
                        {
                            exception(exception);
                        }

                        if (exception == null && closeListeners != null)
                        {
                            try
                            {
                                foreach (CommandContextCloseListener listener in closeListeners)
                                {
                                    listener.closed(this);
                                }
                            }
                            catch (Exception exception)
                            {
                                exception(exception);
                            }
                        }

                        if (exception != null)
                        {
                            if (exception is JobNotFoundException || exception is ActivitiTaskAlreadyClaimedException)
                            {
                                // reduce log level, because this may have been caused because of job deletion due to cancelActiviti="true"
                                log.info("Error while closing command context", exception);
                            }
                            else if (exception is ActivitiOptimisticLockingException)
                            {
                                // reduce log level, as normally we're not interested in logging this exception
                                log.debug("Optimistic locking exception : " + exception);
                            }
                            else
                            {
                                log.error("Error while closing command context", exception);
                            }

                            transactionContext.rollback();
                        }
                    }
                }
                catch (Exception exception)
                {
                    exception(exception);
                }
                finally
                {
                    closeSessions();

                }
            }
            catch (Exception exception)
            {
                exception(exception);
            }

            // rethrow the original exception if there was one
            if (exception != null)
            {
                if (exception is Exception)
                {
                    throw (Exception)exception;
                }
                else if (exception is Exception)
                {
                    throw (Exception)exception;
                }
                else
                {
                    throw new ActivitiException("exception while executing command " + command, exception);
                }
            }
        }

        public virtual void addCloseListener(CommandContextCloseListener commandContextCloseListener)
        {
            if (closeListeners == null)
            {
                closeListeners = new List<CommandContextCloseListener>(1);
            }
            closeListeners.Add(commandContextCloseListener);
        }

        public virtual IList<CommandContextCloseListener> CloseListeners
        {
            get
            {
                return closeListeners;
            }
        }

        protected internal virtual void flushSessions()
        {
            foreach (Session session in sessions.Values)
            {
                session.flush();
            }
        }

        protected internal virtual void closeSessions()
        {
            foreach (Session session in sessions.Values)
            {
                try
                {
                    session.close();
                }
                catch (Exception exception)
                {
                    exception(exception);
                }
            }
        }

        public virtual void exception(Exception exception)
        {
            if (this.exception_Renamed == null)
            {
                this.exception_Renamed = exception;
            }
            else
            {
                if (Context.ExecutionContextActive)
                {
                    LogMDC.putMDCExecution(Context.ExecutionContext.Execution);
                }
                log.error("masked exception in command context. for root cause, see below as it will be rethrown later.", exception);
                LogMDC.clear();
            }
        }

        public virtual void addAttribute(string key, object value)
        {
            if (attributes == null)
            {
                attributes = new Dictionary<string, object>(1);
            }
            attributes[key] = value;
        }

        public virtual object getAttribute(string key)
        {
            if (attributes != null)
            {
                return attributes[key];
            }
            return null;
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings({"unchecked"}) public <T> T getSession(Class sessionClass)
        public virtual T getSession<T>(Type sessionClass)
        {
            Session session = sessions[sessionClass];
            if (session == null)
            {
                SessionFactory sessionFactory = sessionFactories[sessionClass];
                if (sessionFactory == null)
                {
                    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                    throw new ActivitiException("no session factory configured for " + sessionClass.FullName);
                }
                session = sessionFactory.openSession();
                sessions[sessionClass] = session;
            }

            return (T)session;
        }

        public virtual DbSqlSession DbSqlSession
        {
            get
            {
                return getSession(typeof(DbSqlSession));
            }
        }

        public virtual DeploymentEntityManager DeploymentEntityManager
        {
            get
            {
                return getSession(typeof(DeploymentEntityManager));
            }
        }

        public virtual ResourceEntityManager ResourceEntityManager
        {
            get
            {
                return getSession(typeof(ResourceEntityManager));
            }
        }

        public virtual ByteArrayEntityManager ByteArrayEntityManager
        {
            get
            {
                return getSession(typeof(ByteArrayEntityManager));
            }
        }

        public virtual ProcessDefinitionEntityManager ProcessDefinitionEntityManager
        {
            get
            {
                return getSession(typeof(ProcessDefinitionEntityManager));
            }
        }

        public virtual ModelEntityManager ModelEntityManager
        {
            get
            {
                return getSession(typeof(ModelEntityManager));
            }
        }

        public virtual ProcessDefinitionInfoEntityManager ProcessDefinitionInfoEntityManager
        {
            get
            {
                return getSession(typeof(ProcessDefinitionInfoEntityManager));
            }
        }

        public virtual ExecutionEntityManager ExecutionEntityManager
        {
            get
            {
                return getSession(typeof(ExecutionEntityManager));
            }
        }

        public virtual TaskEntityManager TaskEntityManager
        {
            get
            {
                return getSession(typeof(TaskEntityManager));
            }
        }

        public virtual IdentityLinkEntityManager IdentityLinkEntityManager
        {
            get
            {
                return getSession(typeof(IdentityLinkEntityManager));
            }
        }

        public virtual VariableInstanceEntityManager VariableInstanceEntityManager
        {
            get
            {
                return getSession(typeof(VariableInstanceEntityManager));
            }
        }

        public virtual HistoricProcessInstanceEntityManager HistoricProcessInstanceEntityManager
        {
            get
            {
                return getSession(typeof(HistoricProcessInstanceEntityManager));
            }
        }

        public virtual HistoricDetailEntityManager HistoricDetailEntityManager
        {
            get
            {
                return getSession(typeof(HistoricDetailEntityManager));
            }
        }

        public virtual HistoricVariableInstanceEntityManager HistoricVariableInstanceEntityManager
        {
            get
            {
                return getSession(typeof(HistoricVariableInstanceEntityManager));
            }
        }

        public virtual HistoricActivityInstanceEntityManager HistoricActivityInstanceEntityManager
        {
            get
            {
                return getSession(typeof(HistoricActivityInstanceEntityManager));
            }
        }

        public virtual HistoricTaskInstanceEntityManager HistoricTaskInstanceEntityManager
        {
            get
            {
                return getSession(typeof(HistoricTaskInstanceEntityManager));
            }
        }

        public virtual HistoricIdentityLinkEntityManager HistoricIdentityLinkEntityManager
        {
            get
            {
                return getSession(typeof(HistoricIdentityLinkEntityManager));
            }
        }

        public virtual EventLogEntryEntityManager EventLogEntryEntityManager
        {
            get
            {
                return getSession(typeof(EventLogEntryEntityManager));
            }
        }

        public virtual JobEntityManager JobEntityManager
        {
            get
            {
                return getSession(typeof(JobEntityManager));
            }
        }

        public virtual UserIdentityManager UserIdentityManager
        {
            get
            {
                return getSession(typeof(UserIdentityManager));
            }
        }

        public virtual GroupIdentityManager GroupIdentityManager
        {
            get
            {
                return getSession(typeof(GroupIdentityManager));
            }
        }

        public virtual IdentityInfoEntityManager IdentityInfoEntityManager
        {
            get
            {
                return getSession(typeof(IdentityInfoEntityManager));
            }
        }

        public virtual MembershipIdentityManager MembershipIdentityManager
        {
            get
            {
                return getSession(typeof(MembershipIdentityManager));
            }
        }

        public virtual AttachmentEntityManager AttachmentEntityManager
        {
            get
            {
                return getSession(typeof(AttachmentEntityManager));
            }
        }

        public virtual TableDataManager TableDataManager
        {
            get
            {
                return getSession(typeof(TableDataManager));
            }
        }

        public virtual CommentEntityManager CommentEntityManager
        {
            get
            {
                return getSession(typeof(CommentEntityManager));
            }
        }

        public virtual PropertyEntityManager PropertyEntityManager
        {
            get
            {
                return getSession(typeof(PropertyEntityManager));
            }
        }

        public virtual EventSubscriptionEntityManager EventSubscriptionEntityManager
        {
            get
            {
                return getSession(typeof(EventSubscriptionEntityManager));
            }
        }

        public virtual IDictionary<Type, SessionFactory> SessionFactories
        {
            get
            {
                return sessionFactories;
            }
        }

        public virtual HistoryManager HistoryManager
        {
            get
            {
                return getSession(typeof(HistoryManager));
            }
        }

        // getters and setters //////////////////////////////////////////////////////

        public virtual TransactionContext TransactionContext
        {
            get
            {
                return transactionContext;
            }
        }
        //JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        //ORIGINAL LINE: public Command< ? > getCommand()
        public virtual Command<?> Command
        {
            get
            {
                return command;
            }
        }
        public virtual IDictionary<Type, Session> Sessions
        {
            get
            {
                return sessions;
            }
        }
        public virtual Exception Exception
        {
            get
            {
                return exception_Renamed;
            }
        }
        public virtual FailedJobCommandFactory FailedJobCommandFactory
        {
            get
            {
                return failedJobCommandFactory;
            }
        }
        public virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration
        {
            get
            {
                return processEngineConfiguration;
            }
        }
        public virtual ActivitiEventDispatcher EventDispatcher
        {
            get
            {
                return processEngineConfiguration.EventDispatcher;
            }
        }
    }
}