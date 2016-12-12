using System;
using System.Collections.Generic;
using System.Text;

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
	using ActivitiVariableEvent = org.activiti.engine.@delegate.@event.ActivitiVariableEvent;
	using ActivitiEventBuilder = org.activiti.engine.@delegate.@event.impl.ActivitiEventBuilder;
	using Context = org.activiti.engine.impl.context.Context;
	using BulkDeleteable = org.activiti.engine.impl.db.BulkDeleteable;
	using VariableType = org.activiti.engine.impl.variable.VariableType;
	using StringUtils = org.apache.commons.lang3.StringUtils;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Marcus Klimstra (CGI)
	/// </summary>
	[Serializable]
	public class VariableInstanceEntity : VariableInstance, BulkDeleteable
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal int revision;

	  protected internal string name;
	  protected internal string localizedName;
	  protected internal string localizedDescription;
	  protected internal VariableType type;
	  protected internal string typeName;

	  protected internal string processInstanceId;
	  protected internal string executionId;
	  protected internal string taskId;

	  protected internal long? longValue;
	  protected internal double? doubleValue;
	  protected internal string textValue;
	  protected internal string textValue2;
	  protected internal readonly ByteArrayRef byteArrayRef = new ByteArrayRef();

	  protected internal object cachedValue;
	  protected internal bool forcedUpdate;
	  protected internal bool deleted = false;

	  // Default constructor for SQL mapping
	  protected internal VariableInstanceEntity()
	  {
	  }

	  public static VariableInstanceEntity createAndInsert(string name, VariableType type, object value)
	  {
		VariableInstanceEntity variableInstance = create(name, type, value);

		Context.CommandContext.DbSqlSession.insert(variableInstance);

		return variableInstance;
	  }

	  public static VariableInstanceEntity create(string name, VariableType type, object value)
	  {
		VariableInstanceEntity variableInstance = new VariableInstanceEntity();
		variableInstance.name = name;
		variableInstance.type = type;
		variableInstance.typeName = type.TypeName;
		variableInstance.Value = value;
		return variableInstance;
	  }

	  public virtual ExecutionEntity Execution
	  {
		  set
		  {
			this.executionId = value.Id;
			this.processInstanceId = value.ProcessInstanceId;
			forceUpdate();
		  }
	  }

	  public virtual void forceUpdate()
	  {
			forcedUpdate = true;

	  }

	  public virtual void delete()
	  {
		Context.CommandContext.DbSqlSession.delete(this);

		if (!deleted && Context.ProcessEngineConfiguration != null && Context.ProcessEngineConfiguration.EventDispatcher.Enabled)
		{
		  Context.ProcessEngineConfiguration.EventDispatcher.dispatchEvent(createVariableDeleteEvent(this));
		}

		byteArrayRef.delete();
		deleted = true;

	  }

	  protected internal static ActivitiVariableEvent createVariableDeleteEvent(VariableInstanceEntity variableInstance)
	  {
		return ActivitiEventBuilder.createVariableEvent(ActivitiEventType.VARIABLE_DELETED, variableInstance.Name, null, variableInstance.Type, variableInstance.TaskId, variableInstance.ExecutionId, variableInstance.ProcessInstanceId, null);
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
			if (longValue != null)
			{
			  persistentState["longValue"] = longValue;
			}
			if (doubleValue != null)
			{
			  persistentState["doubleValue"] = doubleValue;
			}
			if (textValue != null)
			{
			  persistentState["textValue"] = textValue;
			}
			if (textValue2 != null)
			{
			  persistentState["textValue2"] = textValue2;
			}
			if (byteArrayRef.Id != null)
			{
			  persistentState["byteArrayValueId"] = byteArrayRef.Id;
			}
			if (forcedUpdate)
			{
			  persistentState["forcedUpdate"] = true;
			}
			return persistentState;
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }


	  public virtual bool Deleted
	  {
		  get
		  {
			return deleted;
		  }
	  }

	  // lazy initialized relations ///////////////////////////////////////////////

	  public virtual string ProcessInstanceId
	  {
		  set
		  {
			this.processInstanceId = value;
		  }
		  get
		  {
			return processInstanceId;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  set
		  {
			this.executionId = value;
		  }
		  get
		  {
			return executionId;
		  }
	  }

	  // byte array value /////////////////////////////////////////////////////////

	  public override sbyte[] Bytes
	  {
		  get
		  {
			return byteArrayRef.Bytes;
		  }
		  set
		  {
			byteArrayRef.setValue("var-" + name, value);
		  }
	  }


	  [Obsolete]
	  public override ByteArrayEntity getByteArrayValue()
	  {
		return byteArrayRef.Entity;
	  }

	  [Obsolete]
	  public override string ByteArrayValueId
	  {
		  get
		  {
			return byteArrayRef.Id;
		  }
	  }

	  [Obsolete]
	  public override void setByteArrayValue(sbyte[] bytes)
	  {
		Bytes = bytes;
	  }

	  // value ////////////////////////////////////////////////////////////////////

	  public virtual object Value
	  {
		  get
		  {
			if (!type.Cachable || cachedValue == null)
			{
			  cachedValue = type.getValue(this);
			}
			return cachedValue;
		  }
		  set
		  {
			type.setValue(value, this);
			typeName = type.TypeName;
			cachedValue = value;
		  }
	  }


	  // getters and setters //////////////////////////////////////////////////////

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }

	  public virtual int Revision
	  {
		  get
		  {
			return revision;
		  }
		  set
		  {
			this.revision = value;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }


	  public virtual string LocalizedName
	  {
		  get
		  {
			return localizedName;
		  }
		  set
		  {
			this.localizedName = value;
		  }
	  }


	  public virtual string LocalizedDescription
	  {
		  get
		  {
			return localizedDescription;
		  }
		  set
		  {
			this.localizedDescription = value;
		  }
	  }


	  public virtual string TypeName
	  {
		  get
		  {
			return typeName;
		  }
		  set
		  {
			this.typeName = value;
		  }
	  }

	  public virtual VariableType Type
	  {
		  get
		  {
			return type;
		  }
		  set
		  {
			this.type = value;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId;
		  }
		  set
		  {
			this.taskId = value;
		  }
	  }

	  public virtual long? LongValue
	  {
		  get
		  {
			return longValue;
		  }
		  set
		  {
			this.longValue = value;
		  }
	  }

	  public virtual double? DoubleValue
	  {
		  get
		  {
			return doubleValue;
		  }
		  set
		  {
			this.doubleValue = value;
		  }
	  }

	  public virtual string TextValue
	  {
		  get
		  {
			return textValue;
		  }
		  set
		  {
			this.textValue = value;
		  }
	  }

	  public virtual string TextValue2
	  {
		  get
		  {
			return textValue2;
		  }
		  set
		  {
			this.textValue2 = value;
		  }
	  }

	  public virtual object CachedValue
	  {
		  get
		  {
			return cachedValue;
		  }
		  set
		  {
			this.cachedValue = value;
		  }
	  }

	  // misc methods /////////////////////////////////////////////////////////////

	  public override string ToString()
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append("VariableInstanceEntity[");
		sb.Append("id=").Append(id);
		sb.Append(", name=").Append(name);
		sb.Append(", type=").Append(type != null ? type.TypeName : "null");
		if (longValue != null)
		{
		  sb.Append(", longValue=").Append(longValue);
		}
		if (doubleValue != null)
		{
		  sb.Append(", doubleValue=").Append(doubleValue);
		}
		if (textValue != null)
		{
		  sb.Append(", textValue=").Append(StringUtils.abbreviate(textValue, 40));
		}
		if (textValue2 != null)
		{
		  sb.Append(", textValue2=").Append(StringUtils.abbreviate(textValue2, 40));
		}
		if (byteArrayRef.Id != null)
		{
		  sb.Append(", byteArrayValueId=").Append(byteArrayRef.Id);
		}
		sb.Append("]");
		return sb.ToString();
	  }

	}

}