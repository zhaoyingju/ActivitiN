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

namespace org.activiti.engine.impl.persistence.entity
{


	using ActivitiEventType = org.activiti.engine.@delegate.@event.ActivitiEventType;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class MembershipEntityManager : AbstractManager, MembershipIdentityManager
	{

	  public virtual void createMembership(string userId, string groupId)
	  {
		MembershipEntity membershipEntity = new MembershipEntity();
		membershipEntity.UserId = userId;
		membershipEntity.GroupId = groupId;
		DbSqlSession.insert(membershipEntity);

		if (ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createMembershipEvent(ActivitiEventType.MEMBERSHIP_CREATED, groupId, userId));
		}
	  }

	  public virtual void deleteMembership(string userId, string groupId)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["userId"] = userId;
		parameters["groupId"] = groupId;
		DbSqlSession.delete("deleteMembership", parameters);

		if (ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
			ProcessEngineConfiguration.EventDispatcher.dispatchEvent(ActivitiEventBuilder.createMembershipEvent(ActivitiEventType.MEMBERSHIP_DELETED, groupId, userId));
		}
	  }


	}

}