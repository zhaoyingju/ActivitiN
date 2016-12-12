using System.Collections.Generic;

namespace org.activiti.engine.impl.cmd
{

	using BpmnModel = org.activiti.bpmn.model.BpmnModel;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ProcessValidator = org.activiti.validation.ProcessValidator;
	using ValidationError = org.activiti.validation.ValidationError;

	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	public class ValidateBpmnModelCmd : Command<IList<ValidationError>>
	{

		protected internal BpmnModel bpmnModel;

		public ValidateBpmnModelCmd(BpmnModel bpmnModel)
		{
			this.bpmnModel = bpmnModel;
		}

		public override IList<ValidationError> execute(CommandContext commandContext)
		{
			ProcessValidator processValidator = commandContext.ProcessEngineConfiguration.ProcessValidator;
			if (processValidator == null)
			{
				throw new ActivitiException("No process validator defined");
			}

			return processValidator.validate(bpmnModel);
		}

	}

}