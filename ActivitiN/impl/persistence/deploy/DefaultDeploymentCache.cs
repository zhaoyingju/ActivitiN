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


	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// Default cache: keep everything in memory, unless a limit is set.
	/// 
	/// @author Joram Barrez
	/// </summary>
	public class DefaultDeploymentCache<T> : DeploymentCache<T>
	{

	  private static readonly Logger logger = LoggerFactory.getLogger(typeof(DefaultDeploymentCache));

	  protected internal IDictionary<string, T> cache;

	  /// <summary>
	  /// Cache with no limit </summary>
	  public DefaultDeploymentCache() : this(-1)
	  {
	  }

	  /// <summary>
	  /// Cache which has a hard limit: no more elements will be cached than the limit. </summary>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public DefaultDeploymentCache(final int limit)
	  public DefaultDeploymentCache(int limit)
	  {
		if (limit > 0)
		{
		  this.cache = Collections.synchronizedMap(new LinkedHashMapAnonymousInnerClassHelper(this, limit + 1, limit)); // +1 is needed, because the entry is inserted first, before it is removed
		}
		else
		{
		  this.cache = Collections.synchronizedMap(new Dictionary<string, T>());
		}
	  }

	  private class LinkedHashMapAnonymousInnerClassHelper : LinkedHashMap<string, T>
	  {
		  private readonly DefaultDeploymentCache<T> outerInstance;

		  private int limit;

		  public LinkedHashMapAnonymousInnerClassHelper(DefaultDeploymentCache<T> outerInstance, int limit, int limit) : base(limit + 1, 0.75f, true)
		  {
			  this.outerInstance = outerInstance;
			  this.limit = limit;
			  serialVersionUID = 1L;
		  }

																			   // 0.75 is the default (see javadocs)
																			   // true will keep the 'access-order', which is needed to have a real LRU cache
		  private static readonly long serialVersionUID;

		  protected internal virtual bool removeEldestEntry(KeyValuePair<string, T> eldest)
		  {
			bool removeEldest = outerInstance.size() > limit;
			if (removeEldest)
			{
			  logger.trace("Cache limit is reached, {} will be evicted", eldest.Key);
			}
			return removeEldest;
		  }

	  }

	  public virtual T get(string id)
	  {
		return cache[id];
	  }

	  public virtual void add(string id, T obj)
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