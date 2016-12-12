namespace org.activiti.engine.impl.context
{


    using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
    using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
    using JobExecutorContext = org.activiti.engine.impl.jobexecutor.JobExecutorContext;
    using ProcessDefinitionInfoCacheObject = org.activiti.engine.impl.persistence.deploy.ProcessDefinitionInfoCacheObject;
    using InterpretableExecution = org.activiti.engine.impl.pvm.runtime.InterpretableExecution;

    using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;
    using System.Collections.Generic;
    using System.Threading;

    public class Context
    {

        Queue<string> numbers = new Queue<string>();

        protected internal static ThreadLocal<Stack<CommandContext>> commandContextThreadLocal = new ThreadLocal<Stack<CommandContext>>();
        protected internal static ThreadLocal<Stack<ProcessEngineConfigurationImpl>> processEngineConfigurationStackThreadLocal = new ThreadLocal<Stack<ProcessEngineConfigurationImpl>>();
        protected internal static ThreadLocal<Stack<ExecutionContext>> executionContextStackThreadLocal = new ThreadLocal<Stack<ExecutionContext>>();
        protected internal static ThreadLocal<JobExecutorContext> jobExecutorContextThreadLocal = new ThreadLocal<JobExecutorContext>();
        protected internal static ThreadLocal<IDictionary<string, ObjectNode>> bpmnOverrideContextThreadLocal = new ThreadLocal<IDictionary<string, ObjectNode>>();

        public static CommandContext CommandContext
        {
            get
            {
                Stack<CommandContext> stack = getStack(commandContextThreadLocal);
                if (stack.Count == 0)
                {
                    return null;
                }
                return stack.Peek();
            }
            set
            {
                getStack(commandContextThreadLocal).Push(value);
            }
        }

        public static void removeCommandContext()
        {
            getStack(commandContextThreadLocal).Pop();
        }

        public static ProcessEngineConfigurationImpl ProcessEngineConfiguration
        {
            get
            {
                Stack<ProcessEngineConfigurationImpl> stack = getStack(processEngineConfigurationStackThreadLocal);
                if (stack.Count == 0)
                {
                    return null;
                }
                return stack.Peek();
            }
            set
            {
                getStack(processEngineConfigurationStackThreadLocal).Push(value);
            }
        }

        public static void removeProcessEngineConfiguration()
        {
            getStack(processEngineConfigurationStackThreadLocal).Pop();
        }

        public static ExecutionContext getExecutionContext()
        {
            return getStack(executionContextStackThreadLocal).Peek();
        }

        public static bool ExecutionContextActive
        {
            get
            {
                Stack<ExecutionContext> stack = executionContextStackThreadLocal.get();
                return stack != null && stack.Count > 0;
            }
        }

        public static void setExecutionContext(InterpretableExecution execution)
        {
            getStack(executionContextStackThreadLocal).Push(new ExecutionContext(execution));
        }

        public static void removeExecutionContext()
        {
            getStack(executionContextStackThreadLocal).Pop();
        }

        protected internal static Stack<T> getStack<T>(ThreadLocal<Stack<T>> threadLocal)
        {
            Stack<T> stack = threadLocal.get();
            if (stack == null)
            {
                stack = new Stack<T>();
                threadLocal.set(stack);
            }
            return stack;
        }

        public static JobExecutorContext JobExecutorContext
        {
            get
            {
                return jobExecutorContextThreadLocal.get();
            }
            set
            {
                jobExecutorContextThreadLocal.set(value);
            }
        }


        public static void removeJobExecutorContext()
        {
            jobExecutorContextThreadLocal.remove();
        }

        public static ObjectNode getBpmnOverrideElementProperties(string id, string processDefinitionId)
        {
            ObjectNode definitionInfoNode = getProcessDefinitionInfoNode(processDefinitionId);
            ObjectNode elementProperties = null;
            if (definitionInfoNode != null)
            {
                elementProperties = ProcessEngineConfiguration.DynamicBpmnService.getBpmnElementProperties(id, definitionInfoNode);
            }
            return elementProperties;
        }

        public static ObjectNode getLocalizationElementProperties(string language, string id, string processDefinitionId, bool useFallback)
        {
            ObjectNode definitionInfoNode = getProcessDefinitionInfoNode(processDefinitionId);
            ObjectNode localizationProperties = null;
            if (definitionInfoNode != null)
            {
                if (useFallback == false)
                {
                    localizationProperties = ProcessEngineConfiguration.DynamicBpmnService.getLocalizationElementProperties(language, id, definitionInfoNode);

                }
                else
                {
                    HashSet<Locale> candidateLocales = new LinkedHashSet<Locale>();
                    candidateLocales.addAll(resourceBundleControl.getCandidateLocales(id, new Locale(language)));
                    candidateLocales.addAll(resourceBundleControl.getCandidateLocales(id, Locale.Default));
                    foreach (Locale locale in candidateLocales)
                    {
                        localizationProperties = ProcessEngineConfiguration.DynamicBpmnService.getLocalizationElementProperties(locale.Language, id, definitionInfoNode);

                        if (localizationProperties != null)
                        {
                            break;
                        }
                    }
                }
            }
            return localizationProperties;
        }

        public static void removeBpmnOverrideContext()
        {
            bpmnOverrideContextThreadLocal.remove();
        }

        protected internal static ObjectNode getProcessDefinitionInfoNode(string processDefinitionId)
        {
            IDictionary<string, ObjectNode> bpmnOverrideMap = BpmnOverrideContext;
            if (bpmnOverrideMap.ContainsKey(processDefinitionId) == false)
            {
                ProcessDefinitionInfoCacheObject cacheObject = ProcessEngineConfiguration.DeploymentManager.ProcessDefinitionInfoCache.get(processDefinitionId);

                addBpmnOverrideElement(processDefinitionId, cacheObject.InfoNode);
            }

            return BpmnOverrideContext[processDefinitionId];
        }

        protected internal static IDictionary<string, ObjectNode> BpmnOverrideContext
        {
            get
            {
                IDictionary<string, ObjectNode> bpmnOverrideMap = bpmnOverrideContextThreadLocal.get();
                if (bpmnOverrideMap == null)
                {
                    bpmnOverrideMap = new Dictionary<string, ObjectNode>();
                }
                return bpmnOverrideMap;
            }
        }

        protected internal static void addBpmnOverrideElement(string id, ObjectNode infoNode)
        {
            IDictionary<string, ObjectNode> bpmnOverrideMap = bpmnOverrideContextThreadLocal.get();
            if (bpmnOverrideMap == null)
            {
                bpmnOverrideMap = new Dictionary<string, ObjectNode>();
                bpmnOverrideContextThreadLocal.set(bpmnOverrideMap);
            }
            bpmnOverrideMap[id] = infoNode;
        }
    }
}