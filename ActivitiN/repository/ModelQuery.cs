namespace org.activiti.engine.repository
{

	using org.activiti.engine.query;

	/// <summary>
	/// Allows programmatic querying of <seealso cref="Model"/>s.
	/// 
	/// @author Tijs Rademakers
	/// @author Joram Barrez
	/// </summary>
	public interface ModelQuery : Query<ModelQuery, Model>
	{

	  /// <summary>
	  /// Only select model with the given id. </summary>
	  ModelQuery modelId(string modelId);

	  /// <summary>
	  /// Only select models with the given category. </summary>
	  ModelQuery modelCategory(string modelCategory);

	  /// <summary>
	  /// Only select models where the category matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%
	  /// </summary>
	  ModelQuery modelCategoryLike(string modelCategoryLike);

	  /// <summary>
	  /// Only select models that have a different category then the given one. </summary>
	  ModelQuery modelCategoryNotEquals(string categoryNotEquals);

	  /// <summary>
	  /// Only select models with the given name. </summary>
	  ModelQuery modelName(string modelName);

	  /// <summary>
	  /// Only select models where the name matches the given parameter.
	  /// The syntax that should be used is the same as in SQL, eg. %activiti%
	  /// </summary>
	  ModelQuery modelNameLike(string modelNameLike);

	  /// <summary>
	  /// Only selects models with the given key. </summary>
	  ModelQuery modelKey(string key);

	  /// <summary>
	  /// Only select model with a certain version. </summary>
	  ModelQuery modelVersion(int? modelVersion);

	  /// <summary>
	  /// Only select models which has the highest version.
	  /// 
	  /// Note: if modelKey(key) is not used in this query, all the models with
	  /// the highest version for each key will be returned (similar to process definitions) 
	  /// </summary>
	  ModelQuery latestVersion();

	  /// <summary>
	  /// Only select models that are the source for the provided deployment </summary>
	  ModelQuery deploymentId(string deploymentId);

	  /// <summary>
	  /// Only select models that are deployed (ie deploymentId != null) </summary>
	  ModelQuery deployed();

	  /// <summary>
	  /// Only select models that are not yet deployed </summary>
	  ModelQuery notDeployed();

		/// <summary>
		/// Only select models that have the given tenant id.
		/// </summary>
	  ModelQuery modelTenantId(string tenantId);

		/// <summary>
		/// Only select models with a tenant id like the given one.
		/// </summary>
	  ModelQuery modelTenantIdLike(string tenantIdLike);

		/// <summary>
		/// Only select models that do not have a tenant id.
		/// </summary>
	  ModelQuery modelWithoutTenantId();


	  // ordering ////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Order by the category of the models (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ModelQuery orderByModelCategory();

	  /// <summary>
	  /// Order by the id of the models (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ModelQuery orderByModelId();

	  /// <summary>
	  /// Order by the key of the models (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ModelQuery orderByModelKey();

	  /// <summary>
	  /// Order by the version of the models (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ModelQuery orderByModelVersion();

	  /// <summary>
	  /// Order by the name of the models (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ModelQuery orderByModelName();

	  /// <summary>
	  /// Order by the creation time of the models (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ModelQuery orderByCreateTime();

	  /// <summary>
	  /// Order by the last update time of the models (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ModelQuery orderByLastUpdateTime();

		/// <summary>
		/// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or <seealso cref="#desc()"/>). </summary>
	  ModelQuery orderByTenantId();

	}

}