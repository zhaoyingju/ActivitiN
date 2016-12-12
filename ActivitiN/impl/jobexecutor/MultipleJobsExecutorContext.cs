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
namespace org.activiti.engine.impl.jobexecutor
{


	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;

	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class MultipleJobsExecutorContext : JobExecutorContext
	{

	  protected internal IList<string> currentProcessorJobQueue = new LinkedList<string>();
	  protected internal JobEntity currentJob;

	  public virtual IList<string> CurrentProcessorJobQueue
	  {
		  get
		  {
			return currentProcessorJobQueue;
		  }
	  }

	  public virtual bool ExecutingExclusiveJob
	  {
		  get
		  {
			return currentJob == null ? false : currentJob.Exclusive;
		  }
	  }

	  public virtual JobEntity CurrentJob
	  {
		  set
		  {
			this.currentJob = value;
		  }
		  get
		  {
			return currentJob;
		  }
	  }

	}

}