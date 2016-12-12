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

	using Context = org.activiti.engine.impl.context.Context;


	/// <summary>
	/// Variable type capable of storing reference to JPA-entities. Only JPA-Entities which
	/// are configured by annotations are supported. Use of compound primary keys is not supported.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class JPAEntityVariableType : VariableType, CacheableVariable
	{

	  public const string TYPE_NAME = "jpa-entity";

	  private JPAEntityMappings mappings;

	  private bool forceCacheable = false;

	  public JPAEntityVariableType()
	  {
		mappings = new JPAEntityMappings();
	  }

	  public virtual string TypeName
	  {
		  get
		  {
			return TYPE_NAME;
		  }
	  }

	  public virtual bool Cachable
	  {
		  get
		  {
			return forceCacheable;
		  }
	  }

	  public virtual bool isAbleToStore(object value)
	  {
		if (value == null)
		{
		  return true;
		}
		return mappings.isJPAEntity(value);
	  }

	  public virtual void setValue(object value, ValueFields valueFields)
	  {
		EntityManagerSession entityManagerSession = Context.CommandContext.getSession(typeof(EntityManagerSession));
		if (entityManagerSession == null)
		{
		  throw new ActivitiException("Cannot set JPA variable: " + typeof(EntityManagerSession) + " not configured");
		}
		else
		{
		  // Before we set the value we must flush all pending changes from the entitymanager
		  // If we don't do this, in some cases the primary key will not yet be set in the object
		  // which will cause exceptions down the road.
		  entityManagerSession.flush();
		}

		if (value != null)
		{
		  string className = mappings.getJPAClassString(value);
		  string idString = mappings.getJPAIdString(value);
		  valueFields.TextValue = className;
		  valueFields.TextValue2 = idString;
		}
		else
		{
		  valueFields.TextValue = null;
		  valueFields.TextValue2 = null;
		}
	  }

	  public virtual object getValue(ValueFields valueFields)
	  {
		if (valueFields.TextValue != null && valueFields.TextValue2 != null)
		{
		  return mappings.getJPAEntity(valueFields.TextValue, valueFields.TextValue2);
		}
		return null;
	  }

	  /// <summary>
	  /// Force the value to be cacheable.
	  /// </summary>
	  public virtual bool ForceCacheable
	  {
		  set
		  {
			this.forceCacheable = value;
		  }
	  }


	}

}