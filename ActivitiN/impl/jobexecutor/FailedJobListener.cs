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

namespace org.activiti.engine.impl.jobexecutor
{

	using TransactionListener = org.activiti.engine.impl.cfg.TransactionListener;
	using org.activiti.engine.impl.interceptor;
	using CommandConfig = org.activiti.engine.impl.interceptor.CommandConfig;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Frederik Heremans
	/// @author Saeid Mirzaei
	/// </summary>
	public class FailedJobListener : TransactionListener
	{
	  private static readonly Logger log = LoggerFactory.getLogger(typeof(FailedJobListener));

	  protected internal CommandExecutor commandExecutor;
	  protected internal string jobId;
	  protected internal Exception exception;

	  public FailedJobListener(CommandExecutor commandExecutor, string jobId)
	  {
		this.commandExecutor = commandExecutor;
		this.jobId = jobId;
	  }

	  public virtual void execute(CommandContext commandContext)
	  {
		CommandConfig commandConfig = commandExecutor.DefaultConfig.transactionRequiresNew();
		  FailedJobCommandFactory failedJobCommandFactory = commandContext.FailedJobCommandFactory;
		  Command<object> cmd = failedJobCommandFactory.getCommand(jobId, exception);

		  log.trace("Using FailedJobCommandFactory '" + failedJobCommandFactory.GetType() + "' and command of type '" + cmd.GetType() + "'");
		  commandExecutor.execute(commandConfig, cmd);
	  }

	  public virtual Exception Exception
	  {
		  set
		  {
			this.exception = value;
		  }
	  }

	}

}