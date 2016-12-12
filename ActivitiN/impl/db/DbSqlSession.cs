using System;
using System.Collections;
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

namespace org.activiti.engine.impl.db
{


	using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.activiti.engine.impl.context.Context;
	using DbUpgradeStep = org.activiti.engine.impl.db.upgrade.DbUpgradeStep;
	using Session = org.activiti.engine.impl.interceptor.Session;
	using PropertyEntity = org.activiti.engine.impl.persistence.entity.PropertyEntity;
	using IoUtil = org.activiti.engine.impl.util.IoUtil;
	using ReflectUtil = org.activiti.engine.impl.util.ReflectUtil;
	using DeserializedObject = org.activiti.engine.impl.variable.DeserializedObject;
	using SqlSession = org.apache.ibatis.session.SqlSession;
	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// <summary>
	/// responsibilities:
	///   - delayed flushing of inserts updates and deletes
	///   - optional dirty checking
	///   - db specific statement name mapping
	///   
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public class DbSqlSession : Session
	{

	  private static readonly Logger log = LoggerFactory.getLogger(typeof(DbSqlSession));

	  protected internal static readonly Pattern CLEAN_VERSION_REGEX = Pattern.compile("\\d\\.\\d*");

	  protected internal static readonly IList<ActivitiVersion> ACTIVITI_VERSIONS = new List<ActivitiVersion>();
	  static DbSqlSession()
	  {

		  /* Previous */

		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.7"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.8"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.9"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.10"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.11"));

		  // 5.12.1 was a bugfix release on 5.12 and did NOT change the version in ACT_GE_PROPERTY
		  // On top of that, DB2 create script for 5.12.1 was shipped with a 'T' suffix ...
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.12", Arrays.asList("5.12.1", "5.12T")));

		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.13"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.14"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.15"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.15.1"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.16"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.16.1"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.16.2-SNAPSHOT"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.16.2"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.16.3.0"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.16.4.0"));

		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.17.0.0"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.17.0.1"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.17.0.2"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.18.0.0"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.18.0.1"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.20.0.0"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.20.0.1"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.20.0.2"));
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion("5.21.0.0"));

		  /* Current */
		  ACTIVITI_VERSIONS.Add(new ActivitiVersion(org.activiti.engine.ProcessEngine_Fields.VERSION));
	  }

	  protected internal SqlSession sqlSession;
	  protected internal DbSqlSessionFactory dbSqlSessionFactory;
	  protected internal IDictionary<Type, IList<PersistentObject>> insertedObjects = new Dictionary<Type, IList<PersistentObject>>();
	  protected internal IDictionary<Type, IDictionary<string, CachedObject>> cachedObjects = new Dictionary<Type, IDictionary<string, CachedObject>>();
	  protected internal IList<DeleteOperation> deleteOperations = new List<DeleteOperation>();
	  protected internal IList<DeserializedObject> deserializedObjects = new List<DeserializedObject>();
	  protected internal string connectionMetadataDefaultCatalog;
	  protected internal string connectionMetadataDefaultSchema;

	  public DbSqlSession(DbSqlSessionFactory dbSqlSessionFactory)
	  {
		this.dbSqlSessionFactory = dbSqlSessionFactory;
		this.sqlSession = dbSqlSessionFactory.SqlSessionFactory.openSession();
	  }

	  public DbSqlSession(DbSqlSessionFactory dbSqlSessionFactory, Connection connection, string catalog, string schema)
	  {
		this.dbSqlSessionFactory = dbSqlSessionFactory;
		this.sqlSession = dbSqlSessionFactory.SqlSessionFactory.openSession(connection);
		this.connectionMetadataDefaultCatalog = catalog;
		this.connectionMetadataDefaultSchema = schema;
	  }

	  // insert ///////////////////////////////////////////////////////////////////


	  public virtual void insert(PersistentObject persistentObject)
	  {
		if (persistentObject.Id == null)
		{
		  string id = dbSqlSessionFactory.IdGenerator.NextId;
		  persistentObject.Id = id;
		}

		Type clazz = persistentObject.GetType();
		if (!insertedObjects.ContainsKey(clazz))
		{
			insertedObjects[clazz] = new List<PersistentObject>();
		}

		insertedObjects[clazz].Add(persistentObject);
		cachePut(persistentObject, false);
	  }

	  // update ///////////////////////////////////////////////////////////////////

	  public virtual void update(PersistentObject persistentObject)
	  {
		cachePut(persistentObject, false);
	  }

	  public virtual int update(string statement, object parameters)
	  {
		 string updateStatement = dbSqlSessionFactory.mapStatement(statement);
		 return SqlSession.update(updateStatement, parameters);
	  }

	  // delete ///////////////////////////////////////////////////////////////////

	  public virtual void delete(string statement, object parameter)
	  {
		deleteOperations.Add(new BulkDeleteOperation(this, statement, parameter));
	  }

	  public virtual void delete(PersistentObject persistentObject)
	  {
		foreach (DeleteOperation deleteOperation in deleteOperations)
		{
			if (deleteOperation.sameIdentity(persistentObject))
			{
			  log.debug("skipping redundant delete: {}", persistentObject);
			  return; // Skip this delete. It was already added.
			}
		}

		deleteOperations.Add(new CheckedDeleteOperation(this, persistentObject));
	  }

	  public interface DeleteOperation
	  {

		  /// <returns> The persistent object class that is being deleted.
		  ///         Null in case there are multiple objects of different types! </returns>
		  Type PersistentObjectClass {get;}

		bool sameIdentity(PersistentObject other);

		void clearCache();

		void execute();

	  }

	  /// <summary>
	  /// Use this <seealso cref="DeleteOperation"/> to execute a dedicated delete statement.
	  /// It is important to note there won't be any optimistic locking checks done 
	  /// for these kind of delete operations!
	  /// 
	  /// For example, a usage of this operation would be to delete all variables for
	  /// a certain execution, when that certain execution is removed. The optimistic locking
	  /// happens on the execution, but the variables can be removed by a simple
	  /// 'delete from var_table where execution_id is xxx'. It could very well be there
	  /// are no variables, which would also work with this query, but not with the 
	  /// regular <seealso cref="CheckedDeleteOperation"/>. 
	  /// </summary>
	  public class BulkDeleteOperation : DeleteOperation
	  {
		  private readonly DbSqlSession outerInstance;

		internal string statement;
		internal object parameter;

		public BulkDeleteOperation(DbSqlSession outerInstance, string statement, object parameter)
		{
			this.outerInstance = outerInstance;
		  this.statement = outerInstance.dbSqlSessionFactory.mapStatement(statement);
		  this.parameter = parameter;
		}

		public override Type PersistentObjectClass
		{
			get
			{
				return null;
			}
			set
			{
					this.persistentObjectClass = value;
			}
		}

		public override bool sameIdentity(PersistentObject other)
		{
		  // this implementation is unable to determine what the identity of the removed object(s) will be.
		  return false;
		}

		public override void clearCache()
		{
		  // this implementation cannot clear the object(s) to be removed from the cache.
		}

		public override void execute()
		{
		  outerInstance.sqlSession.delete(statement, parameter);
		}

		public override string ToString()
		{
		  return "bulk delete: " + statement + "(" + parameter + ")";
		}
	  }

	  /// <summary>
	  /// A <seealso cref="DeleteOperation"/> that checks for concurrent modifications if the persistent object implements <seealso cref="HasRevision"/>.
	  /// That is, it employs optimisting concurrency control. Used when the persistent object has been fetched already.
	  /// </summary>
	  public class CheckedDeleteOperation : DeleteOperation
	  {
		  private readonly DbSqlSession outerInstance;

		protected internal readonly PersistentObject persistentObject;

		public CheckedDeleteOperation(DbSqlSession outerInstance, PersistentObject persistentObject)
		{
			this.outerInstance = outerInstance;
		  this.persistentObject = persistentObject;
		}

		public override Type PersistentObjectClass
		{
			get
			{
				return persistentObject.GetType();
			}
		}

		public override bool sameIdentity(PersistentObject other)
		{
		  return persistentObject.GetType().Equals(other.GetType()) && persistentObject.Id.Equals(other.Id);
		}

		public override void clearCache()
		{
		  outerInstance.cacheRemove(persistentObject.GetType(), persistentObject.Id);
		}

		public override void execute()
		{
		  string deleteStatement = outerInstance.dbSqlSessionFactory.getDeleteStatement(persistentObject.GetType());
		  deleteStatement = outerInstance.dbSqlSessionFactory.mapStatement(deleteStatement);
		  if (deleteStatement == null)
		  {
			throw new ActivitiException("no delete statement for " + persistentObject.GetType() + " in the ibatis mapping files");
		  }

		  // It only makes sense to check for optimistic locking exceptions for objects that actually have a revision
		  if (persistentObject is HasRevision)
		  {
			int nrOfRowsDeleted = outerInstance.sqlSession.delete(deleteStatement, persistentObject);
			if (nrOfRowsDeleted == 0)
			{
			  throw new ActivitiOptimisticLockingException(persistentObject + " was updated by another transaction concurrently");
			}
		  }
		  else
		  {
			outerInstance.sqlSession.delete(deleteStatement, persistentObject);
		  }
		}

		public virtual PersistentObject PersistentObject
		{
			get
			{
			  return persistentObject;
			}
		}

		public override string ToString()
		{
		  return "delete " + persistentObject;
		}
	  }


	  /// <summary>
	  /// A bulk version of the <seealso cref="CheckedDeleteOperation"/>.
	  /// </summary>
	  public class BulkCheckedDeleteOperation : DeleteOperation
	  {
		  private readonly DbSqlSession outerInstance;


		  protected internal Type persistentObjectClass;
		protected internal IList<PersistentObject> persistentObjects = new List<PersistentObject>();

		public BulkCheckedDeleteOperation(DbSqlSession outerInstance, Type persistentObjectClass)
		{
			this.outerInstance = outerInstance;
			this.persistentObjectClass = persistentObjectClass;
		}

		public virtual void addPersistentObject(PersistentObject persistentObject)
		{
			persistentObjects.Add(persistentObject);
		}

		public override bool sameIdentity(PersistentObject other)
		{
			foreach (PersistentObject persistentObject in persistentObjects)
			{
				if (persistentObject.GetType().Equals(other.GetType()) && persistentObject.Id.Equals(other.Id))
				{
					return true;
				}
			}
			return false;
		}

		public override void clearCache()
		{
			foreach (PersistentObject persistentObject in persistentObjects)
			{
				outerInstance.cacheRemove(persistentObject.GetType(), persistentObject.Id);
			}
		}

		public override void execute()
		{

			if (persistentObjects.Count == 0)
			{
				return;
			}

		  string bulkDeleteStatement = outerInstance.dbSqlSessionFactory.getBulkDeleteStatement(persistentObjectClass);
		  bulkDeleteStatement = outerInstance.dbSqlSessionFactory.mapStatement(bulkDeleteStatement);
		  if (bulkDeleteStatement == null)
		  {
			throw new ActivitiException("no bulk delete statement for " + persistentObjectClass + " in the mapping files");
		  }

		  // It only makes sense to check for optimistic locking exceptions for objects that actually have a revision
		  if (persistentObjects[0] is HasRevision)
		  {
			int nrOfRowsDeleted = outerInstance.sqlSession.delete(bulkDeleteStatement, persistentObjects);
			if (nrOfRowsDeleted < persistentObjects.Count)
			{
			  throw new ActivitiOptimisticLockingException("One of the entities " + persistentObjectClass + " was updated by another transaction concurrently while trying to do a bulk delete");
			}
		  }
		  else
		  {
			outerInstance.sqlSession.delete(bulkDeleteStatement, persistentObjects);
		  }
		}

		public virtual Type PersistentObjectClass
		{
			get
			{
					return persistentObjectClass;
			}
		}


			public virtual IList<PersistentObject> PersistentObjects
			{
				get
				{
					return persistentObjects;
				}
				set
				{
					this.persistentObjects = value;
				}
			}


			public override string ToString()
			{
		  return "bulk delete of " + persistentObjects.Count + (persistentObjects.Count > 0 ? " entities of " + persistentObjects[0].GetType() : 0);
			}
	  }

	  // select ///////////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes" }) public java.util.List selectList(String statement)
	  public virtual IList selectList(string statement)
	  {
		return selectList(statement, null, 0, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public java.util.List selectList(String statement, Object parameter)
	  public virtual IList selectList(string statement, object parameter)
	  {
		return selectList(statement, parameter, 0, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public java.util.List selectList(String statement, Object parameter, org.activiti.engine.impl.Page page)
	  public virtual IList selectList(string statement, object parameter, Page page)
	  {
		if (page != null)
		{
		  return selectList(statement, parameter, page.FirstResult, page.MaxResults);
		}
		else
		{
		  return selectList(statement, parameter, 0, int.MaxValue);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public java.util.List selectList(String statement, ListQueryParameterObject parameter, org.activiti.engine.impl.Page page)
	  public virtual IList selectList(string statement, ListQueryParameterObject parameter, Page page)
	  {
		if (page != null)
		{
		  parameter.FirstResult = page.FirstResult;
		  parameter.MaxResults = page.MaxResults;
		}
		return selectList(statement, parameter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public java.util.List selectList(String statement, Object parameter, int firstResult, int maxResults)
	  public virtual IList selectList(string statement, object parameter, int firstResult, int maxResults)
	  {
		return selectList(statement, new ListQueryParameterObject(parameter, firstResult, maxResults));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public java.util.List selectList(String statement, ListQueryParameterObject parameter)
	  public virtual IList selectList(string statement, ListQueryParameterObject parameter)
	  {
		return selectListWithRawParameter(statement, parameter, parameter.FirstResult, parameter.MaxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes", "unchecked" }) public java.util.List selectListWithRawParameter(String statement, Object parameter, int firstResult, int maxResults)
	  public virtual IList selectListWithRawParameter(string statement, object parameter, int firstResult, int maxResults)
	  {
		statement = dbSqlSessionFactory.mapStatement(statement);
		if (firstResult == -1 || maxResults == -1)
		{
		  return Collections.EMPTY_LIST;
		}
		IList loadedObjects = sqlSession.selectList(statement, parameter);
		return filterLoadedObjects(loadedObjects);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings({ "rawtypes" }) public java.util.List selectListWithRawParameterWithoutFilter(String statement, Object parameter, int firstResult, int maxResults)
	  public virtual IList selectListWithRawParameterWithoutFilter(string statement, object parameter, int firstResult, int maxResults)
	  {
		statement = dbSqlSessionFactory.mapStatement(statement);
		if (firstResult == -1 || maxResults == -1)
		{
		  return Collections.EMPTY_LIST;
		}
		return sqlSession.selectList(statement, parameter);
	  }

	  public virtual object selectOne(string statement, object parameter)
	  {
		statement = dbSqlSessionFactory.mapStatement(statement);
		object result = sqlSession.selectOne(statement, parameter);
		if (result is PersistentObject)
		{
		  PersistentObject loadedObject = (PersistentObject) result;
		  result = cacheFilter(loadedObject);
		}
		return result;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends PersistentObject> T selectById(Class entityClass, String id)
	  public virtual T selectById<T>(Type entityClass, string id) where T : PersistentObject
	  {
		T persistentObject = cacheGet(entityClass, id);
		if (persistentObject != null)
		{
		  return persistentObject;
		}
		string selectStatement = dbSqlSessionFactory.getSelectStatement(entityClass);
		selectStatement = dbSqlSessionFactory.mapStatement(selectStatement);
		persistentObject = (T) sqlSession.selectOne(selectStatement, id);
		if (persistentObject == null)
		{
		  return null;
		}
		cachePut(persistentObject, true);
		return persistentObject;
	  }

	  // internal session cache ///////////////////////////////////////////////////

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") protected java.util.List filterLoadedObjects(java.util.List<Object> loadedObjects)
	  protected internal virtual IList filterLoadedObjects(IList<object> loadedObjects)
	  {
		if (loadedObjects.Count == 0)
		{
		  return loadedObjects;
		}
		if (!(loadedObjects[0] is PersistentObject))
		{
		  return loadedObjects;
		}

		IList<PersistentObject> filteredObjects = new List<PersistentObject>(loadedObjects.Count);
		foreach (object loadedObject in loadedObjects)
		{
		  PersistentObject cachedPersistentObject = cacheFilter((PersistentObject) loadedObject);
		  filteredObjects.Add(cachedPersistentObject);
		}
		return filteredObjects;
	  }

	  protected internal virtual CachedObject cachePut(PersistentObject persistentObject, bool storeState)
	  {
		IDictionary<string, CachedObject> classCache = cachedObjects[persistentObject.GetType()];
		if (classCache == null)
		{
		  classCache = new Dictionary<string, CachedObject>();
		  cachedObjects[persistentObject.GetType()] = classCache;
		}
		CachedObject cachedObject = new CachedObject(persistentObject, storeState);
		classCache[persistentObject.Id] = cachedObject;
		return cachedObject;
	  }

	  /// <summary>
	  /// returns the object in the cache.  if this object was loaded before, 
	  /// then the original object is returned.  if this is the first time 
	  /// this object is loaded, then the loadedObject is added to the cache. 
	  /// </summary>
	  protected internal virtual PersistentObject cacheFilter(PersistentObject persistentObject)
	  {
		PersistentObject cachedPersistentObject = cacheGet(persistentObject.GetType(), persistentObject.Id);
		if (cachedPersistentObject != null)
		{
		  return cachedPersistentObject;
		}
		cachePut(persistentObject, true);
		return persistentObject;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected <T> T cacheGet(Class entityClass, String id)
	  protected internal virtual T cacheGet<T>(Type entityClass, string id)
	  {
		CachedObject cachedObject = null;
		IDictionary<string, CachedObject> classCache = cachedObjects[entityClass];
		if (classCache != null)
		{
		  cachedObject = classCache[id];
		}
		if (cachedObject != null)
		{
		  return (T) cachedObject.PersistentObject;
		}
		return null;
	  }

	  protected internal virtual void cacheRemove(Type persistentObjectClass, string persistentObjectId)
	  {
		IDictionary<string, CachedObject> classCache = cachedObjects[persistentObjectClass];
		if (classCache == null)
		{
		  return;
		}
		classCache.Remove(persistentObjectId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T> java.util.List<T> findInCache(Class entityClass)
	  public virtual IList<T> findInCache<T>(Type entityClass)
	  {
		IDictionary<string, CachedObject> classCache = cachedObjects[entityClass];
		if (classCache != null)
		{
		  IList<T> entities = new List<T>(classCache.Count);
		  foreach (CachedObject cachedObject in classCache.Values)
		  {
			entities.Add((T) cachedObject.PersistentObject);
		  }
		  return entities;
		}
		return Collections.emptyList();
	  }

	  public virtual T findInCache<T>(Type entityClass, string id)
	  {
		return cacheGet(entityClass, id);
	  }

	  public class CachedObject
	  {
		protected internal PersistentObject persistentObject;
		protected internal object persistentObjectState;

		public CachedObject(PersistentObject persistentObject, bool storeState)
		{
		  this.persistentObject = persistentObject;
		  if (storeState)
		  {
			this.persistentObjectState = persistentObject.PersistentState;
		  }
		}

		public virtual PersistentObject PersistentObject
		{
			get
			{
			  return persistentObject;
			}
		}

		public virtual object PersistentObjectState
		{
			get
			{
			  return persistentObjectState;
			}
		}
	  }

	  // deserialized objects /////////////////////////////////////////////////////

	  public virtual void addDeserializedObject(DeserializedObject deserializedObject)
	  {
		  deserializedObjects.Add(deserializedObject);
	  }

	  // flush ////////////////////////////////////////////////////////////////////

	  public override void flush()
	  {
		IList<DeleteOperation> removedOperations = removeUnnecessaryOperations();

		flushDeserializedObjects();
		IList<PersistentObject> updatedObjects = UpdatedObjects;

		if (log.DebugEnabled)
		{
		  ICollection<IList<PersistentObject>> insertedObjectLists = insertedObjects.Values;
		  int nrOfInserts = 0, nrOfUpdates = 0, nrOfDeletes = 0;
		  foreach (IList<PersistentObject> insertedObjectList in insertedObjectLists)
		  {
			  foreach (PersistentObject insertedObject in insertedObjectList)
			  {
				  log.debug("  insert {}", insertedObject);
				  nrOfInserts++;
			  }
		  }
		  foreach (PersistentObject updatedObject in updatedObjects)
		  {
			log.debug("  update {}", updatedObject);
			nrOfUpdates++;
		  }
		  foreach (DeleteOperation deleteOperation in deleteOperations)
		  {
			log.debug("  {}", deleteOperation);
			nrOfDeletes++;
		  }
		  log.debug("flush summary: {} insert, {} update, {} delete.", nrOfInserts, nrOfUpdates, nrOfDeletes);
		  log.debug("now executing flush...");
		}

		flushInserts();
		flushUpdates(updatedObjects);
		flushDeletes(removedOperations);
	  }

	  /// <summary>
	  /// Clears all deleted and inserted objects from the cache, 
	  /// and removes inserts and deletes that cancel each other.
	  /// </summary>
	  protected internal virtual IList<DeleteOperation> removeUnnecessaryOperations()
	  {
		IList<DeleteOperation> removedDeleteOperations = new List<DeleteOperation>();

		for (IEnumerator<DeleteOperation> deleteIterator = deleteOperations.GetEnumerator(); deleteIterator.MoveNext();)
		{

		  DeleteOperation deleteOperation = deleteIterator.Current;
		  Type deletedPersistentObjectClass = deleteOperation.PersistentObjectClass;

		  IList<PersistentObject> insertedObjectsOfSameClass = insertedObjects[deletedPersistentObjectClass];
		  if (insertedObjectsOfSameClass != null && insertedObjectsOfSameClass.Count > 0)
		  {

			  for (IEnumerator<PersistentObject> insertIterator = insertedObjectsOfSameClass.GetEnumerator(); insertIterator.MoveNext();)
			  {
				PersistentObject insertedObject = insertIterator.Current;

				// if the deleted object is inserted,
				if (deleteOperation.sameIdentity(insertedObject))
				{
				  // remove the insert and the delete, they cancel each other
				  insertIterator.remove();
				  deleteIterator.remove();
				  // add removed operations to be able to fire events
				  removedDeleteOperations.Add(deleteOperation);
				}
			  }

			  if (insertedObjects[deletedPersistentObjectClass].Count == 0)
			  {
				  insertedObjects.Remove(deletedPersistentObjectClass);
			  }

		  }

		  // in any case, remove the deleted object from the cache
		  deleteOperation.clearCache();
		}

		foreach (Type persistentObjectClass in insertedObjects.Keys)
		{
			foreach (PersistentObject insertedObject in insertedObjects[persistentObjectClass])
			{
				cacheRemove(insertedObject.GetType(), insertedObject.Id);
			}
		}

		return removedDeleteOperations;
	  }

	//  
	//  [Joram] Put this in comments. Had all kinds of errors.
	//  
	//  /**
	//   * Optimizes the given delete operations:
	//   * for example, if there are two deletes for two different variables, merges this into
	//   * one bulk delete which improves performance
	//   */
	//  protected List<DeleteOperation> optimizeDeleteOperations(List<DeleteOperation> deleteOperations) {
	//  	
	//  	// No optimization possible for 0 or 1 operations
	//  	if (!isOptimizeDeleteOperationsEnabled || deleteOperations.size() <= 1) {
	//  		return deleteOperations;
	//  	}
	//  	
	//  	List<DeleteOperation> optimizedDeleteOperations = new ArrayList<DbSqlSession.DeleteOperation>();
	//  	boolean[] checkedIndices = new boolean[deleteOperations.size()];
	//  	for (int i=0; i<deleteOperations.size(); i++) {
	//  		
	//  		if (checkedIndices[i] == true) {
	//  			continue;
	//  		}
	//  		
	//  		DeleteOperation deleteOperation = deleteOperations.get(i);
	//  		boolean couldOptimize = false;
	//  		if (deleteOperation instanceof CheckedDeleteOperation) {
	//  			
	//  			PersistentObject persistentObject = ((CheckedDeleteOperation) deleteOperation).getPersistentObject();
	//  			if (persistentObject instanceof BulkDeleteable) {
	//				String bulkDeleteStatement = dbSqlSessionFactory.getBulkDeleteStatement(persistentObject.getClass());
	//				bulkDeleteStatement = dbSqlSessionFactory.mapStatement(bulkDeleteStatement);
	//				if (bulkDeleteStatement != null) {
	//					BulkCheckedDeleteOperation bulkCheckedDeleteOperation = null;
	//					
	//					// Find all objects of the same type
	//					for (int j=0; j<deleteOperations.size(); j++) {
	//						DeleteOperation otherDeleteOperation = deleteOperations.get(j);
	//						if (j != i && checkedIndices[j] == false && otherDeleteOperation instanceof CheckedDeleteOperation) {
	//							PersistentObject otherPersistentObject = ((CheckedDeleteOperation) otherDeleteOperation).getPersistentObject();
	//							if (otherPersistentObject.getClass().equals(persistentObject.getClass())) {
	//	  							if (bulkCheckedDeleteOperation == null) {
	//	  								bulkCheckedDeleteOperation = new BulkCheckedDeleteOperation(persistentObject.getClass());
	//	  								bulkCheckedDeleteOperation.addPersistentObject(persistentObject);
	//	  								optimizedDeleteOperations.add(bulkCheckedDeleteOperation);
	//	  							}
	//	  							couldOptimize = true;
	//	  							bulkCheckedDeleteOperation.addPersistentObject(otherPersistentObject);
	//	  							checkedIndices[j] = true;
	//							} else {
	//							    // We may only optimize subsequent delete operations of the same type, to prevent messing up 
	//							    // the order of deletes of related entities which may depend on the referenced entity being deleted before
	//							    break;
	//							}
	//						}
	//						
	//					}
	//				}
	//  			}
	//  		}
	//  		
	//   		if (!couldOptimize) {
	//  			optimizedDeleteOperations.add(deleteOperation);
	//  		}
	//  		checkedIndices[i]=true;
	//  		
	//  	}
	//  	return optimizedDeleteOperations;
	//  }

	  protected internal virtual void flushDeserializedObjects()
	  {
		foreach (DeserializedObject deserializedObject in deserializedObjects)
		{
		  deserializedObject.flush();
		}
	  }

	  public virtual IList<PersistentObject> UpdatedObjects
	  {
		  get
		  {
			IList<PersistentObject> updatedObjects = new List<PersistentObject>();
			foreach (Type clazz in cachedObjects.Keys)
			{
    
			  IDictionary<string, CachedObject> classCache = cachedObjects[clazz];
			  foreach (CachedObject cachedObject in classCache.Values)
			  {
    
				PersistentObject persistentObject = cachedObject.PersistentObject;
				if (!isPersistentObjectDeleted(persistentObject))
				{
				  object originalState = cachedObject.PersistentObjectState;
				  if (persistentObject.PersistentState != null && !persistentObject.PersistentState.Equals(originalState))
				  {
					updatedObjects.Add(persistentObject);
				  }
				  else
				  {
					log.trace("loaded object '{}' was not updated", persistentObject);
				  }
				}
    
			  }
    
			}
			return updatedObjects;
		  }
	  }

	  protected internal virtual bool isPersistentObjectDeleted(PersistentObject persistentObject)
	  {
		foreach (DeleteOperation deleteOperation in deleteOperations)
		{
		  if (deleteOperation.sameIdentity(persistentObject))
		  {
			return true;
		  }
		}
		return false;
	  }

	  public virtual IList<T> pruneDeletedEntities<T>(IList<T> listToPrune) where T : PersistentObject
	  {
		IList<T> prunedList = new List<T>(listToPrune);
		foreach (T potentiallyDeleted in listToPrune)
		{
		  foreach (DeleteOperation deleteOperation in deleteOperations)
		  {

			if (deleteOperation.sameIdentity(potentiallyDeleted))
			{
			  prunedList.Remove(potentiallyDeleted);
			}

		  }
		}
		return prunedList;
	  }

	  protected internal virtual void flushInserts()
	  {

		  // Handle in entity dependency order
		foreach (Type persistentObjectClass in EntityDependencyOrder.INSERT_ORDER)
		{
		  if (insertedObjects.ContainsKey(persistentObjectClass))
		  {
			  flushPersistentObjects(persistentObjectClass, insertedObjects[persistentObjectClass]);
			  insertedObjects.Remove(persistentObjectClass);
		  }
		}

		// Next, in case of custom entities or we've screwed up and forgotten some entity
		if (insertedObjects.Count > 0)
		{
			foreach (Type persistentObjectClass in insertedObjects.Keys)
			{
			  flushPersistentObjects(persistentObjectClass, insertedObjects[persistentObjectClass]);
			}
		}

		insertedObjects.Clear();
	  }

		protected internal virtual void flushPersistentObjects(Type persistentObjectClass, IList<PersistentObject> persistentObjectsToInsert)
		{
		  if (persistentObjectsToInsert.Count == 1)
		  {
			  flushRegularInsert(persistentObjectsToInsert[0], persistentObjectClass);
		  }
		  else if (false.Equals(dbSqlSessionFactory.isBulkInsertable(persistentObjectClass)))
		  {
			  foreach (PersistentObject persistentObject in persistentObjectsToInsert)
			  {
				  flushRegularInsert(persistentObject, persistentObjectClass);
			  }
		  }
		  else
		  {
			  flushBulkInsert(insertedObjects[persistentObjectClass], persistentObjectClass);
		  }
		}

	  protected internal virtual void flushRegularInsert(PersistentObject persistentObject, Type clazz)
	  {
		   string insertStatement = dbSqlSessionFactory.getInsertStatement(persistentObject);
		 insertStatement = dbSqlSessionFactory.mapStatement(insertStatement);

		 if (insertStatement == null)
		 {
		   throw new ActivitiException("no insert statement for " + persistentObject.GetType() + " in the ibatis mapping files");
		 }

		 log.debug("inserting: {}", persistentObject);
		 sqlSession.insert(insertStatement, persistentObject);

		 // See https://activiti.atlassian.net/browse/ACT-1290
		 if (persistentObject is HasRevision)
		 {
		   ((HasRevision) persistentObject).Revision = ((HasRevision) persistentObject).RevisionNext;
		 }
	  }

	  protected internal virtual void flushBulkInsert(IList<PersistentObject> persistentObjectList, Type clazz)
	  {
		string insertStatement = dbSqlSessionFactory.getBulkInsertStatement(clazz);
		insertStatement = dbSqlSessionFactory.mapStatement(insertStatement);

		if (insertStatement == null)
		{
		  throw new ActivitiException("no insert statement for " + persistentObjectList[0].GetType() + " in the ibatis mapping files");
		}

		if (persistentObjectList.Count <= dbSqlSessionFactory.MaxNrOfStatementsInBulkInsert)
		{
		  sqlSession.insert(insertStatement, persistentObjectList);
		}
		else
		{

		  for (int start = 0; start < persistentObjectList.Count; start += dbSqlSessionFactory.MaxNrOfStatementsInBulkInsert)
		  {
			IList<PersistentObject> subList = persistentObjectList.subList(start, Math.Min(start + dbSqlSessionFactory.MaxNrOfStatementsInBulkInsert, persistentObjectList.Count));
			sqlSession.insert(insertStatement, subList);
		  }

		}

		if (persistentObjectList[0] is HasRevision)
		{
		  foreach (PersistentObject insertedObject in persistentObjectList)
		  {
			((HasRevision) insertedObject).Revision = ((HasRevision) insertedObject).RevisionNext;
		  }
		}
	  }

	  protected internal virtual void flushUpdates(IList<PersistentObject> updatedObjects)
	  {
		foreach (PersistentObject updatedObject in updatedObjects)
		{
		  string updateStatement = dbSqlSessionFactory.getUpdateStatement(updatedObject);
		  updateStatement = dbSqlSessionFactory.mapStatement(updateStatement);

		  if (updateStatement == null)
		  {
			throw new ActivitiException("no update statement for " + updatedObject.GetType() + " in the ibatis mapping files");
		  }

		  log.debug("updating: {}", updatedObject);
		  int updatedRecords = sqlSession.update(updateStatement, updatedObject);
		  if (updatedRecords != 1)
		  {
			throw new ActivitiOptimisticLockingException(updatedObject + " was updated by another transaction concurrently");
		  }

		  // See https://activiti.atlassian.net/browse/ACT-1290
		  if (updatedObject is HasRevision)
		  {
			((HasRevision) updatedObject).Revision = ((HasRevision) updatedObject).RevisionNext;
		  }

		}
		updatedObjects.Clear();
	  }

	  protected internal virtual void flushDeletes(IList<DeleteOperation> removedOperations)
	  {
		flushRegularDeletes();
		deleteOperations.Clear();
	  }

	  protected internal virtual void flushRegularDeletes()
	  {
		  foreach (DeleteOperation delete in deleteOperations)
		  {
		  log.debug("executing: {}", delete);
		  delete.execute();
		  }
	  }

	  public override void close()
	  {
		sqlSession.close();
	  }

	  public virtual void commit()
	  {
		sqlSession.commit();
	  }

	  public virtual void rollback()
	  {
		sqlSession.rollback();
	  }

	  // schema operations ////////////////////////////////////////////////////////

	  public virtual void dbSchemaCheckVersion()
	  {
		try
		{
		  string dbVersion = DbVersion;
		  if (!org.activiti.engine.ProcessEngine_Fields.VERSION.Equals(dbVersion))
		  {
			throw new ActivitiWrongDbException(org.activiti.engine.ProcessEngine_Fields.VERSION, dbVersion);
		  }

		  string errorMessage = null;
		  if (!EngineTablePresent)
		  {
			errorMessage = addMissingComponent(errorMessage, "engine");
		  }
		  if (dbSqlSessionFactory.DbHistoryUsed && !HistoryTablePresent)
		  {
			errorMessage = addMissingComponent(errorMessage, "history");
		  }
		  if (dbSqlSessionFactory.DbIdentityUsed && !IdentityTablePresent)
		  {
			errorMessage = addMissingComponent(errorMessage, "identity");
		  }

		  if (errorMessage != null)
		  {
			throw new ActivitiException("Activiti database problem: " + errorMessage);
		  }

		}
		catch (Exception e)
		{
		  if (isMissingTablesException(e))
		  {
			throw new ActivitiException("no activiti tables in db. set <property name=\"databaseSchemaUpdate\" to value=\"true\" or value=\"create-drop\" (use create-drop for testing only!) in bean processEngineConfiguration in activiti.cfg.xml for automatic schema creation", e);
		  }
		  else
		  {
			if (e is Exception)
			{
			  throw (Exception) e;
			}
			else
			{
			  throw new ActivitiException("couldn't get db schema version", e);
			}
		  }
		}

		log.debug("activiti db schema check successful");
	  }

	  protected internal virtual string addMissingComponent(string missingComponents, string component)
	  {
		if (missingComponents == null)
		{
		  return "Tables missing for component(s) " + component;
		}
		return missingComponents + ", " + component;
	  }

	  protected internal virtual string DbVersion
	  {
		  get
		  {
			string selectSchemaVersionStatement = dbSqlSessionFactory.mapStatement("selectDbSchemaVersion");
			return (string) sqlSession.selectOne(selectSchemaVersionStatement);
		  }
	  }

	  public virtual void dbSchemaCreate()
	  {
		if (EngineTablePresent)
		{
		  string dbVersion = DbVersion;
		  if (!org.activiti.engine.ProcessEngine_Fields.VERSION.Equals(dbVersion))
		  {
			throw new ActivitiWrongDbException(org.activiti.engine.ProcessEngine_Fields.VERSION, dbVersion);
		  }
		}
		else
		{
		  dbSchemaCreateEngine();
		}

		if (dbSqlSessionFactory.DbHistoryUsed)
		{
		  dbSchemaCreateHistory();
		}

		if (dbSqlSessionFactory.DbIdentityUsed)
		{
		  dbSchemaCreateIdentity();
		}
	  }

	  protected internal virtual void dbSchemaCreateIdentity()
	  {
		executeMandatorySchemaResource("create", "identity");
	  }

	  protected internal virtual void dbSchemaCreateHistory()
	  {
		executeMandatorySchemaResource("create", "history");
	  }

	  protected internal virtual void dbSchemaCreateEngine()
	  {
		executeMandatorySchemaResource("create", "engine");
	  }

	  public virtual void dbSchemaDrop()
	  {
		executeMandatorySchemaResource("drop", "engine");
		if (dbSqlSessionFactory.DbHistoryUsed)
		{
		  executeMandatorySchemaResource("drop", "history");
		}
		if (dbSqlSessionFactory.DbIdentityUsed)
		{
		  executeMandatorySchemaResource("drop", "identity");
		}
	  }

	  public virtual void dbSchemaPrune()
	  {
		if (HistoryTablePresent && !dbSqlSessionFactory.DbHistoryUsed)
		{
		  executeMandatorySchemaResource("drop", "history");
		}
		if (IdentityTablePresent && dbSqlSessionFactory.DbIdentityUsed)
		{
		  executeMandatorySchemaResource("drop", "identity");
		}
	  }

	  public virtual void executeMandatorySchemaResource(string operation, string component)
	  {
		executeSchemaResource(operation, component, getResourceForDbOperation(operation, operation, component), false);
	  }

	  public static string[] JDBC_METADATA_TABLE_TYPES = new string[] {"TABLE"};

		public virtual string dbSchemaUpdate()
		{

			string feedback = null;
			bool isUpgradeNeeded = false;
			int matchingVersionIndex = -1;

			if (EngineTablePresent)
			{

				PropertyEntity dbVersionProperty = selectById(typeof(PropertyEntity),"schema.version");
				string dbVersion = dbVersionProperty.Value;

				// Determine index in the sequence of Activiti releases
				int index = 0;
				while (matchingVersionIndex < 0 && index < ACTIVITI_VERSIONS.Count)
				{
					if (ACTIVITI_VERSIONS[index].matches(dbVersion))
					{
						matchingVersionIndex = index;
					}
					else
					{
						index++;
					}
				}

				// Exception when no match was found: unknown/unsupported version
				if (matchingVersionIndex < 0)
				{
					throw new ActivitiException("Could not update Activiti database schema: unknown version from database: '" + dbVersion + "'");
				}

				isUpgradeNeeded = (matchingVersionIndex != (ACTIVITI_VERSIONS.Count - 1));

				if (isUpgradeNeeded)
				{
					dbVersionProperty.Value = org.activiti.engine.ProcessEngine_Fields.VERSION;

					PropertyEntity dbHistoryProperty;
					if ("5.0".Equals(dbVersion))
					{
						dbHistoryProperty = new PropertyEntity("schema.history", "create(5.0)");
						insert(dbHistoryProperty);
					}
					else
					{
						dbHistoryProperty = selectById(typeof(PropertyEntity), "schema.history");
					}

					// Set upgrade history
					string dbHistoryValue = dbHistoryProperty.Value + " upgrade(" + dbVersion + "->" + org.activiti.engine.ProcessEngine_Fields.VERSION + ")";
					dbHistoryProperty.Value = dbHistoryValue;

					// Engine upgrade
					dbSchemaUpgrade("engine", matchingVersionIndex);
					feedback = "upgraded Activiti from " + dbVersion + " to " + org.activiti.engine.ProcessEngine_Fields.VERSION;
				}

			}
			else
			{
				dbSchemaCreateEngine();
			}
			if (HistoryTablePresent)
			{
				if (isUpgradeNeeded)
				{
					dbSchemaUpgrade("history", matchingVersionIndex);
				}
			}
			else if (dbSqlSessionFactory.DbHistoryUsed)
			{
				dbSchemaCreateHistory();
			}

		if (IdentityTablePresent)
		{
		  if (isUpgradeNeeded)
		  {
			dbSchemaUpgrade("identity", matchingVersionIndex);
		  }
		}
		else if (dbSqlSessionFactory.DbIdentityUsed)
		{
		  dbSchemaCreateIdentity();
		}

		return feedback;
		}

	  public virtual bool EngineTablePresent
	  {
		  get
		  {
			return isTablePresent("ACT_RU_EXECUTION");
		  }
	  }
	  public virtual bool HistoryTablePresent
	  {
		  get
		  {
			return isTablePresent("ACT_HI_PROCINST");
		  }
	  }
	  public virtual bool IdentityTablePresent
	  {
		  get
		  {
			return isTablePresent("ACT_ID_USER");
		  }
	  }

	  public virtual bool isTablePresent(string tableName)
	  {
		  // ACT-1610: in case the prefix IS the schema itself, we don't add the prefix, since the
		  // check is already aware of the schema
		  if (!dbSqlSessionFactory.TablePrefixIsSchema)
		  {
			  tableName = prependDatabaseTablePrefix(tableName);
		  }

		Connection connection = null;
		try
		{
		  connection = sqlSession.Connection;
		  DatabaseMetaData databaseMetaData = connection.MetaData;
		  ResultSet tables = null;

		  string catalog = this.connectionMetadataDefaultCatalog;
		  if (dbSqlSessionFactory.DatabaseCatalog != null && dbSqlSessionFactory.DatabaseCatalog.Length > 0)
		  {
			catalog = dbSqlSessionFactory.DatabaseCatalog;
		  }

		  string schema = this.connectionMetadataDefaultSchema;
		  if (dbSqlSessionFactory.DatabaseSchema != null && dbSqlSessionFactory.DatabaseSchema.Length > 0)
		  {
			schema = dbSqlSessionFactory.DatabaseSchema;
		  }

		  string databaseType = dbSqlSessionFactory.DatabaseType;

		  if ("postgres".Equals(databaseType))
		  {
			tableName = tableName.ToLower();
		  }

		  try
		  {
			tables = databaseMetaData.getTables(catalog, schema, tableName, JDBC_METADATA_TABLE_TYPES);
			return tables.next();
		  }
		  finally
		  {
			try
			{
			  tables.close();
			}
			catch (Exception e)
			{
			  log.error("Error closing meta data tables", e);
			}
		  }

		}
		catch (Exception e)
		{
		  throw new ActivitiException("couldn't check if tables are already present using metadata: " + e.Message, e);
		}
	  }

	  protected internal virtual bool isUpgradeNeeded(string versionInDatabase)
	  {
		if (org.activiti.engine.ProcessEngine_Fields.VERSION.Equals(versionInDatabase))
		{
		  return false;
		}

		string cleanDbVersion = getCleanVersion(versionInDatabase);
		string[] cleanDbVersionSplitted = cleanDbVersion.Split("\\.", true);
		int dbMajorVersion = Convert.ToInt32(cleanDbVersionSplitted[0]);
		int dbMinorVersion = Convert.ToInt32(cleanDbVersionSplitted[1]);

		string cleanEngineVersion = getCleanVersion(org.activiti.engine.ProcessEngine_Fields.VERSION);
		string[] cleanEngineVersionSplitted = cleanEngineVersion.Split("\\.", true);
		int engineMajorVersion = Convert.ToInt32(cleanEngineVersionSplitted[0]);
		int engineMinorVersion = Convert.ToInt32(cleanEngineVersionSplitted[1]);

		if ((dbMajorVersion > engineMajorVersion) || ((dbMajorVersion <= engineMajorVersion) && (dbMinorVersion > engineMinorVersion)))
		{
		  throw new ActivitiException("Version of activiti database (" + versionInDatabase + ") is more recent than the engine (" + org.activiti.engine.ProcessEngine_Fields.VERSION + ")");
		}
		else if (cleanDbVersion.CompareTo(cleanEngineVersion) == 0)
		{
		  // Versions don't match exactly, possibly snapshot is being used
		  log.warn("Engine-version is the same, but not an exact match: {} vs. {}. Not performing database-upgrade.", versionInDatabase, org.activiti.engine.ProcessEngine_Fields.VERSION);
		  return false;
		}
		return true;
	  }

	  protected internal virtual string getCleanVersion(string versionString)
	  {
		Matcher matcher = CLEAN_VERSION_REGEX.matcher(versionString);
		if (!matcher.find())
		{
		  throw new ActivitiException("Illegal format for version: " + versionString);
		}

		string cleanString = matcher.group();
		try
		{
		  Convert.ToDouble(cleanString); // try to parse it, to see if it is really a number
		  return cleanString;
		}
		catch (NumberFormatException)
		{
		  throw new ActivitiException("Illegal format for version: " + versionString);
		}
	  }

	  protected internal virtual string prependDatabaseTablePrefix(string tableName)
	  {
		return dbSqlSessionFactory.DatabaseTablePrefix + tableName;
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: protected void dbSchemaUpgrade(final String component, final int currentDatabaseVersionsIndex)
	  protected internal virtual void dbSchemaUpgrade(string component, int currentDatabaseVersionsIndex)
	  {
		  ActivitiVersion activitiVersion = ACTIVITI_VERSIONS[currentDatabaseVersionsIndex];
		  string dbVersion = activitiVersion.MainVersion;
		log.info("upgrading activiti {} schema from {} to {}", component, dbVersion, org.activiti.engine.ProcessEngine_Fields.VERSION);

		// Actual execution of schema DDL SQL
		for (int i = currentDatabaseVersionsIndex + 1; i < ACTIVITI_VERSIONS.Count; i++)
		{
			string nextVersion = ACTIVITI_VERSIONS[i].MainVersion;

			// Taking care of -SNAPSHOT version in development
		  if (nextVersion.EndsWith("-SNAPSHOT"))
		  {
			  nextVersion = nextVersion.Substring(0, nextVersion.Length - "-SNAPSHOT".Length);
		  }

		  dbVersion = dbVersion.Replace(".", "");
		  nextVersion = nextVersion.Replace(".", "");
		  log.info("Upgrade needed: {} -> {}. Looking for schema update resource for component '{}'", dbVersion, nextVersion, component);
			executeSchemaResource("upgrade", component, getResourceForDbOperation("upgrade", "upgradestep." + dbVersion + ".to." + nextVersion, component), true);
			dbVersion = nextVersion;
		}
	  }

	  public virtual string getResourceForDbOperation(string directory, string operation, string component)
	  {
		string databaseType = dbSqlSessionFactory.DatabaseType;
		return "org/activiti/db/" + directory + "/activiti." + databaseType + "." + operation + "." + component + ".sql";
	  }

	  public virtual void executeSchemaResource(string operation, string component, string resourceName, bool isOptional)
	  {
		InputStream inputStream = null;
		try
		{
		  inputStream = ReflectUtil.getResourceAsStream(resourceName);
		  if (inputStream == null)
		  {
			if (isOptional)
			{
			  log.info("no schema resource {} for {}", resourceName, operation);
			}
			else
			{
			  throw new ActivitiException("resource '" + resourceName + "' is not available");
			}
		  }
		  else
		  {
			executeSchemaResource(operation, component, resourceName, inputStream);
		  }

		}
		finally
		{
		  IoUtil.closeSilently(inputStream);
		}
	  }

	  private void executeSchemaResource(string operation, string component, string resourceName, InputStream inputStream)
	  {
		log.info("performing {} on {} with resource {}", operation, component, resourceName);
		string sqlStatement = null;
		string exceptionSqlStatement = null;
		try
		{
		  Connection connection = sqlSession.Connection;
		  Exception exception = null;
		  sbyte[] bytes = IoUtil.readInputStream(inputStream, resourceName);
		  string ddlStatements = StringHelperClass.NewString(bytes);
		  string databaseType = dbSqlSessionFactory.DatabaseType;

		  // Special DDL handling for certain databases
		  try
		  {
				if ("mysql".Equals(databaseType))
				{
				  DatabaseMetaData databaseMetaData = connection.MetaData;
				  int majorVersion = databaseMetaData.DatabaseMajorVersion;
				  int minorVersion = databaseMetaData.DatabaseMinorVersion;
				  log.info("Found MySQL: majorVersion=" + majorVersion + " minorVersion=" + minorVersion);

				  // Special care for MySQL < 5.6
				  if (majorVersion <= 5 && minorVersion < 6)
				  {
					ddlStatements = updateDdlForMySqlVersionLowerThan56(ddlStatements);
				  }
				}
		  }
		  catch (Exception e)
		  {
			log.info("Could not get database metadata", e);
		  }

		  BufferedReader reader = new BufferedReader(new StringReader(ddlStatements));
		  string line = readNextTrimmedLine(reader);
		  bool inOraclePlsqlBlock = false;
		  while (line != null)
		  {
			if (line.StartsWith("# "))
			{
			  log.debug(line.Substring(2));

			}
			else if (line.StartsWith("-- "))
			{
			  log.debug(line.Substring(3));

			}
			else if (line.StartsWith("execute java "))
			{
			  string upgradestepClassName = line.Substring(13).Trim();
			  DbUpgradeStep dbUpgradeStep = null;
			  try
			  {
				dbUpgradeStep = (DbUpgradeStep) ReflectUtil.instantiate(upgradestepClassName);
			  }
			  catch (ActivitiException e)
			  {
				throw new ActivitiException("database update java class '" + upgradestepClassName + "' can't be instantiated: " + e.Message, e);
			  }
			  try
			  {
				log.debug("executing upgrade step java class {}", upgradestepClassName);
				dbUpgradeStep.execute(this);
			  }
			  catch (Exception e)
			  {
				throw new ActivitiException("error while executing database update java class '" + upgradestepClassName + "': " + e.Message, e);
			  }

			}
			else if (line.Length > 0)
			{

			  if ("oracle".Equals(databaseType) && line.StartsWith("begin"))
			  {
				inOraclePlsqlBlock = true;
				sqlStatement = addSqlStatementPiece(sqlStatement, line);

			  }
			  else if ((line.EndsWith(";") && inOraclePlsqlBlock == false) || (line.StartsWith("/") && inOraclePlsqlBlock == true))
			  {

				if (inOraclePlsqlBlock)
				{
				  inOraclePlsqlBlock = false;
				}
				else
				{
				  sqlStatement = addSqlStatementPiece(sqlStatement, line.Substring(0, line.Length - 1));
				}

				Statement jdbcStatement = connection.createStatement();
				try
				{
				  // no logging needed as the connection will log it
				  log.debug("SQL: {}", sqlStatement);
				  jdbcStatement.execute(sqlStatement);
				  jdbcStatement.close();
				}
				catch (Exception e)
				{
				  if (exception == null)
				  {
					exception = e;
					exceptionSqlStatement = sqlStatement;
				  }
				  log.error("problem during schema {}, statement {}", operation, sqlStatement, e);
				}
				finally
				{
				  sqlStatement = null;
				}
			  }
			  else
			  {
				sqlStatement = addSqlStatementPiece(sqlStatement, line);
			  }
			}

			line = readNextTrimmedLine(reader);
		  }

		  if (exception != null)
		  {
			throw exception;
		  }

		  log.debug("activiti db schema {} for component {} successful", operation, component);

		}
		catch (Exception e)
		{
		  throw new ActivitiException("couldn't " + operation + " db schema: " + exceptionSqlStatement, e);
		}
	  }

	  /// <summary>
	  /// MySQL is funny when it comes to timestamps and dates.
	  ///  
	  /// More specifically, for a DDL statement like 'MYCOLUMN timestamp(3)':
	  ///   - MySQL 5.6.4+ has support for timestamps/dates with millisecond (or smaller) precision. 
	  ///     The DDL above works and the data in the table will have millisecond precision
	  ///   - MySQL < 5.5.3 allows the DDL statement, but ignores it.
	  ///     The DDL above works but the data won't have millisecond precision
	  ///   - MySQL 5.5.3 < [version] < 5.6.4 gives and exception when using the DDL above.
	  ///   
	  /// Also, the 5.5 and 5.6 branches of MySQL are both actively developed and patched.
	  /// 
	  /// Hence, when doing auto-upgrade/creation of the Activiti tables, the default 
	  /// MySQL DDL file is used and all timestamps/datetimes are converted to not use the 
	  /// millisecond precision by string replacement done in the method below.
	  /// 
	  /// If using the DDL files directly (which is a sane choice in production env.),
	  /// there is a distinction between MySQL version < 5.6.
	  /// </summary>
	  protected internal virtual string updateDdlForMySqlVersionLowerThan56(string ddlStatements)
	  {
		  return ddlStatements.Replace("timestamp(3)", "timestamp").Replace("datetime(3)", "datetime").Replace("TIMESTAMP(3)", "TIMESTAMP").Replace("DATETIME(3)", "DATETIME");
	  }

	  protected internal virtual string addSqlStatementPiece(string sqlStatement, string line)
	  {
		if (sqlStatement == null)
		{
		  return line;
		}
		return sqlStatement + " \n" + line;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected String readNextTrimmedLine(java.io.BufferedReader reader) throws java.io.IOException
	  protected internal virtual string readNextTrimmedLine(BufferedReader reader)
	  {
		string line = reader.readLine();
		if (line != null)
		{
		  line = line.Trim();
		}
		return line;
	  }

	  protected internal virtual bool isMissingTablesException(Exception e)
	  {
		string exceptionMessage = e.Message;
		if (e.Message != null)
		{
		  // Matches message returned from H2
		  if ((exceptionMessage.IndexOf("Table") != -1) && (exceptionMessage.IndexOf("not found") != -1))
		  {
			return true;
		  }

		  // Message returned from MySQL and Oracle
		  if (((exceptionMessage.IndexOf("Table") != -1 || exceptionMessage.IndexOf("table") != -1)) && (exceptionMessage.IndexOf("doesn't exist") != -1))
		  {
			return true;
		  }

		  // Message returned from Postgres
		  if (((exceptionMessage.IndexOf("relation") != -1 || exceptionMessage.IndexOf("table") != -1)) && (exceptionMessage.IndexOf("does not exist") != -1))
		  {
			return true;
		  }
		}
		return false;
	  }

	  public virtual void performSchemaOperationsProcessEngineBuild()
	  {
		string databaseSchemaUpdate = Context.ProcessEngineConfiguration.DatabaseSchemaUpdate;
		if (ProcessEngineConfigurationImpl.DB_SCHEMA_UPDATE_DROP_CREATE.Equals(databaseSchemaUpdate))
		{
		  try
		  {
			dbSchemaDrop();
		  }
		  catch (Exception)
		  {
			// ignore
		  }
		}
		if (ProcessEngineConfiguration.DB_SCHEMA_UPDATE_CREATE_DROP.Equals(databaseSchemaUpdate) || ProcessEngineConfigurationImpl.DB_SCHEMA_UPDATE_DROP_CREATE.Equals(databaseSchemaUpdate) || ProcessEngineConfigurationImpl.DB_SCHEMA_UPDATE_CREATE.Equals(databaseSchemaUpdate))
		{
		  dbSchemaCreate();

		}
		else if (ProcessEngineConfiguration.DB_SCHEMA_UPDATE_FALSE.Equals(databaseSchemaUpdate))
		{
		  dbSchemaCheckVersion();

		}
		else if (ProcessEngineConfiguration.DB_SCHEMA_UPDATE_TRUE.Equals(databaseSchemaUpdate))
		{
		  dbSchemaUpdate();
		}
	  }

	  public virtual void performSchemaOperationsProcessEngineClose()
	  {
		string databaseSchemaUpdate = Context.ProcessEngineConfiguration.DatabaseSchemaUpdate;
		if (ProcessEngineConfiguration.DB_SCHEMA_UPDATE_CREATE_DROP.Equals(databaseSchemaUpdate))
		{
		  dbSchemaDrop();
		}
	  }

	  public virtual T getCustomMapper<T>(Type type)
	  {
		  return sqlSession.getMapper(type);
	  }

	  // query factory methods ////////////////////////////////////////////////////  

	  public virtual DeploymentQueryImpl createDeploymentQuery()
	  {
		return new DeploymentQueryImpl();
	  }
	  public virtual ModelQueryImpl createModelQueryImpl()
	  {
		return new ModelQueryImpl();
	  }
	  public virtual ProcessDefinitionQueryImpl createProcessDefinitionQuery()
	  {
		return new ProcessDefinitionQueryImpl();
	  }
	  public virtual ProcessInstanceQueryImpl createProcessInstanceQuery()
	  {
		return new ProcessInstanceQueryImpl();
	  }
	  public virtual ExecutionQueryImpl createExecutionQuery()
	  {
		return new ExecutionQueryImpl();
	  }
	  public virtual TaskQueryImpl createTaskQuery()
	  {
		return new TaskQueryImpl();
	  }
	  public virtual JobQueryImpl createJobQuery()
	  {
		return new JobQueryImpl();
	  }
	  public virtual HistoricProcessInstanceQueryImpl createHistoricProcessInstanceQuery()
	  {
		return new HistoricProcessInstanceQueryImpl();
	  }
	  public virtual HistoricActivityInstanceQueryImpl createHistoricActivityInstanceQuery()
	  {
		return new HistoricActivityInstanceQueryImpl();
	  }
	  public virtual HistoricTaskInstanceQueryImpl createHistoricTaskInstanceQuery()
	  {
		return new HistoricTaskInstanceQueryImpl();
	  }
	  public virtual HistoricDetailQueryImpl createHistoricDetailQuery()
	  {
		return new HistoricDetailQueryImpl();
	  }
	  public virtual HistoricVariableInstanceQueryImpl createHistoricVariableInstanceQuery()
	  {
		return new HistoricVariableInstanceQueryImpl();
	  }
	  public virtual UserQueryImpl createUserQuery()
	  {
		return new UserQueryImpl();
	  }
	  public virtual GroupQueryImpl createGroupQuery()
	  {
		return new GroupQueryImpl();
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual SqlSession SqlSession
	  {
		  get
		  {
			return sqlSession;
		  }
	  }
	  public virtual DbSqlSessionFactory DbSqlSessionFactory
	  {
		  get
		  {
			return dbSqlSessionFactory;
		  }
	  }

	}

}