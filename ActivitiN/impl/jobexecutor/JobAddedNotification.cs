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
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class JobAddedNotification : TransactionListener
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(JobAddedNotification));

	  protected internal JobExecutor jobExecutor;

	  public JobAddedNotification(JobExecutor jobExecutor)
	  {
		this.jobExecutor = jobExecutor;
	  }

	  public virtual void execute(CommandContext commandContext)
	  {
		log.debug("notifying job executor of new job");
		jobExecutor.jobWasAdded();
	  }
	}

}