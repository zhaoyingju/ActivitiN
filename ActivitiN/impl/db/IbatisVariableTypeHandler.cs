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

namespace org.activiti.engine.impl.db
{


	using Context = org.activiti.engine.impl.context.Context;
	using VariableType = org.activiti.engine.impl.variable.VariableType;
	using VariableTypes = org.activiti.engine.impl.variable.VariableTypes;
	using JdbcType = org.apache.ibatis.type.JdbcType;
	using TypeHandler = org.apache.ibatis.type.TypeHandler;


	/// <summary>
	/// @author Dave Syer
	/// </summary>
	public class IbatisVariableTypeHandler : TypeHandler<VariableType>
	{

	  protected internal VariableTypes variableTypes;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.activiti.engine.impl.variable.VariableType getResult(java.sql.ResultSet rs, String columnName) throws java.sql.SQLException
	  public virtual VariableType getResult(ResultSet rs, string columnName)
	  {
		string typeName = rs.getString(columnName);
		VariableType type = VariableTypes.getVariableType(typeName);
		if (type == null && typeName != null)
		{
		  throw new ActivitiException("unknown variable type name " + typeName);
		}
		return type;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.activiti.engine.impl.variable.VariableType getResult(java.sql.CallableStatement cs, int columnIndex) throws java.sql.SQLException
	  public virtual VariableType getResult(CallableStatement cs, int columnIndex)
	  {
		string typeName = cs.getString(columnIndex);
		VariableType type = VariableTypes.getVariableType(typeName);
		if (type == null)
		{
		  throw new ActivitiException("unknown variable type name " + typeName);
		}
		return type;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setParameter(java.sql.PreparedStatement ps, int i, org.activiti.engine.impl.variable.VariableType parameter, org.apache.ibatis.type.JdbcType jdbcType) throws java.sql.SQLException
	  public virtual void setParameter(PreparedStatement ps, int i, VariableType parameter, JdbcType jdbcType)
	  {
		string typeName = parameter.TypeName;
		ps.setString(i, typeName);
	  }

	  protected internal virtual VariableTypes VariableTypes
	  {
		  get
		  {
			if (variableTypes == null)
			{
			  variableTypes = Context.ProcessEngineConfiguration.VariableTypes;
			}
			return variableTypes;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.activiti.engine.impl.variable.VariableType getResult(java.sql.ResultSet resultSet, int columnIndex) throws java.sql.SQLException
	  public virtual VariableType getResult(ResultSet resultSet, int columnIndex)
	  {
		string typeName = resultSet.getString(columnIndex);
		VariableType type = VariableTypes.getVariableType(typeName);
		if (type == null)
		{
		  throw new ActivitiException("unknown variable type name " + typeName);
		}
		return type;
	  }
	}

}