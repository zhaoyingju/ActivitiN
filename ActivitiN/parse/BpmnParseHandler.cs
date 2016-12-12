using System;
using System.Collections.Generic;


namespace org.activiti.engine.parse
{

    using BaseElement = org.activiti.bpmn.model.BaseElement;
    using BpmnParse = org.activiti.engine.impl.bpmn.parser.BpmnParse;
    using org.activiti.engine.impl.bpmn.parser.handler;
    using ProcessEngineConfigurationImpl = org.activiti.engine.impl.cfg.ProcessEngineConfigurationImpl;

    /// <summary>
    /// 
    /// </summary>
    public interface BpmnParseHandler
    {
        /// <summary>
        /// �ڴ�����̽����ڼ������ô˴����������͡�
        /// </summary>
        ICollection<Type> HandledTypes { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bpmnParse"></param>
        /// <param name="element"></param>
        void parse(BpmnParse bpmnParse, BaseElement element);
    }
}