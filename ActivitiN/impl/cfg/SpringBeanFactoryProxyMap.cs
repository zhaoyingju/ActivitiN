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

namespace org.activiti.engine.impl.cfg
{


	using BeanFactory = org.springframework.beans.factory.BeanFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class SpringBeanFactoryProxyMap : IDictionary<object, object>
	{

	  protected internal BeanFactory beanFactory;

	  public SpringBeanFactoryProxyMap(BeanFactory beanFactory)
	  {
		this.beanFactory = beanFactory;
	  }

	  public virtual object get(object key)
	  {
		if ((key == null) || (!key.GetType().IsSubclassOf(typeof(string))))
		{
		  return null;
		}
		return beanFactory.getBean((string) key);
	  }

	  public virtual bool ContainsKey(object key)
	  {
		if ((key == null) || (!key.GetType().IsSubclassOf(typeof(string))))
		{
		  return false;
		}
		return beanFactory.containsBean((string) key);
	  }

	  public virtual Set<object> keySet()
	  {
		throw new ActivitiException("unsupported operation on configuration beans");
	//    List<String> beanNames = Arrays.asList(beanFactory.getBeanDefinitionNames());
	//    return new HashSet<Object>(beanNames);
	  }

	  public virtual void Clear()
	  {
		throw new ActivitiException("can't clear configuration beans");
	  }

	  public virtual bool containsValue(object value)
	  {
		throw new ActivitiException("can't search values in configuration beans");
	  }

	  public virtual Set<KeyValuePair<object, object>> entrySet()
	  {
		throw new ActivitiException("unsupported operation on configuration beans");
	  }

	  public virtual bool Empty
	  {
		  get
		  {
			throw new ActivitiException("unsupported operation on configuration beans");
		  }
	  }

	  public virtual object put(object key, object value)
	  {
		throw new ActivitiException("unsupported operation on configuration beans");
	  }

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public void putAll(java.util.Map< ? extends Object, ? extends Object> m)
	  public virtual void putAll<T1>(IDictionary<T1> m) where T1 : Object where ? : Object
	  {
		throw new ActivitiException("unsupported operation on configuration beans");
	  }

	  public virtual object remove(object key)
	  {
		throw new ActivitiException("unsupported operation on configuration beans");
	  }

	  public virtual int Count
	  {
		  get
		  {
			throw new ActivitiException("unsupported operation on configuration beans");
		  }
	  }

	  public virtual ICollection<object> Values
	  {
		  get
		  {
			throw new ActivitiException("unsupported operation on configuration beans");
		  }
	  }
	}

}