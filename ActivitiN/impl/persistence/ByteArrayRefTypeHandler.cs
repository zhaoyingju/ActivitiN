namespace org.activiti.engine.impl.persistence
{


	using ByteArrayRef = org.activiti.engine.impl.persistence.entity.ByteArrayRef;
	using JdbcType = org.apache.ibatis.type.JdbcType;
	using TypeHandler = org.apache.ibatis.type.TypeHandler;
	using TypeReference = org.apache.ibatis.type.TypeReference;

	/// <summary>
	/// MyBatis TypeHandler for <seealso cref="ByteArrayRef"/>.
	/// 
	/// @author Marcus Klimstra (CGI)
	/// </summary>
	public class ByteArrayRefTypeHandler : TypeReference<ByteArrayRef>, TypeHandler<ByteArrayRef>
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setParameter(java.sql.PreparedStatement ps, int i, org.activiti.engine.impl.persistence.entity.ByteArrayRef parameter, org.apache.ibatis.type.JdbcType jdbcType) throws java.sql.SQLException
	  public override void setParameter(PreparedStatement ps, int i, ByteArrayRef parameter, JdbcType jdbcType)
	  {
		ps.setString(i, getValueToSet(parameter));
	  }

	  private string getValueToSet(ByteArrayRef parameter)
	  {
		if (parameter == null)
		{
		  // Note that this should not happen: ByteArrayRefs should always be initialized.
		  return null;
		}
		return parameter.Id;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.activiti.engine.impl.persistence.entity.ByteArrayRef getResult(java.sql.ResultSet rs, String columnName) throws java.sql.SQLException
	  public override ByteArrayRef getResult(ResultSet rs, string columnName)
	  {
		string id = rs.getString(columnName);
		return new ByteArrayRef(id);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.activiti.engine.impl.persistence.entity.ByteArrayRef getResult(java.sql.ResultSet rs, int columnIndex) throws java.sql.SQLException
	  public override ByteArrayRef getResult(ResultSet rs, int columnIndex)
	  {
		string id = rs.getString(columnIndex);
		return new ByteArrayRef(id);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.activiti.engine.impl.persistence.entity.ByteArrayRef getResult(java.sql.CallableStatement cs, int columnIndex) throws java.sql.SQLException
	  public override ByteArrayRef getResult(CallableStatement cs, int columnIndex)
	  {
		string id = cs.getString(columnIndex);
		return new ByteArrayRef(id);
	  }

	}

}