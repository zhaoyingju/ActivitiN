namespace org.activiti.engine.impl.pvm.runtime
{

    using ActivityImpl = org.activiti.engine.impl.pvm.process.ActivityImpl;


    public class StartingExecution
    {

        protected internal readonly ActivityImpl selectedInitial;

        public StartingExecution(ActivityImpl selectedInitial)
        {
            this.selectedInitial = selectedInitial;
        }

        public virtual ActivityImpl Initial
        {
            get
            {
                return selectedInitial;
            }
        }

    }

}