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



	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class AcquiredJobs
	{

		protected internal IList<IList<string>> acquiredJobBatches = new List<IList<string>>();
		protected internal Set<string> acquiredJobs = new HashSet<string>();

		public virtual IList<IList<string>> JobIdBatches
		{
			get
			{
			return acquiredJobBatches;
			}
		}

		public virtual void addJobIdBatch(IList<string> jobIds)
		{
		acquiredJobBatches.Add(jobIds);
		acquiredJobs.addAll(jobIds);
		}

	  public virtual bool contains(string jobId)
	  {
		return acquiredJobs.contains(jobId);
	  }

	  public virtual int size()
	  {
		return acquiredJobs.size();
	  }


	}

}