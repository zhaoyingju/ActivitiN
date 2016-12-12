using System;
using System.Collections.Generic;

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

namespace org.activiti.engine.impl.bpmn.parser
{


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	[Serializable]
	public class ErrorEventDefinition
	{

	  public static IComparer<ErrorEventDefinition> comparator = new ComparatorAnonymousInnerClassHelper();

	  private class ComparatorAnonymousInnerClassHelper : IComparer<ErrorEventDefinition>
	  {
		  public ComparatorAnonymousInnerClassHelper()
		  {
		  }

		  public virtual int Compare(ErrorEventDefinition o1, ErrorEventDefinition o2)
		  {
			return o2.Precedence.compareTo(o1.Precedence);
		  }
	  }

	  private const long serialVersionUID = 1L;

	  protected internal readonly string handlerActivityId;
	  protected internal string errorCode;
	  protected internal int? precedence = 0;

	  public ErrorEventDefinition(string handlerActivityId)
	  {
		this.handlerActivityId = handlerActivityId;
	  }

	  public virtual string ErrorCode
	  {
		  get
		  {
			return errorCode;
		  }
		  set
		  {
			this.errorCode = value;
		  }
	  }


	  public virtual string HandlerActivityId
	  {
		  get
		  {
			return handlerActivityId;
		  }
	  }

	  public virtual int? Precedence
	  {
		  get
		  {
			// handlers with error code take precedence over catchall-handlers
			return precedence + (errorCode != null ? 1 : 0);
		  }
		  set
		  {
			this.precedence = value;
		  }
	  }


	  public virtual bool catches(string errorCode)
	  {
		return errorCode == null || this.errorCode == null || this.errorCode.Equals(errorCode);
	  }

	}

}