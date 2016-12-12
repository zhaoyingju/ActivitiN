using System.Collections.Generic;

namespace org.activiti.engine.impl.pvm
{
    /// <summary>
    /// Ö´ÐÐÊµÀý
    /// </summary>
    public interface PvmExecution
    {
        void signal(string signalName, object signalData);

        PvmActivity Activity { get; }

        bool hasVariable(string variableName);

        void setVariable(string variableName, object value);

        object getVariable(string variableName);

        IDictionary<string, object> Variables { get; }
    }
}