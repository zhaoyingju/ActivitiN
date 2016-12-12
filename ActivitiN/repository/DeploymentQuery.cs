
namespace org.activiti.engine.repository
{

	using org.activiti.engine.query;

	/// <summary>
	/// Allows programmatic querying of <seealso cref="Deployment"/>s.
	/// 
	/// Note that it is impossible to retrieve the deployment resources through the
	/// results of this operation, since that would cause a huge transfer of
	/// (possibly) unneeded bytes over the wire.
	/// 
	/// To retrieve the actual bytes of a deployment resource use the operations on
	/// the <seealso cref="RepositoryService#getDeploymentResourceNames(String)"/> and
	/// <seealso cref="RepositoryService#getResourceAsStream(String, String)"/>
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez
	/// </summary>
	public interface DeploymentQuery : Query<DeploymentQuery, Deployment>
	{

		/// <summary>
		/// Only select deployments with the given deployment id. 
		/// </summary>
		DeploymentQuery deploymentId(string deploymentId);

		/// <summary>
		/// Only select deployments with the given name. 
		/// </summary>
		DeploymentQuery deploymentName(string name);

		/// <summary>
		/// Only select deployments with a name like the given string. 
		/// </summary>
		DeploymentQuery deploymentNameLike(string nameLike);

		/// <summary>
		/// Only select deployments with the given category.
		/// </summary>
		/// <seealso cref= DeploymentBuilder#category(String) </seealso>
		DeploymentQuery deploymentCategory(string category);

		/// <summary>
		/// Only select deployments that have a different category then the given one.
		/// </summary>
		/// <seealso cref= DeploymentBuilder#category(String) </seealso>
		DeploymentQuery deploymentCategoryNotEquals(string categoryNotEquals);

		/// <summary>
		/// Only select deployment that have the given tenant id.
		/// </summary>
		DeploymentQuery deploymentTenantId(string tenantId);

		/// <summary>
		/// Only select deployments with a tenant id like the given one.
		/// </summary>
		DeploymentQuery deploymentTenantIdLike(string tenantIdLike);

		/// <summary>
		/// Only select deployments that do not have a tenant id.
		/// </summary>
		DeploymentQuery deploymentWithoutTenantId();

		/// <summary>
		/// Only select deployments with the given process definition key. </summary>
		DeploymentQuery processDefinitionKey(string key);

		/// <summary>
		/// Only select deployments with a process definition key like the given
		/// string.
		/// </summary>
		DeploymentQuery processDefinitionKeyLike(string keyLike);



		// sorting ////////////////////////////////////////////////////////

		/// <summary>
		/// Order by deployment id (needs to be followed by <seealso cref="#asc()"/> or
		/// <seealso cref="#desc()"/>).
		/// </summary>
		DeploymentQuery orderByDeploymentId();

		/// <summary>
		/// Order by deployment name (needs to be followed by <seealso cref="#asc()"/> or
		/// <seealso cref="#desc()"/>).
		/// </summary>
		DeploymentQuery orderByDeploymentName();

		/// <summary>
		/// Order by deployment time (needs to be followed by <seealso cref="#asc()"/> or
		/// <seealso cref="#desc()"/>).
		/// </summary>
		DeploymentQuery orderByDeploymenTime();

		/// <summary>
		/// Order by tenant id (needs to be followed by <seealso cref="#asc()"/> or
		/// <seealso cref="#desc()"/>).
		/// </summary>
		DeploymentQuery orderByTenantId();
	}

}