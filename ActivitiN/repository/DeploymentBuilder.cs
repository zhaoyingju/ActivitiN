using System;

namespace org.activiti.engine.repository
{


    using BpmnModel = org.activiti.bpmn.model.BpmnModel;

    /// <summary>
    /// 用于创建新部署的构建器。
    /// </summary>
    public interface DeploymentBuilder
    {

        DeploymentBuilder addInputStream(string resourceName, InputStream inputStream);
        DeploymentBuilder addClasspathResource(string resource);
        DeploymentBuilder addString(string resourceName, string text);
        DeploymentBuilder addZipInputStream(ZipInputStream zipInputStream);
        DeploymentBuilder addBpmnModel(string resourceName, BpmnModel bpmnModel);

        /// <summary>
        /// If called, no XML schema validation against the BPMN 2.0 XSD.
        /// 
        /// Not recommended in general.
        /// </summary>
        DeploymentBuilder disableSchemaValidation();

        /// <summary>
        /// If called, no validation that the process definition is executable on the engine
        /// will be done against the process definition.
        /// 
        /// Not recommended in general.
        /// </summary>
        DeploymentBuilder disableBpmnValidation();

        /// <summary>
        /// Gives the deployment the given name.
        /// </summary>
        DeploymentBuilder name(string name);

        /// <summary>
        /// Gives the deployment the given category.
        /// </summary>
        DeploymentBuilder category(string category);

        /// <summary>
        /// Gives the deployment the given tenant id.
        /// </summary>
        DeploymentBuilder tenantId(string tenantId);

        /// <summary>
        /// If set, this deployment will be compared to any previous deployment.
        /// This means that every (non-generated) resource will be compared with the
        /// provided resources of this deployment.
        /// </summary>
        DeploymentBuilder enableDuplicateFiltering();

        /// <summary>
        /// Sets the date on which the process definitions contained in this deployment
        /// will be activated. This means that all process definitions will be deployed
        /// as usual, but they will be suspended from the start until the given activation date.
        /// </summary>
        DeploymentBuilder activateProcessDefinitionsOn(DateTime date);

        /// <summary>
        /// Deploys all provided sources to the Activiti engine.
        /// </summary>
        Deployment deploy();
    }
}