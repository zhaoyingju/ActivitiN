using System;

/* Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace org.activiti.engine.impl.bpmn.listener
{

	using DelegateTask = org.activiti.engine.@delegate.DelegateTask;
	using TaskListener = org.activiti.engine.@delegate.TaskListener;
	using Context = org.activiti.engine.impl.context.Context;
	using Expression = org.activiti.engine.impl.el.Expression;
	using ScriptingEngines = org.activiti.engine.impl.scripting.ScriptingEngines;

	/// <summary>
	/// @author Rich Kroll
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class ScriptTaskListener : TaskListener
	{

		private const long serialVersionUID = -8915149072830499057L;

	  protected internal Expression script;

	  protected internal Expression language = null;

	  protected internal Expression resultVariable = null;

	  protected internal bool autoStoreVariables;

		public virtual void notify(DelegateTask delegateTask)
		{
			validateParameters();

			ScriptingEngines scriptingEngines = Context.ProcessEngineConfiguration.ScriptingEngines;

			object result = scriptingEngines.evaluate(script.ExpressionText, language.ExpressionText, delegateTask, autoStoreVariables);

			if (resultVariable != null)
			{
				delegateTask.setVariable(resultVariable.ExpressionText, result);
			}
		}

	  protected internal virtual void validateParameters()
	  {
		if (script == null)
		{
				throw new System.ArgumentException("The field 'script' should be set on the TaskListener");
		}

			if (language == null)
			{
				throw new System.ArgumentException("The field 'language' should be set on the TaskListener");
			}
	  }

		public virtual Expression Script
		{
			set
			{
				this.script = value;
			}
		}

		public virtual Expression Language
		{
			set
			{
				this.language = value;
			}
		}

		public virtual Expression ResultVariable
		{
			set
			{
				this.resultVariable = value;
			}
		}

	  public virtual bool AutoStoreVariables
	  {
		  set
		  {
			this.autoStoreVariables = value;
		  }
	  }


	}

}