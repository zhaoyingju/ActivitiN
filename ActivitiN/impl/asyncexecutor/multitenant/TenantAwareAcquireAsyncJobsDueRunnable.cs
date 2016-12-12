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

	/// <summary>
	/// Extends the default <seealso cref="AcquireAsyncJobsDueRunnable"/> by setting the 'tenant' context before executing.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class TenantAwareAcquireAsyncJobsDueRunnable : AcquireAsyncJobsDueRunnable
	{

	  protected internal TenantInfoHolder tenantInfoHolder;
	  protected internal string tenantId;

	  public TenantAwareAcquireAsyncJobsDueRunnable(AsyncExecutor asyncExecutor, TenantInfoHolder tenantInfoHolder, string tenantId) : base(asyncExecutor)
	  {
		this.tenantInfoHolder = tenantInfoHolder;
		this.tenantId = tenantId;
	  }

	  protected internal virtual ExecutorPerTenantAsyncExecutor TenantAwareAsyncExecutor
	  {
		  get
		  {
			return (ExecutorPerTenantAsyncExecutor) asyncExecutor;
		  }
	  }

	  public override void run()
	  {
		  lock (this)
		  {
			tenantInfoHolder.CurrentTenantId = tenantId;
			base.run();
			tenantInfoHolder.clearCurrentTenantId();
		  }
	  }

	}

}