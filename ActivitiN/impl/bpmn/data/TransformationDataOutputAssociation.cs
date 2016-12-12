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
	/// A transformation based data output association
	/// 
	/// @author Esteban Robles Luna
	/// </summary>
	public class TransformationDataOutputAssociation : AbstractDataAssociation
	{

	  private const long serialVersionUID = 1L;

	  protected internal Expression transformation;

	  public TransformationDataOutputAssociation(string sourceRef, string targetRef, Expression transformation) : base(sourceRef, targetRef)
	  {
		this.transformation = transformation;
	  }

	  public override void evaluate(ActivityExecution execution)
	  {
		object value = this.transformation.getValue(execution);
		execution.setVariable(this.Target, value);
	  }
	}

}