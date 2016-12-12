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

namespace org.activiti.engine.history
{

	using GroupQuery = org.activiti.engine.identity.GroupQuery;
	using UserQuery = org.activiti.engine.identity.UserQuery;
	using IdentityLink = org.activiti.engine.task.IdentityLink;
	using IdentityLinkType = org.activiti.engine.task.IdentityLinkType;


	/// <summary>
	/// Historic counterpart of <seealso cref="IdentityLink"/> that represents the current state
	/// if any runtime link. Will be preserved when the runtime process instance or task 
	/// is finished.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public interface HistoricIdentityLink
	{

	  /// <summary>
	  /// Returns the type of link.
	  /// See <seealso cref="IdentityLinkType"/> for the native supported types by Activiti. 
	  /// </summary>
	  string Type {get;}

	  /// <summary>
	  /// If the identity link involves a user, then this will be a non-null id of a user.
	  /// That userId can be used to query for user information through the <seealso cref="UserQuery"/> API.
	  /// </summary>
	  string UserId {get;}

	  /// <summary>
	  /// If the identity link involves a group, then this will be a non-null id of a group.
	  /// That groupId can be used to query for user information through the <seealso cref="GroupQuery"/> API.
	  /// </summary>
	  string GroupId {get;}

	  /// <summary>
	  /// The id of the task associated with this identity link.
	  /// </summary>
	  string TaskId {get;}

	  /// <summary>
	  /// The id of the process instance associated with this identity link.
	  /// </summary>
	  string ProcessInstanceId {get;}
	}

}