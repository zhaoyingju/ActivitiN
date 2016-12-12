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
namespace org.activiti.engine.impl.form
{


	using TaskEntity = org.activiti.engine.impl.persistence.entity.TaskEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class FormData : IDictionary<string, object>
	{

	  internal TaskEntity task;

	  public FormData(TaskEntity task)
	  {
		this.task = task;
	  }

	  public virtual void Clear()
	  {
	  }

	  public virtual bool ContainsKey(object key)
	  {
		return false;
	  }

	  public virtual bool containsValue(object value)
	  {
		return false;
	  }

	  public virtual Set<KeyValuePair<string, object>> entrySet()
	  {
		return null;
	  }

	  public virtual object get(object key)
	  {
		return null;
	  }

	  public virtual bool Empty
	  {
		  get
		  {
			return false;
		  }
	  }

	  public virtual Set<string> keySet()
	  {
		return null;
	  }

	  public virtual object put(string key, object value)
	  {
		return null;
	  }

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public void putAll(java.util.Map< ? extends String, ? extends Object> m)
	  public virtual void putAll<T1>(IDictionary<T1> m) where T1 : String where ? : Object
	  {
	  }

	  public virtual object remove(object key)
	  {
		return null;
	  }

	  public virtual int Count
	  {
		  get
		  {
			return 0;
		  }
	  }

	  public virtual ICollection<object> Values
	  {
		  get
		  {
			return null;
		  }
	  }

	}

}