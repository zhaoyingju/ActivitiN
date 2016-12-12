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

	using JtaTransactionContextFactory = org.activiti.engine.impl.cfg.jta.JtaTransactionContextFactory;
	using CommandInterceptor = org.activiti.engine.impl.interceptor.CommandInterceptor;
	using JtaTransactionInterceptor = org.activiti.engine.impl.interceptor.JtaTransactionInterceptor;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class JtaProcessEngineConfiguration : ProcessEngineConfigurationImpl
	{

	  protected internal TransactionManager transactionManager;

	  public JtaProcessEngineConfiguration()
	  {
		this.transactionsExternallyManaged = true;
	  }

	  protected internal override CommandInterceptor createTransactionInterceptor()
	  {
		if (transactionManager == null)
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ActivitiException("transactionManager is required property for JtaProcessEngineConfiguration, use " + typeof(StandaloneProcessEngineConfiguration).FullName + " otherwise");
		}

		return new JtaTransactionInterceptor(transactionManager);
	  }

	  protected internal override void initTransactionContextFactory()
	  {
		if (transactionContextFactory == null)
		{
		  transactionContextFactory = new JtaTransactionContextFactory(transactionManager);
		}
	  }

	  public virtual TransactionManager TransactionManager
	  {
		  get
		  {
			return transactionManager;
		  }
		  set
		  {
			this.transactionManager = value;
		  }
	  }

	}

}