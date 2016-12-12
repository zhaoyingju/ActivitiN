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
namespace org.activiti.engine.impl.bpmn.data
{

	using Expression = org.activiti.engine.@delegate.Expression;
	using ActivityExecution = org.activiti.engine.impl.pvm.@delegate.ActivityExecution;

	/// <summary>
	/// A data association (Input or Output) between a source and a target
	/// 
	/// @author Esteban Robles Luna
	/// </summary>
	[Serializable]
	public abstract class AbstractDataAssociation
	{

	  private const long serialVersionUID = 1L;

	  protected internal string source;

	  protected internal Expression sourceExpression;

	  protected internal string target;

	  protected internal AbstractDataAssociation(string source, string target)
	  {
		this.source = source;
		this.target = target;
	  }

	  protected internal AbstractDataAssociation(Expression sourceExpression, string target)
	  {
		this.sourceExpression = sourceExpression;
		this.target = target;
	  }

	  public abstract void evaluate(ActivityExecution execution);

	  public virtual string Source
	  {
		  get
		  {
			return source;
		  }
	  }

	  public virtual string Target
	  {
		  get
		  {
			return target;
		  }
	  }


	  public virtual Expression SourceExpression
	  {
		  get
		  {
			return sourceExpression;
		  }
	  }
	}

}