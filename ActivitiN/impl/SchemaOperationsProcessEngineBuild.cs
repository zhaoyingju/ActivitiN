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
namespace org.activiti.engine.impl
{

	using DbSqlSession = org.activiti.engine.impl.db.DbSqlSession;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public sealed class SchemaOperationsProcessEngineBuild : Command<object>
	{

	  public object execute(CommandContext commandContext)
	  {
		commandContext.getSession(typeof(DbSqlSession)).performSchemaOperationsProcessEngineBuild();
		return null;
	  }
	}
}