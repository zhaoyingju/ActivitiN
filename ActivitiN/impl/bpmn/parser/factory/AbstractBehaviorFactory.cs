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
namespace org.activiti.engine.impl.bpmn.parser.factory
{


	using FieldExtension = org.activiti.bpmn.model.FieldExtension;
	using Expression = org.activiti.engine.@delegate.Expression;
	using ExpressionManager = org.activiti.engine.impl.el.ExpressionManager;
	using FixedValue = org.activiti.engine.impl.el.FixedValue;
	using StringUtils = org.apache.commons.lang3.StringUtils;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public abstract class AbstractBehaviorFactory
	{

	  protected internal ExpressionManager expressionManager;

	  public virtual IList<FieldDeclaration> createFieldDeclarations(IList<FieldExtension> fieldList)
	  {
		IList<FieldDeclaration> fieldDeclarations = new List<FieldDeclaration>();

		foreach (FieldExtension fieldExtension in fieldList)
		{
		  FieldDeclaration fieldDeclaration = null;
		  if (StringUtils.isNotEmpty(fieldExtension.Expression))
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			fieldDeclaration = new FieldDeclaration(fieldExtension.FieldName, typeof(Expression).FullName, expressionManager.createExpression(fieldExtension.Expression));
		  }
		  else
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			fieldDeclaration = new FieldDeclaration(fieldExtension.FieldName, typeof(Expression).FullName, new FixedValue(fieldExtension.StringValue));
		  }

		  fieldDeclarations.Add(fieldDeclaration);
		}
		return fieldDeclarations;
	  }


	  public virtual ExpressionManager ExpressionManager
	  {
		  get
		  {
			return expressionManager;
		  }
		  set
		  {
			this.expressionManager = value;
		  }
	  }


	}

}