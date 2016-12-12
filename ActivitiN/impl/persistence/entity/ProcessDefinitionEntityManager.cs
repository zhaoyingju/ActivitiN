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

namespace org.activiti.engine.impl.persistence.entity
{


	using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;


	/// <summary>
	/// @author Tom Baeyens
	/// @author Falko Menge
	/// @author Saeid Mirzaei
	/// @author Joram Barrez
	/// </summary>
	public class ProcessDefinitionEntityManager : AbstractManager
	{

	  public virtual ProcessDefinitionEntity findLatestProcessDefinitionByKey(string processDefinitionKey)
	  {
		return (ProcessDefinitionEntity) DbSqlSession.selectOne("selectLatestProcessDefinitionByKey", processDefinitionKey);
	  }

	  public virtual ProcessDefinitionEntity findLatestProcessDefinitionByKeyAndTenantId(string processDefinitionKey, string tenantId)
	  {
		  IDictionary<string, object> @params = new Dictionary<string, object>(2);
		  @params["processDefinitionKey"] = processDefinitionKey;
		  @params["tenantId"] = tenantId;
		return (ProcessDefinitionEntity) DbSqlSession.selectOne("selectLatestProcessDefinitionByKeyAndTenantId", @params);
	  }

	  public virtual void deleteProcessDefinitionsByDeploymentId(string deploymentId)
	  {
		DbSqlSession.delete("deleteProcessDefinitionsByDeploymentId", deploymentId);
	  }

	  public virtual ProcessDefinitionEntity findProcessDefinitionById(string processDefinitionId)
	  {
		return (ProcessDefinitionEntity) DbSqlSession.selectOne("selectProcessDefinitionById", processDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.repository.ProcessDefinition> findProcessDefinitionsByQueryCriteria(org.activiti.engine.impl.ProcessDefinitionQueryImpl processDefinitionQuery, org.activiti.engine.impl.Page page)
	  public virtual IList<ProcessDefinition> findProcessDefinitionsByQueryCriteria(ProcessDefinitionQueryImpl processDefinitionQuery, Page page)
	  {
	//    List<ProcessDefinition> processDefinitions = 
		return DbSqlSession.selectList("selectProcessDefinitionsByQueryCriteria", processDefinitionQuery, page);

		//skipped this after discussion within the team
	//    // retrieve process definitions from cache (https://activiti.atlassian.net/browse/ACT-1020) to have all available information
	//    ArrayList<ProcessDefinition> result = new ArrayList<ProcessDefinition>();
	//    for (ProcessDefinition processDefinitionEntity : processDefinitions) {      
	//      ProcessDefinitionEntity fullProcessDefinition = Context
	//              .getProcessEngineConfiguration()
	//              .getDeploymentCache().resolveProcessDefinition((ProcessDefinitionEntity)processDefinitionEntity);
	//      result.add(fullProcessDefinition);
	//    }
	//    return result;
	  }

	  public virtual long findProcessDefinitionCountByQueryCriteria(ProcessDefinitionQueryImpl processDefinitionQuery)
	  {
		return (long?) DbSqlSession.selectOne("selectProcessDefinitionCountByQueryCriteria", processDefinitionQuery);
	  }

	  public virtual ProcessDefinitionEntity findProcessDefinitionByDeploymentAndKey(string deploymentId, string processDefinitionKey)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["deploymentId"] = deploymentId;
		parameters["processDefinitionKey"] = processDefinitionKey;
		return (ProcessDefinitionEntity) DbSqlSession.selectOne("selectProcessDefinitionByDeploymentAndKey", parameters);
	  }

	  public virtual ProcessDefinitionEntity findProcessDefinitionByDeploymentAndKeyAndTenantId(string deploymentId, string processDefinitionKey, string tenantId)
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["deploymentId"] = deploymentId;
		parameters["processDefinitionKey"] = processDefinitionKey;
		parameters["tenantId"] = tenantId;
		return (ProcessDefinitionEntity) DbSqlSession.selectOne("selectProcessDefinitionByDeploymentAndKeyAndTenantId", parameters);
	  }

	  public virtual ProcessDefinition findProcessDefinitionByKeyAndVersion(string processDefinitionKey, int? processDefinitionVersion)
	  {
		ProcessDefinitionQueryImpl processDefinitionQuery = (new ProcessDefinitionQueryImpl()).processDefinitionKey(processDefinitionKey).processDefinitionVersion(processDefinitionVersion);
		IList<ProcessDefinition> results = findProcessDefinitionsByQueryCriteria(processDefinitionQuery, null);
		if (results.Count == 1)
		{
		  return results[0];
		}
		else if (results.Count > 1)
		{
		  throw new ActivitiException("There are " + results.Count + " process definitions with key = '" + processDefinitionKey + "' and version = '" + processDefinitionVersion + "'.");
		}
		return null;
	  }

	  public virtual IList<ProcessDefinition> findProcessDefinitionsStartableByUser(string user)
	  {
		return (new ProcessDefinitionQueryImpl()).startableByUser(user).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<org.activiti.engine.repository.ProcessDefinition> findProcessDefinitionsByNativeQuery(java.util.Map<String, Object> parameterMap, int firstResult, int maxResults)
	  public virtual IList<ProcessDefinition> findProcessDefinitionsByNativeQuery(IDictionary<string, object> parameterMap, int firstResult, int maxResults)
	  {
		return DbSqlSession.selectListWithRawParameter("selectProcessDefinitionByNativeQuery", parameterMap, firstResult, maxResults);
	  }

	  public virtual long findProcessDefinitionCountByNativeQuery(IDictionary<string, object> parameterMap)
	  {
		return (long?) DbSqlSession.selectOne("selectProcessDefinitionCountByNativeQuery", parameterMap);
	  }

	  public virtual void updateProcessDefinitionTenantIdForDeployment(string deploymentId, string newTenantId)
	  {
		  Dictionary<string, object> @params = new Dictionary<string, object>();
		  @params["deploymentId"] = deploymentId;
		  @params["tenantId"] = newTenantId;
		  DbSqlSession.update("updateProcessDefinitionTenantIdForDeploymentId", @params);
	  }

	}

}