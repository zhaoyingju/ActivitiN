using System.Collections.Generic;

namespace org.activiti.engine.impl.pvm
{


    /// <summary>
    /// ���̻
    /// </summary>
    public interface PvmActivity : PvmScope
    {

        bool Async { get; }

        bool Exclusive { get; }

        PvmScope Parent { get; }

        IList<PvmTransition> IncomingTransitions { get; }

        IList<PvmTransition> OutgoingTransitions { get; }

        PvmTransition findOutgoingTransition(string transitionId);
    }

}