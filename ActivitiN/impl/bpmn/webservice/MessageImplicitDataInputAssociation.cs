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
namespace org.activiti.engine.impl.bpmn.webservice
{

	using WebServiceActivityBehavior = org.activiti.engine.impl.bpmn.behavior.WebServiceActivityBehavior;
	using AbstractDataAssociation = org.activiti.engine.impl.bpmn.data.AbstractDataAssociation;
	using FieldBaseStructureInstance = org.activiti.engine.impl.bpmn.data.FieldBaseStructureInstance;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;
	using StringUtils = org.apache.commons.lang3.StringUtils;

	/// <summary>
	/// An implicit data input association between a source and a target.
	/// source is a variable in the current execution context and target is a property in the message
	/// 
	/// @author Esteban Robles Luna
	/// </summary>
	public class MessageImplicitDataInputAssociation : AbstractDataAssociation
	{

	  private const long serialVersionUID = 1L;

	  public MessageImplicitDataInputAssociation(string source, string target) : base(source, target)
	  {
	  }

	  public override void evaluate(ActivityExecution execution)
	  {
		if (StringUtils.isNotEmpty(this.source))
		{
		  object value = execution.getVariable(this.source);
		  MessageInstance message = (MessageInstance) execution.getVariable(WebServiceActivityBehavior.CURRENT_MESSAGE);
		  if (message.StructureInstance is FieldBaseStructureInstance)
		  {
			FieldBaseStructureInstance structure = (FieldBaseStructureInstance) message.StructureInstance;
			structure.setFieldValue(this.target, value);
		  }
		}
	  }
	}

}