namespace org.activiti.engine.@delegate
{

    using Error = org.activiti.engine.impl.bpmn.parser.Error;


    /// <summary>
    /// 可用于抛出BPMN错误的特殊异常
    /// </summary>
    public class BpmnError : ActivitiException
    {
        private string errorCode;

        public BpmnError(string errorCode) : base("")
        {
            ErrorCode = errorCode;
        }

        public BpmnError(string errorCode, string message) : base(message + " (errorCode='" + errorCode + "')")
        {
            ErrorCode = errorCode;
        }

        protected internal virtual string ErrorCode
        {
            set
            {
                if (value == null)
                {
                    throw new ActivitiIllegalArgumentException("Error Code must not be null.");
                }
                if (value.Length < 1)
                {
                    throw new ActivitiIllegalArgumentException("Error Code must not be empty.");
                }
                this.errorCode = value;
            }
            get
            {
                return errorCode;
            }
        }

        public override string ToString()
        {
            return base.ToString() + " (errorCode='" + errorCode + "')";
        }
    }
}