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
namespace org.activiti.engine.impl.scripting
{

	using DelegateExecution = org.activiti.engine.@delegate.DelegateExecution;
	using Context = org.activiti.engine.impl.context.Context;

	using JsonNode = com.fasterxml.jackson.databind.JsonNode;
	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class ScriptCondition : Condition
	{

	  private const long serialVersionUID = 1L;

	  private readonly string expression;
	  private readonly string language;

	  public ScriptCondition(string expression, string language)
	  {
		this.expression = expression;
		this.language = language;
	  }

	  public virtual bool evaluate(string sequenceFlowId, DelegateExecution execution)
	  {
		string conditionExpression = null;
		if (Context.ProcessEngineConfiguration.EnableProcessDefinitionInfoCache)
		{
		  ObjectNode elementProperties = Context.getBpmnOverrideElementProperties(sequenceFlowId, execution.ProcessDefinitionId);
		  conditionExpression = getActiveValue(expression, org.activiti.engine.DynamicBpmnConstants_Fields.SEQUENCE_FLOW_CONDITION, elementProperties);
		}
		else
		{
		  conditionExpression = expression;
		}

		ScriptingEngines scriptingEngines = Context.ProcessEngineConfiguration.ScriptingEngines;

		object result = scriptingEngines.evaluate(conditionExpression, language, execution);
		if (result == null)
		{
		  throw new ActivitiException("condition script returns null: " + expression);
		}
		if (!(result is bool?))
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ActivitiException("condition script returns non-Boolean: " + result + " (" + result.GetType().FullName + ")");
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