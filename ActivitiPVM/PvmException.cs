using System;

namespace org.activiti.engine.impl.pvm
{


    /// <summary>
    /// �쳣
    /// </summary>
    public class PvmException : Exception
    {
        public PvmException(string message, Exception cause) : base(message, cause)
        {
        }

        public PvmException(string message) : base(message)
        {
        }
    }

}