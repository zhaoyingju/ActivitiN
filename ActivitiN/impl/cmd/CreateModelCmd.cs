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
	using Model = org.activiti.engine.repository.Model;


	/// <summary>
	/// @author Tijs Rademakers
	/// </summary>
	[Serializable]
	public class CreateModelCmd : Command<Model>
	{

	  private const long serialVersionUID = 1L;

	  public virtual Model execute(CommandContext commandContext)
	  {
		return commandContext.ModelEntityManager.createNewModel();
	  }

	}

}