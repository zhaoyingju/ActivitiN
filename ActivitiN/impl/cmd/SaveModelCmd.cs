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
namespace org.activiti.engine.impl.cmd
{

	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using ModelEntity = org.activiti.engine.impl.persistence.entity.ModelEntity;


	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	[Serializable]
	public class SaveModelCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;
	  protected internal ModelEntity model;

	  public SaveModelCmd(ModelEntity model)
	  {
		this.model = model;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		if (model == null)
		{
		  throw new ActivitiIllegalArgumentException("model is null");
		}
		if (model.Id == null)
		{
		  commandContext.ModelEntityManager.insertModel(model);
		}
		else
		{
		  commandContext.ModelEntityManager.updateModel(model);
		}
		return null;
	  }

	}

}