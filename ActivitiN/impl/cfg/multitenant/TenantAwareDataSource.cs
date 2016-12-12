using System;
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

namespace org.activiti.engine.impl.cfg.multitenant
{



	/// <summary>
	/// A <seealso cref="DataSource"/> implementation that switches the currently used datasource based on the
	/// current values of the <seealso cref="TenantInfoHolder"/>.
	/// 
	/// When a <seealso cref="Connection"/> is requested from this <seealso cref="DataSource"/>, the correct <seealso cref="DataSource"/>
	/// for the current tenant will be determined and used.
	/// 
	/// Heavily influenced and inspired by Spring's AbstractRoutingDataSource.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class TenantAwareDataSource : DataSource
	{

	  protected internal TenantInfoHolder tenantInfoHolder;
	  protected internal IDictionary<object, DataSource> dataSources = new Dictionary<object, DataSource>();

	  public TenantAwareDataSource(TenantInfoHolder tenantInfoHolder)
	  {
		this.tenantInfoHolder = tenantInfoHolder;
	  }

	  public virtual void addDataSource(object key, DataSource dataSource)
	  {
		dataSources[key] = dataSource;
	  }

	  public virtual void removeDataSource(object key)
	  {
		dataSources.Remove(key);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Connection getConnection() throws java.sql.SQLException
	  public virtual Connection Connection
	  {
		  get
		  {
			return CurrentDataSource.Connection;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.sql.Connection getConnection(String username, String password) throws java.sql.SQLException
	  public virtual Connection getConnection(string username, string password)
	  {
		return CurrentDataSource.getConnection(username, password);
	  }

	  protected internal virtual DataSource CurrentDataSource
	  {
		  get
		  {
			string tenantId = tenantInfoHolder.CurrentTenantId;
			DataSource dataSource = dataSources[tenantId];
			if (dataSource == null)
			{
			  throw new ActivitiException("Could not find a dataSource for tenant " + tenantId);
			}
			return dataSource;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int getLoginTimeout() throws java.sql.SQLException
	  public virtual int LoginTimeout
	  {
		  get
		  {
			return 0; // Default
		  }
		  set
		  {
			throw new System.NotSupportedException();
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.logging.Logger getParentLogger() throws java.sql.SQLFeatureNotSupportedException
	  public virtual Logger ParentLogger
	  {
		  get
		  {
			return Logger.getLogger(Logger.GLOBAL_LOGGER_NAME);
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> T unwrap(Class iface) throws java.sql.SQLException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual T unwrap<T>(Type iface)
	  {
		if (iface.IsInstanceOfType(this))
		{
		  return (T) this;
		}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		throw new SQLException("Cannot unwrap " + this.GetType().FullName + " as an instance of " + iface.FullName);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean isWrapperFor(Class iface) throws java.sql.SQLException
	  public virtual bool isWrapperFor(Type iface)
	  {
		return iface.IsInstanceOfType(this);
	  }

	  public virtual IDictionary<object, DataSource> DataSources
	  {
		  get
		  {
			return dataSources;
		  }
		  set
		  {
			this.dataSources = value;
		  }
	  }


	  // Unsupported //////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.io.PrintWriter getLogWriter() throws java.sql.SQLException
	  public virtual PrintWriter LogWriter
	  {
		  get
		  {
			throw new System.NotSupportedException();
		  }
		  set
		  {
			throw new System.NotSupportedException();
		  }
	  }



	}

}