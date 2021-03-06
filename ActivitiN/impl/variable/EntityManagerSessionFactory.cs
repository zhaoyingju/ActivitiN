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

namespace org.activiti.engine.impl.variable
{

	using Session = org.activiti.engine.impl.interceptor.Session;
	using SessionFactory = org.activiti.engine.impl.interceptor.SessionFactory;

	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	public class EntityManagerSessionFactory : SessionFactory
	{

	  protected internal EntityManagerFactory entityManagerFactory;
	  protected internal bool handleTransactions;
	  protected internal bool closeEntityManager;

	  public EntityManagerSessionFactory(object entityManagerFactory, bool handleTransactions, bool closeEntityManager)
	  {
		if (entityManagerFactory == null)
		{
		  throw new ActivitiIllegalArgumentException("entityManagerFactory is null");
		}
		if (!(entityManagerFactory is EntityManagerFactory))
		{
		  throw new ActivitiIllegalArgumentException("EntityManagerFactory must implement 'javax.persistence.EntityManagerFactory'");
		}

		this.entityManagerFactory = (EntityManagerFactory) entityManagerFactory;
		this.handleTransactions = handleTransactions;
		this.closeEntityManager = closeEntityManager;
	  }

	  public virtual Type SessionType
	  {
		  get
		  {
			return typeof(EntityManagerSession);
		  }
	  }

	  public virtual Session openSession()
	  {
		return new EntityManagerSessionImpl(entityManagerFactory, handleTransactions, closeEntityManager);
	  }

	  public virtual EntityManagerFactory EntityManagerFactory
	  {
		  get
		  {
			return entityManagerFactory;
		  }
	  }
	}

}