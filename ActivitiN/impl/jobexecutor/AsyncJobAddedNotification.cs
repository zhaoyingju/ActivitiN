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


	using AsyncExecutor = org.activiti.engine.impl.asyncexecutor.AsyncExecutor;
	using TransactionListener = org.activiti.engine.impl.cfg.TransactionListener;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	public class AsyncJobAddedNotification : TransactionListener
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(AsyncJobAddedNotification));

	  protected internal JobEntity job;
	  protected internal AsyncExecutor asyncExecutor;

	  public AsyncJobAddedNotification(JobEntity job, AsyncExecutor asyncExecutor)
	  {
		this.job = job;
		this.asyncExecutor = asyncExecutor;
	  }

	  public virtual void execute(CommandContext commandContext)
	  {
		log.debug("notifying job executor of new job");
		asyncExecutor.executeAsyncJob(job);
	  }
	}

}