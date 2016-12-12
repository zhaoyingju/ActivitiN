using System;

namespace org.activiti.engine.impl.repository
{

    using BpmnXMLConverter = org.activiti.bpmn.converter.BpmnXMLConverter;
    using BpmnModel = org.activiti.bpmn.model.BpmnModel;
    using DeploymentEntity = org.activiti.engine.impl.persistence.entity.DeploymentEntity;
    using ResourceEntity = org.activiti.engine.impl.persistence.entity.ResourceEntity;
    using IoUtil = org.activiti.engine.impl.util.IoUtil;
    using ReflectUtil = org.activiti.engine.impl.util.ReflectUtil;
    using Deployment = org.activiti.engine.repository.Deployment;
    using DeploymentBuilder = org.activiti.engine.repository.DeploymentBuilder;

    [Serializable]
    public class DeploymentBuilderImpl : DeploymentBuilder
    {
        protected internal const string DEFAULT_ENCODING = "UTF-8";

        [NonSerialized]
        protected internal RepositoryServiceImpl repositoryService;
        protected internal DeploymentEntity deployment = new DeploymentEntity();
        protected internal bool isBpmn20XsdValidationEnabled = true;
        protected internal bool isProcessValidationEnabled = true;
        protected internal bool isDuplicateFilterEnabled = false;
        protected internal DateTime processDefinitionsActivationDate;

        public DeploymentBuilderImpl(RepositoryServiceImpl repositoryService)
        {
            this.repositoryService = repositoryService;
        }

        public virtual DeploymentBuilder addInputStream(string resourceName, InputStream inputStream)
        {
            if (inputStream == null)
            {
                throw new ActivitiIllegalArgumentException("inputStream for resource '" + resourceName + "' is null");
            }
            sbyte[] bytes = IoUtil.readInputStream(inputStream, resourceName);
            ResourceEntity resource = new ResourceEntity();
            resource.Name = resourceName;
            resource.Bytes = bytes;
            deployment.addResource(resource);
            return this;
        }

        public virtual DeploymentBuilder addClasspathResource(string resource)
        {
            InputStream inputStream = ReflectUtil.getResourceAsStream(resource);
            if (inputStream == null)
            {
                throw new ActivitiIllegalArgumentException("resource '" + resource + "' not found");
            }
            return addInputStream(resource, inputStream);
        }

        public virtual DeploymentBuilder addString(string resourceName, string text)
        {
            if (text == null)
            {
                throw new ActivitiIllegalArgumentException("text is null");
            }
            ResourceEntity resource = new ResourceEntity();
            resource.Name = resourceName;
            try
            {
                resource.Bytes = text.GetBytes(DEFAULT_ENCODING);
            }
            catch (UnsupportedEncodingException e)
            {
                throw new ActivitiException("Unable to get process bytes.", e);
            }
            deployment.addResource(resource);
            return this;
        }

        public virtual DeploymentBuilder addZipInputStream(ZipInputStream zipInputStream)
        {
            try
            {
                ZipEntry entry = zipInputStream.NextEntry;
                while (entry != null)
                {
                    if (!entry.Directory)
                    {
                        string entryName = entry.Name;
                        sbyte[] bytes = IoUtil.readInputStream(zipInputStream, entryName);
                        ResourceEntity resource = new ResourceEntity();
                        resource.Name = entryName;
                        resource.Bytes = bytes;
                        deployment.addResource(resource);
                    }
                    entry = zipInputStream.NextEntry;
                }
            }
            catch (Exception e)
            {
                throw new ActivitiException("problem reading zip input stream", e);
            }
            return this;
        }

        public virtual DeploymentBuilder addBpmnModel(string resourceName, BpmnModel bpmnModel)
        {
            BpmnXMLConverter bpmnXMLConverter = new BpmnXMLConverter();
            try
            {
                string bpmn20Xml = new string(bpmnXMLConverter.convertToXML(bpmnModel), "UTF-8");
                addString(resourceName, bpmn20Xml);
            }
            catch (UnsupportedEncodingException e)
            {
                throw new ActivitiException("Errot while transforming BPMN model to xml: not UTF-8 encoded", e);
            }
            return this;
        }

        public virtual DeploymentBuilder name(string name)
        {
            deployment.Name = name;
            return this;
        }

        public virtual DeploymentBuilder category(string category)
        {
            deployment.Category = category;
            return this;
        }

        public virtual DeploymentBuilder disableBpmnValidation()
        {
            this.isProcessValidationEnabled = false;
            return this;
        }

        public virtual DeploymentBuilder disableSchemaValidation()
        {
            this.isBpmn20XsdValidationEnabled = false;
            return this;
        }

        public virtual DeploymentBuilder tenantId(string tenantId)
        {
            deployment.TenantId = tenantId;
            return this;
        }

        public virtual DeploymentBuilder enableDuplicateFiltering()
        {
            this.isDuplicateFilterEnabled = true;
            return this;
        }

        public virtual DeploymentBuilder activateProcessDefinitionsOn(DateTime date)
        {
            this.processDefinitionsActivationDate = date;
            return this;
        }

        public virtual Deployment deploy()
        {
            return repositoryService.deploy(this);
        }

        // getters and setters //////////////////////////////////////////////////////

        public virtual DeploymentEntity Deployment
        {
            get
            {
                return deployment;
            }
        }
        public virtual bool ProcessValidationEnabled
        {
            get
            {
                return isProcessValidationEnabled;
            }
        }
        public virtual bool Bpmn20XsdValidationEnabled
        {
            get
            {
                return isBpmn20XsdValidationEnabled;
            }
        }
        public virtual bool DuplicateFilterEnabled
        {
            get
            {
                return isDuplicateFilterEnabled;
            }
        }
        public virtual DateTime ProcessDefinitionsActivationDate
        {
            get
            {
                return processDefinitionsActivationDate;
            }
        }

    }

}