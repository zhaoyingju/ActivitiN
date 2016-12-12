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
namespace org.activiti.engine.impl.pvm.@delegate
{



	/// @deprecated use <seealso cref="org.activiti.delegate.ExecutionListener"/> instead.
	/// 
	/// @author Tom Baeyens
	/// @author Joram Barrez 
	[Obsolete("use <seealso cref="org.activiti.@delegate.ExecutionListener"/> instead.")]
	public interface ExecutionListener
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void notify(ExecutionListenerExecution execution) throws Exception;
	  void notify(ExecutionListenerExecution execution);
	}

	public static class ExecutionListener_Fields
	{
	  public const string EVENTNAME_START = "start";
	  public const string EVENTNAME_END = "end";
	  public const string EVENTNAME_TAKE = "take";
	}

}