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
namespace org.activiti.engine.impl.bpmn.data
{


	using Expression = org.activiti.engine.@delegate.Expression;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;

	/// <summary>
	/// A simple data input association between a source and a target with assignments
	/// 
	/// @author Esteban Robles Luna
	/// </summary>
	public class SimpleDataInputAssociation : AbstractDataAssociation
	{

	  private const long serialVersionUID = 1L;

	  protected internal IList<Assignment> assignments = new List<Assignment>();

	  public SimpleDataInputAssociation(Expression sourceExpression, string target) : base(sourceExpression, target)
	  {
	  }

	  public SimpleDataInputAssociation(string source, string target) : base(source, target)
	  {
	  }

	  public virtual void addAssignment(Assignment assignment)
	  {
		this.assignments.Add(assignment);
	  }

	  public override void evaluate(ActivityExecution execution)
	  {
		foreach (Assignment assignment in this.assignments)
		{
		  assignment.evaluate(execution);
		}
	  }
	}

}