using System.Collections.Generic;

namespace org.activiti.engine.impl.pvm
{
    public interface PvmScope : PvmProcessElement
    {
        IList<PvmActivity> Activities { get; }

        PvmActivity findActivity(string activityId);
    }
}