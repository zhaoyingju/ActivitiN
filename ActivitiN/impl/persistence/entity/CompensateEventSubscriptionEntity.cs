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

namespace org.activiti.engine.impl.persistence.entity
{

	using CompensationEventHandler = org.activiti.engine.impl.@event.CompensationEventHandler;



	/// <summary>
	/// @author Daniel Meyer
	/// @author Joram Barrez
	/// </summary>
	public class CompensateEventSubscriptionEntity : EventSubscriptionEntity
	{

	  private const long serialVersionUID = 1L;

	  private CompensateEventSubscriptionEntity()
	  {
	  }

	  private CompensateEventSubscriptionEntity(ExecutionEntity executionEntity) : base(executionEntity)
	  {
		eventType = CompensationEventHandler.EVENT_HANDLER_TYPE;
	  }

	  public static CompensateEventSubscriptionEntity createAndInsert(ExecutionEntity executionEntity)
	  {
		CompensateEventSubscriptionEntity eventSubscription = new CompensateEventSubscriptionEntity(executionEntity);
		if (executionEntity.TenantId != null)
		{
			eventSubscription.TenantId = executionEntity.TenantId;
		}
		eventSubscription.insert();
		return eventSubscription;
	  }

	  // custom processing behavior //////////////////////////////////////////////////////////////////////////////  

	  protected internal override void processEventSync(object payload)
	  {
		delete();
		base.processEventSync(payload);
	  }

	  public virtual CompensateEventSubscriptionEntity moveUnder(ExecutionEntity newExecution)
	  {

		delete();

		CompensateEventSubscriptionEntity newSubscription = createAndInsert(newExecution);
		newSubscription.Activity = Activity;
		newSubscription.Configuration = configuration;
		// use the original date
		newSubscription.Created = created;

		return newSubscription;
	  }

	}

}