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
namespace org.activiti.engine.impl.persistence.entity
{


	using VariableScope = org.activiti.engine.@delegate.VariableScope;
	using Context = org.activiti.engine.impl.context.Context;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ELContext = org.activiti.engine.impl.javax.el.ELContext;
	using VariableType = org.activiti.engine.impl.variable.VariableType;
	using VariableTypes = org.activiti.engine.impl.variable.VariableTypes;



	/// <summary>
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// @author Tijs Rademakers
	/// @author Saeid Mirzaei
	/// </summary>
	[Serializable]
	public abstract class VariableScopeImpl : VariableScope
	{

	  private const long serialVersionUID = 1L;

	  // The cache used when fetching all variables
	  protected internal IDictionary<string, VariableInstanceEntity> variableInstances = null; // needs to be null, the logic depends on it for checking if vars were already fetched

	  // The cache is used when fetching/setting specific variables
	  protected internal IDictionary<string, VariableInstanceEntity> usedVariablesCache = new Dictionary<string, VariableInstanceEntity>();

	  protected internal ELContext cachedElContext;

	  protected internal string id = null;

	  protected internal abstract IList<VariableInstanceEntity> loadVariableInstances();
	  protected internal abstract VariableScopeImpl ParentVariableScope {get;}
	  protected internal abstract void initializeVariableInstanceBackPointer(VariableInstanceEntity variableInstance);

	  protected internal virtual void ensureVariableInstancesInitialized()
	  {
		if (variableInstances == null)
		{
		  variableInstances = new Dictionary<string, VariableInstanceEntity>();

		  CommandContext commandContext = Context.CommandContext;
		  if (commandContext == null)
		  {
			throw new ActivitiException("lazy loading outside command context");
		  }
		  IList<VariableInstanceEntity> variableInstancesList = loadVariableInstances();
		  foreach (VariableInstanceEntity variableInstance in variableInstancesList)
		  {
			variableInstances[variableInstance.Name] = variableInstance;
		  }
		}
	  }

	  public virtual IDictionary<string, object> Variables
	  {
		  get
		  {
			return collectVariables(new Dictionary<string, object>());
		  }
		  set
		  {
			if (value != null)
			{
			  foreach (string variableName in value.Keys)
			  {
				setVariable(variableName, value[variableName]);
			  }
			}
		  }
	  }

	  public virtual IDictionary<string, VariableInstance> VariableInstances
	  {
		  get
		  {
			return collectVariableInstances(new Dictionary<string, VariableInstance>());
		  }
	  }

	  public virtual IDictionary<string, object> getVariables(ICollection<string> variableNames)
	  {
		  return getVariables(variableNames, true);
	  }

	  public virtual IDictionary<string, VariableInstance> getVariableInstances(ICollection<string> variableNames)
	  {
		return getVariableInstances(variableNames, true);
	  }

	  public virtual IDictionary<string, object> getVariables(ICollection<string> variableNames, bool fetchAllVariables)
	  {

		  IDictionary<string, object> requestedVariables = new Dictionary<string, object>();
		  Set<string> variableNamesToFetch = new HashSet<string>(variableNames);

		  // The values in the fetch-cache will be more recent, so they can override any existing ones
		  foreach (string variableName in variableNames)
		  {
			  if (usedVariablesCache.ContainsKey(variableName))
			  {
				  requestedVariables[variableName] = usedVariablesCache[variableName].Value;
				  variableNamesToFetch.remove(variableName);
			  }
		  }

		  if (fetchAllVariables == true)
		  {

			  // getVariables() will go up the execution hierarchy, no need to do it here
			  // also, the cached values will already be applied too 
			IDictionary<string, object> allVariables = Variables;
			foreach (string variableName in variableNamesToFetch)
			{
				requestedVariables[variableName] = allVariables[variableName];
			}
			return requestedVariables;

		  }
		  else
		  {

			  // Fetch variables on this scope
			  IList<VariableInstanceEntity> variables = getSpecificVariables(variableNamesToFetch);
			  foreach (VariableInstanceEntity variable in variables)
			  {
				  requestedVariables[variable.Name] = variable.Value;
			  }

			// Go up if needed
			VariableScopeImpl parent = ParentVariableScope;
			if (parent != null)
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
				requestedVariables.putAll(parent.getVariables(variableNamesToFetch, fetchAllVariables));
			}

			  return requestedVariables;

		  }

	  }

