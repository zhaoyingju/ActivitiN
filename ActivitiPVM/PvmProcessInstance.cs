using System.Collections.Generic;

namespace org.activiti.engine.impl.pvm
{

    /// <summary>
    /// ����һ������ʵ��
    /// </summary>
    public interface PvmProcessInstance : PvmExecution
    {

        void start();

        PvmExecution findExecution(string activityId);

        IList<string> findActiveActivityIds();

        bool Ended { get; }

        void deleteCascade(string deleteReason);
    }
}