using System;

namespace org.activiti.engine.repository
{

    /// <summary>
    /// 表示已存在于流程存储库中的部署。
    /// </summary>
    public interface Deployment
    {
        string Id { get; }

        string Name { get; }

        DateTime DeploymentTime { get; }

        string Category { get; }

        string TenantId { get; }
    }
}