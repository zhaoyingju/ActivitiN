using System;

namespace org.activiti.engine.repository
{

    /// <summary>
    /// ��ʾ�Ѵ��������̴洢���еĲ���
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