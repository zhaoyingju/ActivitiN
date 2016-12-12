using System;
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

	using HistoricVariableUpdate = org.activiti.engine.history.HistoricVariableUpdate;
	using Context = org.activiti.engine.impl.context.Context;
	using HasRevision = org.activiti.engine.impl.db.HasRevision;
	using PersistentObject = org.activiti.engine.impl.db.PersistentObject;
	using ValueFields = org.activiti.engine.impl.variable.ValueFields;
	using VariableType = org.activiti.engine.impl.variable.VariableType;
	using StringUtils = org.apache.commons.lang3.StringUtils;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class HistoricDetailVariableInstanceUpdateEntity : HistoricDetailEntity, ValueFields, HistoricVariableUpdate, PersistentObject, HasRevision
	{

	  private const long serialVersionUID = 1L;

	  protected internal int revision;

	  protected internal string name;
	  protected internal VariableType variableType;

	  protected internal long? longValue;
	  protected internal double? doubleValue;
	  protected internal string textValue;
	  protected internal string textValue2;
	  protected internal readonly ByteArrayRef byteArrayRef = new ByteArrayRef();

	  protected internal object cachedValue;

	  protected internal HistoricDetailVariableInstanceUpdateEntity()
	  {
		this.detailType = "VariableUpdate";
	  }

	  public static HistoricDetailVariableInstanceUpdateEntity copyAndInsert(VariableInstanceEntity variableInstance)
	  {
		HistoricDetailVariableInstanceUpdateEntity historicVariableUpdate = new HistoricDetailVariableInstanceUpdateEntity();
		historicVariableUpdate.processInstanceId = variableInstance.ProcessInstanceId;
		historicVariableUpdate.executionId = variableInstance.ExecutionId;
		historicVariableUpdate.taskId = variableInstance.TaskId;
		historicVariableUpdate.time = Context.ProcessEngineConfiguration.Clock.CurrentTime;
		historicVariableUpdate.revision = variableInstance.Revision;
		historicVariableUpdate.name = variableInstance.Name;
		historicVariableUpdate.variableType = variableInstance.Type;
		historicVariableUpdate.textValue = variableInstance.TextValue;
		historicVariableUpdate.textValue2 = variableInstance.TextValue2;
		historicVariableUpdate.doubleValue = variableInstance.DoubleValue;
		historicVariableUpdate.longValue = variableInstance.LongValue;

		if (variableInstance.Bytes != null)
		{
		  string byteArrayName = "hist.detail.var-" + variableInstance.Name;
		  historicVariableUpdate.byteArrayRef.setValue(byteArrayName, variableInstance.Bytes);
		}

		Context.CommandContext.DbSqlSession.insert(historicVariableUpdate);

		return historicVariableUpdate;
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

	  public override void delete()
	  {
		base.delete();

		byteArrayRef.delete();
	  }

	  public override object PersistentState
	  {
		  get
		  {
			// HistoricDetailVariableInstanceUpdateEntity is immutable, so always the same object is returned
			return typeof(HistoricDetailVariableInstanceUpdateEntity);
		  }
	  }

	  public virtual string VariableTypeName
	  {
		  get
		  {
			return (variableType != null ? variableType.TypeName : null);
		  }
	  }

	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
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
			throw new System.NotSupportedException("HistoricDetailVariableInstanceUpdateEntity is immutable");
		  }
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
	  public override ByteArrayEntity getByteArrayValue()
	  {
		return byteArrayRef.Entity;
	  }

	  [Obsolete]
	  public override void setByteArrayValue(sbyte[] bytes)
	  {
		Bytes = bytes;
	  }

	  // getters and setters //////////////////////////////////////////////////////

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

	  public virtual string VariableName
	  {
		  get
		  {
			return name;
		  }
	  }
	  public virtual string Name
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
		sb.Append("HistoricDetailVariableInstanceUpdateEntity[");
		sb.Append("id=").Append(id);
		sb.Append(", name=").Append(name);
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