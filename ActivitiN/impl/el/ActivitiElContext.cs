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
	using FunctionMapper = org.activiti.engine.impl.javax.el.FunctionMapper;
	using VariableMapper = org.activiti.engine.impl.javax.el.VariableMapper;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class ActivitiElContext : ELContext
	{

	  protected internal ELResolver elResolver;

	  public ActivitiElContext(ELResolver elResolver)
	  {
		this.elResolver = elResolver;
	  }

	  public override ELResolver ELResolver
	  {
		  get
		  {
			return elResolver;
		  }
	  }

	  public override FunctionMapper FunctionMapper
	  {
		  get
		  {
			return new ActivitiFunctionMapper();
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