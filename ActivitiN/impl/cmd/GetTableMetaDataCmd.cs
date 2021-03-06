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
	using TableMetaData = org.activiti.engine.management.TableMetaData;


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class GetTableMetaDataCmd : Command<TableMetaData>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string tableName;

	  public GetTableMetaDataCmd(string tableName)
	  {
		this.tableName = tableName;
	  }

	  public virtual TableMetaData execute(CommandContext commandContext)
	  {
		if (tableName == null)
		{
		  throw new ActivitiIllegalArgumentException("tableName is null");
		}
		return commandContext.TableDataManager.getTableMetaData(tableName);
	  }

	}

}