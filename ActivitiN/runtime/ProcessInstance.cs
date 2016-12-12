using System.Collections.Generic;

namespace org.activiti.engine.runtime
{

    using ProcessDefinition = org.activiti.engine.repository.ProcessDefinition;



    /// <summary>
    /// Á÷³ÌÊµÀý
    /// </summary>
    public interface ProcessInstance : Execution
    {

        /// <summary>
        /// The id of the process definition of the process instance.
        /// </summary>
        string ProcessDefinitionId { get; }

        /// <summary>
        /// The name of the process definition of the process instance.
        /// </summary>
        string ProcessDefinitionName { get; }

        /// <summary>
        /// The key of the process definition of the process instance.
        /// </summary>
        string ProcessDefinitionKey { get; }

        /// <summary>
        /// The version of the process definition of the process instance.
        /// </summary>
        int? ProcessDefinitionVersion { get; }

        /// <summary>
        /// The deployment id of the process definition of the process instance.
        /// </summary>
        string DeploymentId { get; }

        /// <summary>
        /// The business key of this process instance.
        /// </summary>
        string BusinessKey { get; }

        /// <summary>
        /// returns true if the process instance is suspended
        /// </summary>
        bool Suspended { get; }

        /// <summary>
        /// Returns the process variables if requested in the process instance query 
        /// </summary>
        IDictionary<string, object> ProcessVariables { get; }

        /// <summary>
        /// The tenant identifier of this process instance 
        /// </summary>
        string TenantId { get; }

        /// <summary>
        /// Returns the name of this process instance. 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns the description of this process instance.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Returns the localized name of this process instance.
        /// </summary>
        string LocalizedName { get; }

        /// <summary>
        /// Returns the localized description of this process instance.
        /// </summary>
        string LocalizedDescription { get; }
    }

}