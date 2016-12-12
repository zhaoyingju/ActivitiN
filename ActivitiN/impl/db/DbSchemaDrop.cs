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

namespace org.activiti.engine.impl.db
{

	using org.activiti.engine.impl.interceptor;
	using CommandConfig = org.activiti.engine.impl.interceptor.CommandConfig;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class DbSchemaDrop
	{

	  public static void Main(string[] args)
	  {
		ProcessEngineImpl processEngine = (ProcessEngineImpl) ProcessEngines.DefaultProcessEngine;
		CommandExecutor commandExecutor = processEngine.ProcessEngineConfiguration.CommandExecutor;
		CommandConfig config = (new CommandConfig()).transactionNotSupported();
		commandExecutor.execute(config, new CommandAnonymousInnerClassHelper());
	  }

	  private class CommandAnonymousInnerClassHelper : Command<object>
	  {
		  public CommandAnonymousInnerClassHelper()
		  {
		  }

		  public virtual object execute(CommandContext commandContext)
		  {
			commandContext.getSession(typeof(DbSqlSession)).dbSchemaDrop();
			return null;
		  }
	  }
	}

}