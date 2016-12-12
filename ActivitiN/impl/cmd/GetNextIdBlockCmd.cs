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

	using IdBlock = org.activiti.engine.impl.db.IdBlock;
	using org.activiti.engine.impl.interceptor;
	using CommandContext = org.activiti.engine.impl.interceptor.CommandContext;
	using PropertyEntity = org.activiti.engine.impl.persistence.entity.PropertyEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class GetNextIdBlockCmd : Command<IdBlock>
	{

	  private const long serialVersionUID = 1L;
	  protected internal int idBlockSize;

	  public GetNextIdBlockCmd(int idBlockSize)
	  {
		this.idBlockSize = idBlockSize;
	  }

	  public virtual IdBlock execute(CommandContext commandContext)
	  {
		PropertyEntity property = (PropertyEntity) commandContext.PropertyEntityManager.findPropertyById("next.dbid");
		long oldValue = Convert.ToInt64(property.Value);
		long newValue = oldValue + idBlockSize;
		property.Value = Convert.ToString(newValue);
		return new IdBlock(oldValue, newValue-1);
	  }
	}

}