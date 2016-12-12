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


	using HistoricVariableInstance = org.activiti.engine.history.HistoricVariableInstance;
	using Context = org.activiti.engine.impl.context.Context;
	using BulkDeleteable = org.activiti.engine.impl.db.BulkDeleteable;
	using HasRevision = org.activiti.engine.impl.db.HasRevision;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using ValueFields = org.activiti.engine.impl.variable.ValueFields;
	using VariableType = org.activiti.engine.impl.variable.VariableType;
	using StringUtils = org.apache.commons.lang3.StringUtils;

	/// <summary>
	/// @author Christian Lipphardt (camunda)
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class HistoricVariableInstanceEntity : ValueFields, HistoricVariableInstance, PersistentObject, HasRevision, BulkDeleteable
	{

	  private const long serialVersionUID = 1L;

	  protected internal string id;
	  protected internal int revision;

	  protected internal string name;
	  protected internal VariableType variableType;

	  protected internal string processInstanceId;
	  protected internal string executionId;
	  protected internal string taskId;

	  protected internal DateTime createTime;
	  protected internal DateTime lastUpdatedTime;

	  protected internal long? longValue;
	  protected internal double? doubleValue;
	  protected internal string textValue;
	  protected internal string textValue2;
	  protected internal readonly ByteArrayRef byteArrayRef = new ByteArrayRef();

	  protected internal object cachedValue;

	  // Default constructor for SQL mapping
	  protected internal HistoricVariableInstanceEntity()
	  {
	  }

	  public static HistoricVariableInstanceEntity copyAndInsert(VariableInstanceEntity variableInstance)
	  {
		HistoricVariableInstanceEntity historicVariableInstance = new HistoricVariableInstanceEntity();
		historicVariableInstance.id = variableInstance.Id;
		historicVariableInstance.processInstanceId = variableInstance.ProcessInstanceId;
		historicVariableInstance.executionId = variableInstance.ExecutionId;
		historicVariableInstance.taskId = variableInstance.TaskId;
		historicVariableInstance.revision = variableInstance.Revision;
		historicVariableInstance.name = variableInstance.Name;
		historicVariableInstance.variableType = variableInstance.Type;

		historicVariableInstance.copyValue(variableInstance);

		DateTime time = Context.ProcessEngineConfiguration.Clock.CurrentTime;
		historicVariableInstance.CreateTime = time;
		historicVariableInstance.LastUpdatedTime = time;

		Context.CommandContext.DbSqlSession.insert(historicVariableInstance);

		return historicVariableInstance;
	  }

	  public virtual void copyValue(VariableInstanceEntity variableInstance)
	  {
		this.textValue = variableInstance.TextValue;
		this.textValue2 = variableInstance.TextValue2;
		this.doubleValue = variableInstance.DoubleValue;
		this.longValue = variableInstance.LongValue;

		this.variableType = variableInstance.Type;
		if (variableInstance.ByteArrayValueId != null)
		{
		  setByteArrayValue(variableInstance.getByteArrayValue().Bytes);
		}

		this.lastUpdatedTime = Context.ProcessEngineConfiguration.Clock.CurrentTime;
	  }

	  public virtual void delete()
	  {
		Context.CommandContext.DbSqlSession.delete(this);

		byteArrayRef.delete();
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			  Dictionary<string, object> persistentState = new Dictionary<string, object>();
    
			  persistentState["textValue"] = textValue;
			  persistentState["textValue2"] = textValue2;
			  persistentState["doubleValue"] = doubleValue;
			  persistentState["longValue"] = longValue;
			  persistentState["byteArrayRef"] = byteArrayRef.Id;
			  persistentState["createTime"] = createTime;
			  persistentState["lastUpdatedTime"] = lastUpdatedTime;
    
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

	  public virtual object Value
	  {
		  get
		  {
			if (!variableType.Cachable || cachedValue == null)
			{
			  cachedValue = variableType.getValue(this);
			}
			return cachedValue;
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
			byteArrayRef.setValue("hist.var-" + name, value);
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


	  public virtual string VariableTypeName
	  {
		  get
		  {
			return (variableType != null ? variableType.TypeName : null);
		  }
	  }

	  public virtual string VariableName
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual VariableType VariableType
	  {
		  get
		  {
			return variableType;
		  }
		  set
		  {
			this.variableType = value;
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


	  public virtual DateTime CreateTime
	  {
		  get
		  {
				return createTime;
		  }
		  set
		  {
				this.createTime = value;
		  }
	  }


		public virtual DateTime LastUpdatedTime
		{
			get
			{
				return lastUpdatedTime;
			}
			set
			{
				this.lastUpdatedTime = value;
			}
		}


		public virtual string ExecutionId
		{
			get
			{
			return executionId;
			}
			set
			{
			this.executionId = value;
			}
		}


	  public virtual DateTime Time
	  {
		  get
		  {
			return CreateTime;
		  }
	  }

	  // common methods  //////////////////////////////////////////////////////////

	  public override string ToString()
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append("HistoricVariableInstanceEntity[");
		sb.Append("id=").Append(id);
		sb.Append(", name=").Append(name);
		sb.Append(", revision=").Append(revision);
		sb.Append(", type=").Append(variableType != null ? variableType.TypeName : "null");
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