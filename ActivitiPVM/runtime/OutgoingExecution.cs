namespace org.activiti.engine.impl.pvm.runtime
{

    public class OutgoingExecution
    {

        //private static Logger log = LoggerFactory.getLogger(typeof(OutgoingExecution));

        protected internal InterpretableExecution outgoingExecution;
        protected internal PvmTransition outgoingTransition;
        protected internal bool isNew;

        public OutgoingExecution(InterpretableExecution outgoingExecution, PvmTransition outgoingTransition, bool isNew)
        {
            this.outgoingExecution = outgoingExecution;
            this.outgoingTransition = outgoingTransition;
            this.isNew = isNew;
        }

        public virtual void take()
        {
            take(true);
        }

        public virtual void take(bool fireActivityCompletedEvent)
        {
            if (outgoingExecution.getReplacedBy() != null)
            {
                outgoingExecution = outgoingExecution.getReplacedBy();
            }
            if (!outgoingExecution.DeleteRoot)
            {
                outgoingExecution.take(outgoingTransition, fireActivityCompletedEvent);
            }
            else
            {
                //log.debug("Not taking transition '{}', outgoing execution has ended.", outgoingTransition);
            }
        }
    }
}