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
namespace org.activiti.engine.impl.persistence.deploy
{


	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;
	using ProcessDefinitionInfoEntity = org.activiti.engine.impl.persistence.entity.ProcessDefinitionInfoEntity;
	using ProcessDefinitionInfoEntityManager = org.activiti.engine.impl.persistence.entity.ProcessDefinitionInfoEntityManager;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using ObjectNode = com.fasterxml.jackson.databind.node.ObjectNode;

	/// <summary>
	/// Default cache: keep everything in memory, unless a limit is set.
	/// 
	/// @author Tijs Rademakers
	/// </summary>
	public class ProcessDefinitionInfoCache
	{

	  private static readonly Logger logger = LoggerFactory.getLogger(typeof(ProcessDefinitionInfoCache));

	  protected internal IDictionary<string, ProcessDefinitionInfoCacheObject> cache;
	  protected internal CommandExecutor commandExecutor;

	  /// <summary>
	  /// Cache with no limit </summary>
	  public ProcessDefinitionInfoCache(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
		this.cache = Collections.synchronizedMap(new Dictionary<string, ProcessDefinitionInfoCacheObject>());
	  }

	  /// <summary>
	  /// Cache which has a hard limit: no more elements will be cached than the limit. </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ProcessDefinitionInfoCache(org.activiti.engine.impl.interceptor.CommandExecutor commandExecutor, final int limit)
	  public ProcessDefinitionInfoCache(CommandExecutor commandExecutor, int limit)
	  {
		this.commandExecutor = commandExecutor;
		this.cache = Collections.synchronizedMap(new LinkedHashMapAnonymousInnerClassHelper(this, limit + 1, limit));
	  }

	  private class LinkedHashMapAnonymousInnerClassHelper : LinkedHashMap<string, ProcessDefinitionInfoCacheObject>
	  {
		  private readonly ProcessDefinitionInfoCache outerInstance;

		  private int limit;

		  public LinkedHashMapAnonymousInnerClassHelper(ProcessDefinitionInfoCache outerInstance, int limit, int limit) : base(limit + 1, 0.75f, true)
		  {
			  this.outerInstance = outerInstance;
			  this.limit = limit;
			  serialVersionUID = 1L;
		  }

				// +1 is needed, because the entry is inserted first, before it is removed
				// 0.75 is the default (see javadocs)
				// true will keep the 'access-order', which is needed to have a real LRU cache
		  private static readonly long serialVersionUID;

		  protected internal virtual bool removeEldestEntry(KeyValuePair<string, ProcessDefinitionInfoCacheObject> eldest)
		  {
			bool removeEldest = outerInstance.size() > limit;
			if (removeEldest)
			{
			  logger.trace("Cache limit is reached, {} will be evicted", eldest.Key);
			}
			return removeEldest;
		  }

	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public ProcessDefinitionInfoCacheObject get(final String processDefinitionId)
	  public virtual ProcessDefinitionInfoCacheObject get(string processDefinitionId)
	  {
		ProcessDefinitionInfoCacheObject infoCacheObject = null;
		if (cache.ContainsKey(processDefinitionId))
		{
		  infoCacheObject = commandExecutor.execute(new CommandAnonymousInnerClassHelper(this, processDefinitionId));
		}
		return infoCacheObject;
	  }

	  private class CommandAnonymousInnerClassHelper : Command<ProcessDefinitionInfoCacheObject>
	  {
		  private readonly ProcessDefinitionInfoCache outerInstance;

		  private string processDefinitionId;

		  public CommandAnonymousInnerClassHelper(ProcessDefinitionInfoCache outerInstance, string processDefinitionId)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinitionId = processDefinitionId;
		  }


		  public virtual ProcessDefinitionInfoCacheObject execute(CommandContext commandContext)
		  {
			ProcessDefinitionInfoEntityManager infoEntityManager = commandContext.ProcessDefinitionInfoEntityManager;
			ObjectMapper objectMapper = commandContext.ProcessEngineConfiguration.ObjectMapper;

			ProcessDefinitionInfoCacheObject cacheObject = outerInstance.cache[processDefinitionId];
			ProcessDefinitionInfoEntity infoEntity = infoEntityManager.findProcessDefinitionInfoByProcessDefinitionId(processDefinitionId);
			if (infoEntity != null && infoEntity.Revision != cacheObject.Revision)
			{
			  cacheObject.Revision = infoEntity.Revision;
			  if (infoEntity.InfoJsonId != null)
			  {
				sbyte[] infoBytes = infoEntityManager.findInfoJsonById(infoEntity.InfoJsonId);
				try
				{
				  ObjectNode infoNode = (ObjectNode) objectMapper.readTree(infoBytes);
				  cacheObject.InfoNode = infoNode;
				}
				catch (Exception e)
				{
				  throw new ActivitiException("Error reading json info node for process definition " + processDefinitionId, e);
				}
			  }
			}
			else if (infoEntity == null)
			{
			  cacheObject.Revision = 0;
			  cacheObject.InfoNode = objectMapper.createObjectNode();
			}
			return cacheObject;
		  }
	  }

	  public virtual void add(string id, ProcessDefinitionInfoCacheObject obj)
	  {
		cache[id] = obj;
	  }

	  public virtual void remove(string id)
	  {
		cache.Remove(id);
	  }

	  public virtual void clear()
	  {
		cache.Clear();
	  }

	  // For testing purposes only
	  public virtual int size()
	  {
		return cache.Count;
	  }

	}

}