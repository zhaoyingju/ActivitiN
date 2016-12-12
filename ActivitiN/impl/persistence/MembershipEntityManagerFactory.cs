using System;

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
namespace org.activiti.engine.impl.persistence
{

	using Session = org.activiti.engine.impl.interceptor.Session;
	using SessionFactory = org.activiti.engine.impl.interceptor.SessionFactory;
	using MembershipEntityManager = org.activiti.engine.impl.persistence.entity.MembershipEntityManager;
	using MembershipIdentityManager = org.activiti.engine.impl.persistence.entity.MembershipIdentityManager;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class MembershipEntityManagerFactory : SessionFactory
	{

	  public virtual Type SessionType
	  {
		  get
		  {
			return typeof(MembershipIdentityManager);
		  }
	  }

	  public virtual Session openSession()
	  {
		return new MembershipEntityManager();
	  }

	}

}