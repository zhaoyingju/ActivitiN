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

namespace org.activiti.engine.impl.db
{

	using IdGenerator = org.activiti.engine.impl.cfg.IdGenerator;
	using GetNextIdBlockCmd = org.activiti.engine.impl.cmd.GetNextIdBlockCmd;
	using CommandConfig = org.activiti.engine.impl.interceptor.CommandConfig;
	using CommandExecutor = org.activiti.engine.impl.interceptor.CommandExecutor;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class DbIdGenerator : IdGenerator
	{

	  protected internal int idBlockSize;
	  protected internal long nextId = 0;
	  protected internal long lastId = -1;

	  protected internal CommandExecutor commandExecutor;
	  protected internal CommandConfig commandConfig;

	  public virtual string NextId
	  {
		  get
		  {
			  lock (this)
			  {
				if (lastId < nextId)
				{
				  NewBlock;
				}
				long _nextId = nextId++;
				return Convert.ToString(_nextId);
			  }
		  }
	  }

	  protected internal virtual void getNewBlock()
	  {
		  lock (this)
		  {
			IdBlock idBlock = commandExecutor.execute(commandConfig, new GetNextIdBlockCmd(idBlockSize));
			this.nextId = idBlock.NextId;
			this.lastId = idBlock.LastId;
		  }
	  }

	  public virtual int IdBlockSize
	  {
		  get
		  {
			return idBlockSize;
		  }
		  set
		  {
			this.idBlockSize = value;
		  }
	  }


	  public virtual CommandExecutor CommandExecutor
	  {
		  get
		  {
			return commandExecutor;
		  }
		  set
		  {
			this.commandExecutor = value;
		  }
	  }


	  public virtual CommandConfig CommandConfig
	  {
		  get
		  {
			return commandConfig;
		  }
		  set
		  {
			this.commandConfig = value;
		  }
	  }

	}

}