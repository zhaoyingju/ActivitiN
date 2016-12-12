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

namespace org.activiti.engine.impl.rules
{

	using Context = org.activiti.engine.impl.context.Context;
	using org.activiti.engine.impl.persistence.deploy;
	using DeploymentEntity = org.activiti.engine.impl.persistence.entity.DeploymentEntity;
	using Deployment = org.activiti.engine.repository.Deployment;
	using KnowledgeBase = org.drools.KnowledgeBase;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class RulesHelper
	{

	  public static KnowledgeBase findKnowledgeBaseByDeploymentId(string deploymentId)
	  {
		DeploymentCache<object> knowledgeBaseCache = Context.ProcessEngineConfiguration.DeploymentManager.KnowledgeBaseCache;

		KnowledgeBase knowledgeBase = (KnowledgeBase) knowledgeBaseCache.get(deploymentId);
		if (knowledgeBase == null)
		{
		  DeploymentEntity deployment = Context.CommandContext.DeploymentEntityManager.findDeploymentById(deploymentId);
		  if (deployment == null)
		  {
			throw new ActivitiObjectNotFoundException("no deployment with id " + deploymentId, typeof(Deployment));
		  }
		  Context.ProcessEngineConfiguration.DeploymentManager.deploy(deployment);
		  knowledgeBase = (KnowledgeBase) knowledgeBaseCache.get(deploymentId);
		  if (knowledgeBase == null)
		  {
			throw new ActivitiException("deployment " + deploymentId + " doesn't contain any rules");
		  }
		}
		return knowledgeBase;
	  }
	}

}