	  public virtual IDictionary<string, VariableInstance> getVariableInstances(ICollection<string> variableNames, bool fetchAllVariables)
	  {

		IDictionary<string, VariableInstance> requestedVariables = new Dictionary<string, VariableInstance>();
		Set<string> variableNamesToFetch = new HashSet<string>(variableNames);

		// The values in the fetch-cache will be more recent, so they can override any existing ones
		foreach (string variableName in variableNames)
		{
		  if (usedVariablesCache.ContainsKey(variableName))
		  {
			requestedVariables[variableName] = usedVariablesCache[variableName];
			variableNamesToFetch.remove(variableName);
		  }
		}

		if (fetchAllVariables == true)
		{

		  // getVariables() will go up the execution hierarchy, no need to do it here
		  // also, the cached values will already be applied too 
		  IDictionary<string, VariableInstance> allVariables = VariableInstances;
		  foreach (string variableName in variableNamesToFetch)
		  {
			requestedVariables[variableName] = allVariables[variableName];
		  }
		  return requestedVariables;

		}
		else
		{

		  // Fetch variables on this scope
		  IList<VariableInstanceEntity> variables = getSpecificVariables(variableNamesToFetch);
		  foreach (VariableInstanceEntity variable in variables)
		  {
			requestedVariables[variable.Name] = variable;
		  }

		  // Go up if needed
		  VariableScopeImpl parent = ParentVariableScope;
		  if (parent != null)
		  {
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
			requestedVariables.putAll(parent.getVariableInstances(variableNamesToFetch, fetchAllVariables));
		  }

		  return requestedVariables;

		}

	  }

	  protected internal virtual IDictionary<string, object> collectVariables(Dictionary<string, object> variables)
	  {
		ensureVariableInstancesInitialized();
		VariableScopeImpl parentScope = ParentVariableScope;
		if (parentScope != null)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		  variables.putAll(parentScope.collectVariables(variables));
		}

		foreach (VariableInstanceEntity variableInstance in variableInstances.Values)
		{
		  variables[variableInstance.Name] = variableInstance.Value;
		}

		foreach (string variableName in usedVariablesCache.Keys)
		{
			variables[variableName] = usedVariablesCache[variableName].Value;
		}

