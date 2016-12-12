using System.Collections.Generic;

namespace org.activiti.engine.@delegate
{

    using BpmnModel = org.activiti.bpmn.model.BpmnModel;
    using ExtensionElement = org.activiti.bpmn.model.ExtensionElement;
    using FieldExtension = org.activiti.bpmn.model.FieldExtension;
    using FlowElement = org.activiti.bpmn.model.FlowElement;
    using TaskWithFieldExtensions = org.activiti.bpmn.model.TaskWithFieldExtensions;
    using Context = org.activiti.engine.impl.context.Context;
    using ExpressionManager = org.activiti.engine.impl.el.ExpressionManager;
    using FixedValue = org.activiti.engine.impl.el.FixedValue;
    using ActivityBehavior = org.activiti.engine.impl.pvm.@delegate.ActivityBehavior;
    using StringUtils = org.apache.commons.lang3.StringUtils;

    public class DelegateHelper
    {

        public static BpmnModel getBpmnModel(DelegateExecution execution)
        {
            if (execution == null)
            {
                throw new ActivitiException("Null execution passed");
            }
            return Context.CommandContext.ProcessEngineConfiguration.DeploymentManager.getBpmnModelById(execution.ProcessDefinitionId);
        }


        public static FlowElement getFlowElement(DelegateExecution execution)
        {
            BpmnModel bpmnModel = getBpmnModel(execution);
            FlowElement flowElement = bpmnModel.getFlowElement(execution.CurrentActivityId);
            if (flowElement == null)
            {
                throw new ActivitiException("Could not find a FlowElement for activityId " + execution.CurrentActivityId);
            }
            return flowElement;
        }


        public static IDictionary<string, IList<ExtensionElement>> getExtensionElements(DelegateExecution execution)
        {
            return getFlowElement(execution).ExtensionElements;
        }


        public static IList<FieldExtension> getFields(DelegateExecution execution)
        {
            FlowElement flowElement = getFlowElement(execution);
            if (flowElement is TaskWithFieldExtensions)
            {
                return ((TaskWithFieldExtensions)flowElement).FieldExtensions;
            }
            return new List<FieldExtension>();
        }


        public static FieldExtension getField(DelegateExecution execution, string fieldName)
        {
            IList<FieldExtension> fieldExtensions = getFields(execution);
            if (fieldExtensions == null || fieldExtensions.Count == 0)
            {
                return null;
            }
            foreach (FieldExtension fieldExtension in fieldExtensions)
            {
                if (fieldExtension.FieldName != null && fieldExtension.FieldName.Equals(fieldName))
                {
                    return fieldExtension;
                }
            }
            return null;
        }


        public static Expression createExpressionForField(FieldExtension fieldExtension)
        {
            if (StringUtils.isNotEmpty(fieldExtension.Expression))
            {
                ExpressionManager expressionManager = Context.ProcessEngineConfiguration.ExpressionManager;
                return expressionManager.createExpression(fieldExtension.Expression);
            }
            else
            {
                return new FixedValue(fieldExtension.StringValue);
            }
        }


        public static Expression getFieldExpression(DelegateExecution execution, string fieldName)
        {
            FieldExtension fieldExtension = getField(execution, fieldName);
            if (fieldExtension != null)
            {
                return createExpressionForField(fieldExtension);
            }
            return null;
        }
    }
}