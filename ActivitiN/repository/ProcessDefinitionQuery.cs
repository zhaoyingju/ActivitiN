using System;

namespace org.activiti.engine.repository
{

	using org.activiti.engine.query;

	/// <summary>
	/// Allows programmatic querying of <seealso cref="ProcessDefinition"/>s.
	/// </summary>
	public interface ProcessDefinitionQuery : Query<ProcessDefinitionQuery, ProcessDefinition>
	{

	  /// <summary>
	  /// Only select process definiton with the given id. </summary>
	  ProcessDefinitionQuery processDefinitionId(string processDefinitionId);

	  /// <summary>
	  /// Only select process definitions with the given ids. </summary>
	  ProcessDefinitionQuery processDefinitionIds(Set<string> processDefinitionIds);

	  /// <summary>
	  /// Only select process definitions with the given category. </summary>
	  ProcessDefinitionQuery processDefinitionCategory(string processDefinitionCategory);

	  /// <summary>
	  /// Only select process definitions where the category matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%
	  /// </summary>
	  ProcessDefinitionQuery processDefinitionCategoryLike(string processDefinitionCategoryLike);

	  /// <summary>
	  /// Only select deployments that have a different category then the given one. </summary>
	  /// <seealso cref= DeploymentBuilder#category(String)  </seealso>
	  ProcessDefinitionQuery processDefinitionCategoryNotEquals(string categoryNotEquals);

	  /// <summary>
	  /// Only select process definitions with the given name. </summary>
	  ProcessDefinitionQuery processDefinitionName(string processDefinitionName);

	  /// <summary>
	  /// Only select process definitions where the name matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%
	  /// </summary>
	  ProcessDefinitionQuery processDefinitionNameLike(string processDefinitionNameLike);

	  /// <summary>
	  /// Only select process definitions that are deployed in a deployment with the
	  /// given deployment id
	  /// </summary>
	  ProcessDefinitionQuery deploymentId(string deploymentId);

	  /// <summary>
	  /// Select process definitions that are deployed in deployments with the given set of ids </summary>
	  ProcessDefinitionQuery deploymentIds(Set<string> deploymentIds);

	  /// <summary>
	  /// Only select process definition with the given key.
	  /// </summary>
	  ProcessDefinitionQuery processDefinitionKey(string processDefinitionKey);

	  /// <summary>
	  /// Only select process definitions where the key matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%
	  /// </summary>
	  ProcessDefinitionQuery processDefinitionKeyLike(string processDefinitionKeyLike);

	  /// <summary>
	  /// Only select process definition with a certain version.
	  /// Particulary useful when used in combination with <seealso cref="#processDefinitionKey(String)"/>
	  /// </summary>
	  ProcessDefinitionQuery processDefinitionVersion(int? processDefinitionVersion);

	  /// <summary>
	  /// Only select process definitions which version are greater than a certain version.
	  /// </summary>
	  ProcessDefinitionQuery processDefinitionVersionGreaterThan(int? processDefinitionVersion);

	  /// <summary>
	  /// Only select process definitions which version are greater than or equals a certain version.
	  /// </summary>
	  ProcessDefinitionQuery processDefinitionVersionGreaterThanOrEquals(int? processDefinitionVersion);

	  /// <summary>
	  /// Only select process definitions which version are lower than a certain version.
	  /// </summary>
	  ProcessDefinitionQuery processDefinitionVersionLowerThan(int? processDefinitionVersion);

	  /// <summary>
	  /// Only select process definitions which version are lower than or equals a certain version.
	  /// </summary>
	  ProcessDefinitionQuery processDefinitionVersionLowerThanOrEquals(int? processDefinitionVersion);

	  /// <summary>
	  /// Only select the process definitions which are the latest deployed
	  /// (ie. which have the highest version number for the given key).
	  /// 
	  /// Can also be used without any other criteria (ie. query.latest().list()), which
	  /// will then give all the latest versions of all the deployed process definitions.
	  /// </summary>
	  /// <exception cref="ActivitiIllegalArgumentException"> if used in combination with  <seealso cref="#groupId(string)"/>, <seealso cref="#processDefinitionVersion(int)"/>
	  ///                           or <seealso cref="#deploymentId(String)"/> </exception>
	  ProcessDefinitionQuery latestVersion();

	  /// <summary>
	  /// Only select process definition with the given resource name. </summary>
	  ProcessDefinitionQuery processDefinitionResourceName(string resourceName);

	  /// <summary>
	  /// Only select process definition with a resource name like the given . </summary>
	  ProcessDefinitionQuery processDefinitionResourceNameLike(string resourceNameLike);

	  /// <summary>
	  /// Only selects process definitions which given userId is authoriezed to start
	  /// </summary>
	  ProcessDefinitionQuery startableByUser(string userId);

	  /// <summary>
	  /// Only selects process definitions which are suspended
	  /// </summary>
	  ProcessDefinitionQuery suspended();

	  /// <summary>
	  /// Only selects process definitions which are active
	  /// </summary>
	  ProcessDefinitionQuery active();

		/// <summary>
		/// Only select process definitions that have the given tenant id.
		/// </summary>
	  ProcessDefinitionQuery processDefinitionTenantId(string tenantId);

		/// <summary>
		/// Only select process definitions with a tenant id like the given one.
		/// </summary>
	  ProcessDefinitionQuery processDefinitionTenantIdLike(string tenantIdLike);

		/// <summary>
		/// Only select process definitions that do not have a tenant id.
		/// </summary>
	  ProcessDefinitionQuery processDefinitionWithoutTenantId();

	  // Support for event subscriptions /////////////////////////////////////

	  /// <seealso cref= #messageEventSubscriptionName(String) </seealso>
	  [Obsolete]
	  ProcessDefinitionQuery messageEventSubscription(string messageName);

	  /// <summary>
	  /// Selects the single process definition which has a start message event 
	  /// with the messageName.
	  /// </summary>
	  ProcessDefinitionQuery messageEventSubscriptionName(string messageName);

	  // ordering ////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by the category of the process definitions (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ProcessDefinitionQuery orderByProcessDefinitionCategory();

	  /// <summary>
	  /// Order by process definition key (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ProcessDefinitionQuery orderByProcessDefinitionKey();

	  /// <summary>
	  /// Order by the id of the process definitions (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ProcessDefinitionQuery orderByProcessDefinitionId();

	  /// <summary>
	  /// Order by the version of the process definitions (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ProcessDefinitionQuery orderByProcessDefinitionVersion();

	  /// <summary>
	  /// Order by the name of the process definitions (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ProcessDefinitionQuery orderByProcessDefinitionName();

	  /// <summary>
	  /// Order by deployment id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ProcessDefinitionQuery orderByDeploymentId();

		/// <summary>
		/// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or
		/// <seealso cref="#desc()"/>).
		/// </summary>
	  ProcessDefinitionQuery orderByTenantId();

	}

}