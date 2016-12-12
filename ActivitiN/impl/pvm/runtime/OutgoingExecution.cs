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

namespace org.activiti.engine.impl.pvm.runtime
{

	using Logger = org.slf4j.Logger;
	using LoggerFactory = org.slf4j.LoggerFactory;

	/// 
	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class OutgoingExecution
	{

	  private static Logger log = LoggerFactory.getLogger(typeof(OutgoingExecution));

	  protected internal InterpretableExecution outgoingExecution;
	  protected internal PvmTransition outgoingTransition;
	  protected internal bool isNew;

	  public OutgoingExecution(InterpretableExecution outgoingExecution, PvmTransition outgoingTransition, bool isNew)
	  {
		this.outgoingExecution = outgoingExecution;
		this.outgoingTransition = outgoingTransition;
		this.isNew = isNew;
	  }

	  public virtual void take()
	  {
		  take(true);
	  }

	  public virtual void take(bool fireActivityCompletedEvent)
	  {
		if (outgoingExecution.getReplacedBy() != null)
		{
		  outgoingExecution = outgoingExecution.getReplacedBy();
		}
		if (!outgoingExecution.DeleteRoot)
		{
		  outgoingExecution.take(outgoingTransition, fireActivityCompletedEvent);
		}
		else
		{
		  log.debug("Not taking transition '{}', outgoing execution has ended.", outgoingTransition);
		}
	  }
	}
}