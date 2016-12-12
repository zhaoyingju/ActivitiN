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



	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class ResourceEntityManager : AbstractManager
	{

	  public virtual void insertResource(ResourceEntity resource)
	  {
		DbSqlSession.insert(resource);
	  }

	  public virtual void deleteResourcesByDeploymentId(string deploymentId)
	  {
		DbSqlSession.delete("deleteResourcesByDeploymentId", deploymentId);
	  }

	  public virtual ResourceEntity findResourceByDeploymentIdAndResourceName(string deploymentId, string resourceName)
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["deploymentId"] = deploymentId;
		@params["resourceName"] = resourceName;
		return (ResourceEntity) DbSqlSession.selectOne("selectResourceByDeploymentIdAndResourceName", @params);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<ResourceEntity> findResourcesByDeploymentId(String deploymentId)
	  public virtual IList<ResourceEntity> findResourcesByDeploymentId(string deploymentId)
	  {
		return DbSqlSession.selectList("selectResourcesByDeploymentId", deploymentId);
	  }


	}

}