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

	using ELContext = org.activiti.engine.impl.javax.el.ELContext;
	using ELResolver = org.activiti.engine.impl.javax.el.ELResolver;
	using ExpressionFactory = org.activiti.engine.impl.javax.el.ExpressionFactory;
	using FunctionMapper = org.activiti.engine.impl.javax.el.FunctionMapper;
	using VariableMapper = org.activiti.engine.impl.javax.el.VariableMapper;


	/// <summary>
	/// Simple implementation of the <seealso cref="ELContext"/> used during parsings.
	/// 
	/// Currently this implementation does nothing, but a non-null implementation
	/// of the <seealso cref="ELContext"/> interface is required by the <seealso cref="ExpressionFactory"/>
	/// when create value- and methodexpressions.
	/// </summary>
	/// <seealso cref= ExpressionManager#createExpression(String) </seealso>
	/// <seealso cref= ExpressionManager#createMethodExpression(String)
	/// 
	/// @author Joram Barrez </seealso>
	public class ParsingElContext : ELContext
	{

	  public override ELResolver ELResolver
	  {
		  get
		  {
			return null;
		  }
	  }

	  public override FunctionMapper FunctionMapper
	  {
		  get
		  {
			return null;
		  }
	  }

	  public override VariableMapper VariableMapper
	  {
		  get
		  {
			return null;
		  }
	  }

	}

}