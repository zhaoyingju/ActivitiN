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
namespace org.activiti.engine.impl.bpmn.behavior
{


	using BusinessRuleTaskDelegate = org.activiti.engine.@delegate.BusinessRuleTaskDelegate;
	using Expression = org.activiti.engine.@delegate.Expression;
	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using RulesAgendaFilter = org.activiti.engine.impl.rules.RulesAgendaFilter;
	using RulesHelper = org.activiti.engine.impl.rules.RulesHelper;
	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;
	using KnowledgeBase = org.drools.KnowledgeBase;
	using StatefulKnowledgeSession = org.drools.runtime.StatefulKnowledgeSession;


	/// <summary>
	/// activity implementation of the BPMN 2.0 business rule task.
	/// 
	/// @author Tijs Rademakers
	/// </summary>
	[Serializable]
	public class BusinessRuleTaskActivityBehavior : TaskActivityBehavior, BusinessRuleTaskDelegate
	{

	  private const long serialVersionUID = 1L;
	  protected internal Set<Expression> variablesInputExpressions = new HashSet<Expression>();
	  protected internal Set<Expression> rulesExpressions = new HashSet<Expression>();
	  protected internal bool exclude = false;
	  protected internal string resultVariable;

	  public BusinessRuleTaskActivityBehavior()
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.activiti.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) execution.EngineServices.ProcessEngineConfiguration;
		ProcessDefinition processDefinition = processEngineConfiguration.DeploymentManager.findDeployedProcessDefinitionById(execution.ProcessDefinitionId);
		string deploymentId = processDefinition.DeploymentId;

		KnowledgeBase knowledgeBase = RulesHelper.findKnowledgeBaseByDeploymentId(deploymentId);
		StatefulKnowledgeSession ksession = knowledgeBase.newStatefulKnowledgeSession();

		if (variablesInputExpressions != null)
		{
		  IEnumerator<Expression> itVariable = variablesInputExpressions.GetEnumerator();
		  while (itVariable.MoveNext())
		  {
			Expression variable = itVariable.Current;
			ksession.insert(variable.getValue(execution));
		  }
		}

		if (!rulesExpressions.Empty)
		{
		  RulesAgendaFilter filter = new RulesAgendaFilter();
		  IEnumerator<Expression> itRuleNames = rulesExpressions.GetEnumerator();
		  while (itRuleNames.MoveNext())
		  {
			Expression ruleName = itRuleNames.Current;
			filter.addSuffic(ruleName.getValue(execution).ToString());
		  }
		  filter.Accept = !exclude;
		  ksession.fireAllRules(filter);

		}
		else
		{
		  ksession.fireAllRules();
		}

		ICollection<object> ruleOutputObjects = ksession.Objects;
		if (ruleOutputObjects != null && ruleOutputObjects.Count > 0)
		{
		  ICollection<object> outputVariables = new List<object>();
		  foreach (object @object in ruleOutputObjects)
		  {
			outputVariables.Add(@object);
		  }
		  execution.setVariable(resultVariable, outputVariables);
		}
		ksession.dispose();
		leave(execution);
	  }

	  public virtual void addRuleVariableInputIdExpression(Expression inputId)
	  {
		this.variablesInputExpressions.add(inputId);
	  }

	  public virtual void addRuleIdExpression(Expression inputId)
	  {
		this.rulesExpressions.add(inputId);
	  }

	  public virtual bool Exclude
	  {
		  set
		  {
			this.exclude = value;
		  }
	  }

	  public virtual string ResultVariable
	  {
		  set
		  {
			this.resultVariable = value;
		  }
	  }

	}

}