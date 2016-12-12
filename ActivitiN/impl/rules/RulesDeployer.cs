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

namespace org.activiti.engine.impl.rules
{

	using Context = org.activiti.engine.impl.context.Context;
	using Deployer = org.activiti.engine.impl.persistence.deploy.Deployer;
	using DeploymentManager = org.activiti.engine.impl.persistence.deploy.DeploymentManager;
	using DeploymentEntity = org.activiti.engine.impl.persistence.entity.DeploymentEntity;
	using ResourceEntity = org.activiti.engine.impl.persistence.entity.ResourceEntity;
	using KnowledgeBase = org.drools.KnowledgeBase;
	using KnowledgeBuilder = org.drools.builder.KnowledgeBuilder;
	using KnowledgeBuilderFactory = org.drools.builder.KnowledgeBuilderFactory;
	using ResourceType = org.drools.builder.ResourceType;
	using Resource = org.drools.io.Resource;
	using ResourceFactory = org.drools.io.ResourceFactory;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class RulesDeployer : Deployer
	{

	  private static readonly Logger log = LoggerFactory.getLogger(typeof(RulesDeployer));

	  public virtual void deploy(DeploymentEntity deployment, IDictionary<string, object> deploymentSettings)
	  {
		log.debug("Processing deployment {}", deployment.Name);

		KnowledgeBuilder knowledgeBuilder = null;

		DeploymentManager deploymentManager = Context.ProcessEngineConfiguration.DeploymentManager;

		IDictionary<string, ResourceEntity> resources = deployment.Resources;
		foreach (string resourceName in resources.Keys)
		{
		  log.info("Processing resource {}", resourceName);
		  if (resourceName.EndsWith(".drl")) // is only parsing .drls sufficient? what about other rule dsl's? (@see ResourceType)
		  {
			if (knowledgeBuilder == null)
			{
			  knowledgeBuilder = KnowledgeBuilderFactory.newKnowledgeBuilder();
			}
			ResourceEntity resourceEntity = resources[resourceName];
			sbyte[] resourceBytes = resourceEntity.Bytes;
			Resource droolsResource = ResourceFactory.newByteArrayResource(resourceBytes);
			knowledgeBuilder.add(droolsResource, ResourceType.DRL);
		  }
		}

		if (knowledgeBuilder != null)
		{
		  KnowledgeBase knowledgeBase = knowledgeBuilder.newKnowledgeBase();
		  deploymentManager.getKnowledgeBaseCache().add(deployment.Id, knowledgeBase);
		}
	  }
	}

}