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
namespace org.activiti.engine.impl.form
{

	using ValueExpression = org.activiti.engine.impl.javax.el.ValueExpression;
	using VariableMapper = org.activiti.engine.impl.javax.el.VariableMapper;
	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class TaskVariableMapper : VariableMapper
	{

	  public TaskVariableMapper(TaskEntity task)
	  {
	  }

	  public override ValueExpression resolveVariable(string variableName)
	  {
		return null;
	  }

	  public override ValueExpression setVariable(string variableName, ValueExpression arg1)
	  {
		return null;
	  }

	}

}