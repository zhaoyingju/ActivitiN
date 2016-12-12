using System;
using System.Linq;
using bpmn_converter.converter.util;
using org.activiti.bpmn.model;

namespace org.activiti.bpmn.converter.child
{







    public class ActivitiFailedjobRetryParser : BaseChildElementParser
    {

        //@Override

        public override String getElementName()
        {
            return FAILED_JOB_RETRY_TIME_CYCLE;
        }

        //@Override

        public override void parseChildElement(XMLStreamReader xtr,
            BaseElement parentElement, BpmnModel model)
        {
            if (parentElement as Activity == null)
                return;
            String cycle = xtr.getElementText();
            if (cycle == null | !cycle.Any())
                return;
            ((Activity) parentElement).setFailedJobRetryTimeCycleValue(cycle);
        }

    }
}