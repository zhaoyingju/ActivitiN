using System;

namespace org.activiti.engine.repository
{

    /// <summary>
    /// ��ʾ�洢��ģ�Ϳ��е�ģ�͡�
    /// </summary>
    public interface Model
    {

        string Id { get; }

        string Name { get; set; }

        string Key { get; set; }

        string Category { get; set; }

        DateTime CreateTime { get; }

        DateTime LastUpdateTime { get; }

        int? Version { get; set; }

        string MetaInfo { get; set; }

        string DeploymentId { get; set; }

        string TenantId { set; get; }

        /// <summary>
        /// whether this model has editor source </summary>
        bool hasEditorSource();

        /// <summary>
        /// whether this model has editor source extra </summary>
        bool hasEditorSourceExtra();
    }

}