		return variables;
	  }

	  protected internal virtual IDictionary<string, VariableInstance> collectVariableInstances(Dictionary<string, VariableInstance> variables)
	  {
		ensureVariableInstancesInitialized();
		VariableScopeImpl parentScope = ParentVariableScope;
		if (parentScope != null)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		  variables.putAll(parentScope.collectVariableInstances(variables));
		}

		foreach (VariableInstance variableInstance in variableInstances.Values)
		{
		  variables[variableInstance.Name] = variableInstance;
		}

		foreach (string variableName in usedVariablesCache.Keys)
		{
		  variables[variableName] = usedVariablesCache[variableName];
		}

		return variables;
	  }

	  public virtual object getVariable(string variableName)
	  {
		return getVariable(variableName, true);
	  }

	  public virtual VariableInstance getVariableInstance(string variableName)
	  {
		return getVariableInstance(variableName, true);
	  }

	  /// <summary>
	  /// The same operation as <seealso cref="VariableScopeImpl#getVariable(String)"/>, but with
	  /// an extra parameter to indicate whether or not all variables need to be fetched.
	  /// 
	  /// Note that the default Activiti way (because of backwards compatibility) is to 
	  /// fetch all the variables when doing a get/set of variables. So this means 'true'
	  /// is the default value for this method, and in fact it will simply delegate to <seealso cref="#getVariable(String)"/>.
	  /// This can also be the most performant, if you're doing a lot of 
	  /// variable gets in the same transaction (eg in service tasks).
	  /// 
	  /// In case 'false' is used, only the specific variable will be fetched.
	  /// </summary>
	  public virtual object getVariable(string variableName, bool fetchAllVariables)
	  {
		object value = null;
		VariableInstance variable = getVariableInstance(variableName, fetchAllVariables);
		if (variable != null)
		{
		  value = variable.Value;
		}
		return value;
	  }

	  public virtual VariableInstance getVariableInstance(string variableName, bool fetchAllVariables)
	  {
		if (fetchAllVariables == true)
		{

		   // Check the local single-fetch cache
		  if (usedVariablesCache.ContainsKey(variableName))
		  {
			return usedVariablesCache[variableName];
		  }

		  ensureVariableInstancesInitialized();
		  VariableInstanceEntity variableInstance = variableInstances[variableName];
		  if (variableInstance != null)
		  {
			return variableInstance;
		  }

		  // Go up the hierarchy
		  VariableScope parentScope = ParentVariableScope;
		  if (parentScope != null)
		  {
			return parentScope.getVariableInstance(variableName, true);
		  }

		  return null;

		}
		else
		{

		  if (usedVariablesCache.ContainsKey(variableName))
		  {
			return usedVariablesCache[variableName];
		  }

		  if (variableInstances != null && variableInstances.ContainsKey(variableName))
		  {
			return variableInstances[variableName];
		  }

		  VariableInstanceEntity variable = getSpecificVariable(variableName);
		  if (variable != null)
		  {
			usedVariablesCache[variableName] = variable;
			return variable;
		  }

		  // Go up the hierarchy
		  VariableScope parentScope = ParentVariableScope;
		  if (parentScope != null)
		  {
			return parentScope.getVariableInstance(variableName, false);
		  }

		  return null;

		}
	  }

	  protected internal abstract VariableInstanceEntity getSpecificVariable(string variableName);

	  public virtual object getVariableLocal(string variableName)
	  {
		  return getVariableLocal(variableName, true);
	  }

	  public virtual VariableInstance getVariableInstanceLocal(string variableName)
	  {
		return getVariableInstanceLocal(variableName, true);
	  }

	  public virtual object getVariableLocal(string variableName, bool fetchAllVariables)
	  {
		object value = null;
		VariableInstance variable = getVariableInstanceLocal(variableName, fetchAllVariables);
		if (variable != null)
		{
		  value = variable.Value;
		}
		return value;
	  }

	  public virtual VariableInstance getVariableInstanceLocal(string variableName, bool fetchAllVariables)
	  {
		if (fetchAllVariables == true)
		{

		  if (usedVariablesCache.ContainsKey(variableName))
		  {
			return usedVariablesCache[variableName];
		  }

		  ensureVariableInstancesInitialized();

		  VariableInstanceEntity variableInstance = variableInstances[variableName];
		  if (variableInstance != null)
		  {
			return variableInstance;
		  }
		  return null;

		}
		else
		{

		  if (usedVariablesCache.ContainsKey(variableName))
		  {
			VariableInstanceEntity variable = usedVariablesCache[variableName];
			if (variable != null)
			{
			  return variable;
			}
		  }

		  if (variableInstances != null && variableInstances.ContainsKey(variableName))
		  {
			VariableInstanceEntity variable = variableInstances[variableName];
			if (variable != null)
			{
			  return variableInstances[variableName];
			}
		  }

		  VariableInstanceEntity variable = getSpecificVariable(variableName);
		  if (variable != null)
		  {
			usedVariablesCache[variableName] = variable;
			return variable;
		  }

		  return null;
		}
	  }

	  public virtual bool hasVariables()
	  {
		ensureVariableInstancesInitialized();
		if (variableInstances.Count > 0)
		{
		  return true;
		}
		VariableScope parentScope = ParentVariableScope;
		if (parentScope != null)
		{
		  return parentScope.hasVariables();
		}
		return false;
	  }

	  public virtual bool hasVariablesLocal()
	  {
		ensureVariableInstancesInitialized();
		return variableInstances.Count > 0;
	  }

	  public virtual bool hasVariable(string variableName)
	  {
		if (hasVariableLocal(variableName))
		{
		  return true;
		}
		VariableScope parentScope = ParentVariableScope;
		if (parentScope != null)
		{
		  return parentScope.hasVariable(variableName);
		}
		return false;
	  }

	  public virtual bool hasVariableLocal(string variableName)
	  {
		ensureVariableInstancesInitialized();
		return variableInstances.ContainsKey(variableName);
	  }

	  protected internal virtual Set<string> collectVariableNames(Set<string> variableNames)
	  {
		ensureVariableInstancesInitialized();
		VariableScopeImpl parentScope = ParentVariableScope;
		if (parentScope != null)
		{
		  variableNames.addAll(parentScope.collectVariableNames(variableNames));
		}
		foreach (VariableInstanceEntity variableInstance in variableInstances.Values)
		{
		  variableNames.add(variableInstance.Name);
		}
		return variableNames;
	  }

	  public virtual Set<string> VariableNames
	  {
		  get
		  {
			return collectVariableNames(new HashSet<string>());
		  }
	  }

	  public virtual IDictionary<string, object> VariablesLocal
	  {
		  get
		  {
			IDictionary<string, object> variables = new Dictionary<string, object>();
			ensureVariableInstancesInitialized();
			foreach (VariableInstanceEntity variableInstance in variableInstances.Values)
			{
			  variables[variableInstance.Name] = variableInstance.Value;
			}
			foreach (string variableName in usedVariablesCache.Keys)
			{
				variables[variableName] = usedVariablesCache[variableName].Value;
			}
			return variables;
		  }
		  set
		  {
			if (value != null)
			{
			  foreach (string variableName in value.Keys)
			  {
				setVariableLocal(variableName, value[variableName]);
			  }
			}
		  }
	  }

	  public virtual IDictionary<string, VariableInstance> VariableInstancesLocal
	  {
		  get
		  {
			IDictionary<string, VariableInstance> variables = new Dictionary<string, VariableInstance>();
			ensureVariableInstancesInitialized();
			foreach (VariableInstanceEntity variableInstance in variableInstances.Values)
			{
			  variables[variableInstance.Name] = variableInstance;
			}
			foreach (string variableName in usedVariablesCache.Keys)
			{
			  variables[variableName] = usedVariablesCache[variableName];
			}
			return variables;
		  }
	  }

	  public virtual IDictionary<string, object> getVariablesLocal(ICollection<string> variableNames)
	  {
		  return getVariablesLocal(variableNames, true);
	  }

	  public virtual IDictionary<string, VariableInstance> getVariableInstancesLocal(ICollection<string> variableNames)
	  {
		return getVariableInstancesLocal(variableNames, true);
	  }

	  public virtual IDictionary<string, object> getVariablesLocal(ICollection<string> variableNames, bool fetchAllVariables)
	  {
		  IDictionary<string, object> requestedVariables = new Dictionary<string, object>();

		  // The values in the fetch-cache will be more recent, so they can override any existing ones
		  Set<string> variableNamesToFetch = new HashSet<string>(variableNames);
		  foreach (string variableName in variableNames)
		  {
			  if (usedVariablesCache.ContainsKey(variableName))
			  {
				  requestedVariables[variableName] = usedVariablesCache[variableName].Value;
				  variableNamesToFetch.remove(variableName);
			  }
		  }

		  if (fetchAllVariables == true)
		  {

			IDictionary<string, object> allVariables = VariablesLocal;
			foreach (string variableName in variableNamesToFetch)
			{
				requestedVariables[variableName] = allVariables[variableName];
			}

		  }
		  else
		  {

			  IList<VariableInstanceEntity> variables = getSpecificVariables(variableNamesToFetch);
			  foreach (VariableInstanceEntity variable in variables)
			  {
				  requestedVariables[variable.Name] = variable.Value;
			  }

		  }

		  return requestedVariables;
	  }

	  public virtual IDictionary<string, VariableInstance> getVariableInstancesLocal(ICollection<string> variableNames, bool fetchAllVariables)
	  {
		IDictionary<string, VariableInstance> requestedVariables = new Dictionary<string, VariableInstance>();

		// The values in the fetch-cache will be more recent, so they can override any existing ones
		Set<string> variableNamesToFetch = new HashSet<string>(variableNames);
		foreach (string variableName in variableNames)
		{
		  if (usedVariablesCache.ContainsKey(variableName))
		  {
			requestedVariables[variableName] = usedVariablesCache[variableName];
			variableNamesToFetch.remove(variableName);
		  }
		}

		if (fetchAllVariables == true)
		{

		  IDictionary<string, VariableInstance> allVariables = VariableInstancesLocal;
		  foreach (string variableName in variableNamesToFetch)
		  {
			requestedVariables[variableName] = allVariables[variableName];
		  }

		}
		else
		{

		  IList<VariableInstanceEntity> variables = getSpecificVariables(variableNamesToFetch);
		  foreach (VariableInstanceEntity variable in variables)
		  {
			requestedVariables[variable.Name] = variable;
		  }

		}

		return requestedVariables;
	  }

	  protected internal abstract IList<VariableInstanceEntity> getSpecificVariables(ICollection<string> variableNames);

	  public virtual Set<string> VariableNamesLocal
	  {
		  get
		  {
			ensureVariableInstancesInitialized();
			return variableInstances.Keys;
		  }
	  }

	  public virtual IDictionary<string, VariableInstanceEntity> VariableInstanceEntities
	  {
		  get
		  {
			ensureVariableInstancesInitialized();
			return Collections.unmodifiableMap(variableInstances);
		  }
	  }

	  public virtual IDictionary<string, object> VariableValues
	  {
		  get
		  {
			IDictionary<string, object> variableMap = new Dictionary<string, object>();
			if (variableInstances != null)
			{
			  foreach (string varName in variableInstances.Keys)
			  {
				VariableInstanceEntity variableEntity = variableInstances[varName];
				if (variableEntity != null)
				{
				  variableMap[varName] = variableEntity.Value;
				}
				else
				{
				  variableMap[varName] = null;
				}
			  }
			}
			return variableMap;
		  }
	  }

	  public virtual IDictionary<string, VariableInstanceEntity> UsedVariablesCache
	  {
		  get
		  {
			return usedVariablesCache;
		  }
	  }
	  public virtual void createVariablesLocal<T1>(IDictionary<T1> variables) where T1 : Object
	  {
		if (variables != null)
		{
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (java.util.Map.Entry<String, ? extends Object> entry: variables.entrySet())
		  foreach (KeyValuePair<string, ?> entry in variables)
		  {
			createVariableLocal(entry.Key, entry.Value);
		  }
		}
	  }



	  public virtual void removeVariables()
	  {
		ensureVariableInstancesInitialized();
		IDictionary<string, VariableInstanceEntity>.KeyCollection variableNames = new HashSet<string>(variableInstances.Keys);
		foreach (string variableName in variableNames)
		{
		  removeVariable(variableName);
		}
	  }

	  public virtual void removeVariablesLocal()
	  {
		IList<string> variableNames = new List<string>(VariableNamesLocal);
		foreach (string variableName in variableNames)
		{
		  removeVariableLocal(variableName);
		}
	  }

	  public virtual void deleteVariablesInstanceForLeavingScope()
	  {
		ensureVariableInstancesInitialized();

		foreach (VariableInstanceEntity variableInstance in variableInstances.Values)
		{
			Context.CommandContext.HistoryManager.recordVariableUpdate(variableInstance);

			variableInstance.delete();
		}
	  }

	  public virtual void removeVariables(ICollection<string> variableNames)
	  {
		if (variableNames != null)
		{
		  foreach (string variableName in variableNames)
		  {
			removeVariable(variableName);
		  }
		}
	  }

	  public virtual void removeVariablesLocal(ICollection<string> variableNames)
	  {
		if (variableNames != null)
		{
		  foreach (string variableName in variableNames)
		  {
			removeVariableLocal(variableName);
		  }
		}
	  }

	  public virtual void setVariable(string variableName, object value)
	  {
		setVariable(variableName, value, SourceActivityExecution, true);
	  }

	  /// <summary>
	  /// The default <seealso cref="#setVariable(String, Object)"/> fetches all variables 
	  /// (for historical and backwards compatible reasons) while setting the variables.
	  /// 
	  /// Setting the fetchAllVariables parameter to true is the default behaviour (ie fetching all variables) 
	  /// Setting the fetchAllVariables parameter to false does not do that. 
	  /// 
	  /// </summary>
	  public virtual void setVariable(string variableName, object value, bool fetchAllVariables)
	  {
		setVariable(variableName, value, SourceActivityExecution, fetchAllVariables);
	  }

	  protected internal virtual void setVariable(string variableName, object value, ExecutionEntity sourceActivityExecution, bool fetchAllVariables)
	  {

		  if (fetchAllVariables == true)
		  {

			  // If it's in the cache, it's more recent
			  if (usedVariablesCache.ContainsKey(variableName))
			  {
				  updateVariableInstance(usedVariablesCache[variableName], value, sourceActivityExecution);
			  }

			  // If the variable exists on this scope, replace it
			if (hasVariableLocal(variableName))
			{
			  setVariableLocal(variableName, value, sourceActivityExecution, true);
			  return;
			}

			// Otherwise, go up the hierarchy (we're trying to put it as high as possible)
			VariableScopeImpl parentVariableScope = ParentVariableScope;
			if (parentVariableScope != null)
			{
			  if (sourceActivityExecution == null)
			  {
				parentVariableScope.setVariable(variableName, value);
			  }
			  else
			  {
				parentVariableScope.setVariable(variableName, value, sourceActivityExecution, true);
			  }
			  return;
			}

			// We're as high as possible and the variable doesn't exist yet, so we're creating it
			createVariableLocal(variableName, value);

		  }
		  else
		  {

			  // Check local cache first
			  if (usedVariablesCache.ContainsKey(variableName))
			  {

				  updateVariableInstance(usedVariablesCache[variableName], value, sourceActivityExecution);

			  }
			  else if (variableInstances != null && variableInstances.ContainsKey(variableName))
			  {

				  updateVariableInstance(variableInstances[variableName], value, sourceActivityExecution);

			  }
			  else
			  {

				  // Not in local cache, check if defined on this scope
				  // Create it if it doesn't exist yet
				  VariableInstanceEntity variable = getSpecificVariable(variableName);
				  if (variable != null)
				  {
					  updateVariableInstance(variable, value, sourceActivityExecution);
					  usedVariablesCache[variableName] = variable;
				  }
				  else
				  {

					  VariableScopeImpl parent = ParentVariableScope;
			  if (parent != null)
			  {
				if (sourceActivityExecution == null)
				{
				  parent.setVariable(variableName, value, fetchAllVariables);
				}
				else
				{
				  parent.setVariable(variableName, value, sourceActivityExecution, fetchAllVariables);
				}
				return;
			  }

					  variable = createVariableInstance(variableName, value, sourceActivityExecution);
					  usedVariablesCache[variableName] = variable;

				  }

			  }

		  }

	  }


	  public virtual object setVariableLocal(string variableName, object value)
	  {
		return setVariableLocal(variableName, value, SourceActivityExecution, true);
	  }

	  /// <summary>
	  /// The default <seealso cref="#setVariableLocal(String, Object)"/> fetches all variables 
	  /// (for historical and backwards compatible reasons) while setting the variables.
	  /// 
	  /// Setting the fetchAllVariables parameter to true is the default behaviour (ie fetching all variables) 
	  /// Setting the fetchAllVariables parameter to false does not do that. 
	  /// 
	  /// </summary>
	  public virtual object setVariableLocal(string variableName, object value, bool fetchAllVariables)
	  {
		return setVariableLocal(variableName, value, SourceActivityExecution, fetchAllVariables);
	  }

	  public virtual object setVariableLocal(string variableName, object value, ExecutionEntity sourceActivityExecution, bool fetchAllVariables)
	  {

		  if (fetchAllVariables == true)
		  {

			// If it's in the cache, it's more recent
			  if (usedVariablesCache.ContainsKey(variableName))
			  {
				  updateVariableInstance(usedVariablesCache[variableName], value, sourceActivityExecution);
			  }

			  ensureVariableInstancesInitialized();

			VariableInstanceEntity variableInstance = variableInstances[variableName];
			if (variableInstance == null)
			{
				variableInstance = usedVariablesCache[variableName];
			}

			if (variableInstance == null)
			{
				createVariableLocal(variableName, value);
			}
			else
			{
			  updateVariableInstance(variableInstance, value, sourceActivityExecution);
			}

			return null;

		  }
		  else
		  {

			  if (usedVariablesCache.ContainsKey(variableName))
			  {
				  updateVariableInstance(usedVariablesCache[variableName], value, sourceActivityExecution);
			  }
			  else if (variableInstances != null && variableInstances.ContainsKey(variableName))
			  {
					updateVariableInstance(variableInstances[variableName], value, sourceActivityExecution);
			  }
				else
				{

					VariableInstanceEntity variable = getSpecificVariable(variableName);
					if (variable != null)
					{
						updateVariableInstance(variable, value, sourceActivityExecution);
					}
					else
					{
						variable = createVariableInstance(variableName, value, sourceActivityExecution);
					}
					usedVariablesCache[variableName] = variable;

				}

				return null;

		  }
	  }

	  public virtual void createVariableLocal(string variableName, object value)
	  {
		createVariableLocal(variableName, value, SourceActivityExecution);
	  }

	  /// <summary>
	  /// only called when a new variable is created on this variable scope.
	  /// This method is also responsible for propagating the creation of this 
	  /// variable to the history. 
	  /// </summary>
	  protected internal virtual void createVariableLocal(string variableName, object value, ExecutionEntity sourceActivityExecution)
	  {
		ensureVariableInstancesInitialized();

		if (variableInstances.ContainsKey(variableName))
		{
		  throw new ActivitiException("variable '" + variableName + "' already exists. Use setVariableLocal if you want to overwrite the value");
		}

		createVariableInstance(variableName, value, sourceActivityExecution);
	  }

	  public virtual void removeVariable(string variableName)
	  {
		removeVariable(variableName, SourceActivityExecution);
	  }

	  protected internal virtual void removeVariable(string variableName, ExecutionEntity sourceActivityExecution)
	  {
		ensureVariableInstancesInitialized();
		if (variableInstances.ContainsKey(variableName))
		{
		  removeVariableLocal(variableName);
		  return;
		}
		VariableScopeImpl parentVariableScope = ParentVariableScope;
		if (parentVariableScope != null)
		{
		  if (sourceActivityExecution == null)
		  {
			parentVariableScope.removeVariable(variableName);
		  }
		  else
		  {
			parentVariableScope.removeVariable(variableName, sourceActivityExecution);
		  }
		}
	  }

	  public virtual void removeVariableLocal(string variableName)
	  {
		removeVariableLocal(variableName, SourceActivityExecution);
	  }

	  protected internal virtual ExecutionEntity SourceActivityExecution
	  {
		  get
		  {
			return null;
		  }
	  }

	  protected internal virtual void removeVariableLocal(string variableName, ExecutionEntity sourceActivityExecution)
	  {
		ensureVariableInstancesInitialized();
		VariableInstanceEntity variableInstance = variableInstances.Remove(variableName);
		if (variableInstance != null)
		{
		  deleteVariableInstanceForExplicitUserCall(variableInstance, sourceActivityExecution);
		}
	  }

	  protected internal virtual void deleteVariableInstanceForExplicitUserCall(VariableInstanceEntity variableInstance, ExecutionEntity sourceActivityExecution)
	  {
		variableInstance.delete();
		variableInstance.Value = null;

		// Record historic variable deletion
		Context.CommandContext.HistoryManager.recordVariableRemoved(variableInstance);

		// Record historic detail
		Context.CommandContext.HistoryManager.recordHistoricDetailVariableCreate(variableInstance, sourceActivityExecution, ActivityIdUsedForDetails);
	  }

	  protected internal virtual void updateVariableInstance(VariableInstanceEntity variableInstance, object value, ExecutionEntity sourceActivityExecution)
	  {

		// Always check if the type should be altered. It's possible that the previous type is lower in the type
		// checking chain (e.g. serializable) and will return true on isAbleToStore(), even though another type
		// higher in the chain is eligible for storage.

		VariableTypes variableTypes = Context.ProcessEngineConfiguration.VariableTypes;

		VariableType newType = variableTypes.findVariableType(value);

		if ((variableInstance != null) && (!variableInstance.Type.Equals(newType)))
		{
		  variableInstance.Value = null;
		  variableInstance.Type = newType;
		  variableInstance.forceUpdate();
		  variableInstance.Value = value;
		}
		else
		{
		  variableInstance.Value = value;
		}

		Context.CommandContext.HistoryManager.recordHistoricDetailVariableCreate(variableInstance, sourceActivityExecution, ActivityIdUsedForDetails);

		Context.CommandContext.HistoryManager.recordVariableUpdate(variableInstance);
	  }

	  protected internal virtual VariableInstanceEntity createVariableInstance(string variableName, object value, ExecutionEntity sourceActivityExecution)
	  {
		VariableTypes variableTypes = Context.ProcessEngineConfiguration.VariableTypes;

		VariableType type = variableTypes.findVariableType(value);

		VariableInstanceEntity variableInstance = VariableInstanceEntity.createAndInsert(variableName, type, value);
		initializeVariableInstanceBackPointer(variableInstance);

		if (variableInstances != null)
		{
			variableInstances[variableName] = variableInstance;
		}

		// Record historic variable
		Context.CommandContext.HistoryManager.recordVariableCreate(variableInstance);

		// Record historic detail
		Context.CommandContext.HistoryManager.recordHistoricDetailVariableCreate(variableInstance, sourceActivityExecution, ActivityIdUsedForDetails);

		return variableInstance;
	  }


	  /// <summary>
	  /// Execution variable updates have activity instance ids, but historic task variable updates don't.
	  /// </summary>
	  protected internal virtual bool ActivityIdUsedForDetails
	  {
		  get
		  {
			return true;
		  }
	  }

	  // getters and setters //////////////////////////////////////////////////////

	  public virtual ELContext CachedElContext
	  {
		  get
		  {
			return cachedElContext;
		  }
		  set
		  {
			this.cachedElContext = value;
		  }
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  public virtual T getVariable<T>(string variableName, Type variableClass)
	  {
		return variableClass.cast(getVariable(variableName));
	  }

	  public virtual T getVariableLocal<T>(string variableName, Type variableClass)
	  {
		return variableClass.cast(getVariableLocal(variableName));
	  }
	}

}