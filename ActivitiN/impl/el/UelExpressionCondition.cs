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

namespace org.activiti.engine.impl.el
{

	using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;
	using Expression = org.activiti.engine.@delegate.Expression;
	using Context = org.activiti.engine.impl.context.Context;

	using JsonNode = com.fasterxml.jackson.databind.JsonNode;
	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;


	/// <summary>
	/// <seealso cref="Condition"/> that resolves an UEL expression at runtime.  
	/// 
	/// @author Joram Barrez
	/// @author Frederik Heremans
	/// </summary>
	[Serializable]
	public class UelExpressionCondition : Condition
	{

	  private const long serialVersionUID = 1L;

	  protected internal string initialConditionExpression;

	  public UelExpressionCondition(string conditionExpression)
	  {
		this.initialConditionExpression = conditionExpression;
	  }

	  public virtual bool evaluate(string sequenceFlowId, DelegateExecution execution)
	  {
		string conditionExpression = null;
		if (Context.ProcessEngineConfiguration.EnableProcessDefinitionInfoCache)
		{
		  ObjectNode elementProperties = Context.getBpmnOverrideElementProperties(sequenceFlowId, execution.ProcessDefinitionId);
		  conditionExpression = getActiveValue(initialConditionExpression, org.activiti.engine.DynamicBpmnConstants_Fields.SEQUENCE_FLOW_CONDITION, elementProperties);
		}
		else
		{
		  conditionExpression = initialConditionExpression;
		}

		Expression expression = Context.ProcessEngineConfiguration.ExpressionManager.createExpression(conditionExpression);
		object result = expression.getValue(execution);

		if (result == null)
		{
		  throw new ActivitiException("condition expression returns null");
		}
		if (!(result is bool?))
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ActivitiException("condition expression returns non-Boolean: " + result + " (" + result.GetType().FullName + ")");
		}
		return (bool?) result;
	  }

	  protected internal virtual string getActiveValue(string originalValue, string propertyName, ObjectNode elementProperties)
	  {
		string activeValue = originalValue;
		if (elementProperties != null)
		{
		  JsonNode overrideValueNode = elementProperties.get(propertyName);
		  if (overrideValueNode != null)
		  {
			if (overrideValueNode.Null)
			{
			  activeValue = null;
			}
			else
			{
			  activeValue = overrideValueNode.asText();
			}
		  }
		}
		return activeValue;
	  }

	}

}