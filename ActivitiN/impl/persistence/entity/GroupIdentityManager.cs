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


	using Group = org.activiti.engine.identity.Group;
	using GroupQuery = org.activiti.engine.identity.GroupQuery;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public interface GroupIdentityManager
	{

	  Group createNewGroup(string groupId);

	  void insertGroup(Group group);

	  void updateGroup(Group updatedGroup);

	  void deleteGroup(string groupId);

	  GroupQuery createNewGroupQuery();

	  IList<Group> findGroupByQueryCriteria(GroupQueryImpl query, Page page);

	  long findGroupCountByQueryCriteria(GroupQueryImpl query);

	  IList<Group> findGroupsByUser(string userId);

	  IList<Group> findGroupsByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults);

	  long findGroupCountByNativeQuery(IDictionary<string, object> parameterMap);

	  bool isNewGroup(Group group);

	}

}