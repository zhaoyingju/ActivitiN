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


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class GenericManagerFactory : SessionFactory
	{

	  protected internal Type managerImplementation;

	  public GenericManagerFactory(Type managerImplementation)
	  {
		this.managerImplementation = managerImplementation;
	  }

	  public virtual Type SessionType
	  {
		  get
		  {
			return managerImplementation;
		  }
	  }

	  public virtual Session openSession()
	  {
		try
		{
		  return managerImplementation.newInstance();
		}
		catch (Exception e)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ActivitiException("couldn't instantiate " + managerImplementation.FullName + ": " + e.Message, e);
		}
	  }
	}

}