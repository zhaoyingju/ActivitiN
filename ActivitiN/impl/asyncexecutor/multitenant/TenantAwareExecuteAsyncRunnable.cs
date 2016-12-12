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

namespace org.activiti.engine.impl.asyncexecutor.multitenant
{

	using TenantInfoHolder = org.activiti.engine.impl.cfg.multitenant.TenantInfoHolder;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using JobEntity = org.activiti.engine.impl.persistence.entity.JobEntity;

	/// <summary>
	/// Extends the default <seealso cref="ExecuteAsyncRunnable"/> by setting the 'tenant' context before executing.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class TenantAwareExecuteAsyncRunnable : ExecuteAsyncRunnable
	{

	  protected internal TenantInfoHolder tenantInfoHolder;
	  protected internal string tenantId;

	  public TenantAwareExecuteAsyncRunnable(JobEntity job, CommandExecutor commandExecutor, TenantInfoHolder tenantInfoHolder, string tenantId) : base(job, commandExecutor)
	  {
		this.tenantInfoHolder = tenantInfoHolder;
		this.tenantId = tenantId;
	  }

	  public override void run()
	  {
		tenantInfoHolder.CurrentTenantId = tenantId;
		base.run();
		tenantInfoHolder.clearCurrentTenantId();
	  }

	}

}